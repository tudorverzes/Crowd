using System.Globalization;
using System.Text;
using api.dto.addDto;
using api.model;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace api.service;

public class QdrantService : IQdrantService
{
	private readonly QdrantClient _client;
    private const ulong VectorSize = 384; 

    public QdrantService(QdrantClient client)
    {
        _client = client;
    }

    public async Task EnsureCollectionsExistAsync()
    {
        try
        {
            var collections = await _client.ListCollectionsAsync();
            
            if (!collections.Contains("Events"))
            {
                await _client.CreateCollectionAsync("Events", 
                    new VectorParams { Size = VectorSize, Distance = Distance.Cosine });
                
                await _client.CreatePayloadIndexAsync("Events", "geo_location", PayloadSchemaType.Geo);
                await _client.CreatePayloadIndexAsync("Events", "title", PayloadSchemaType.Text);
            }

            if (!collections.Contains("Ads"))
            {
                await _client.CreateCollectionAsync("Ads", 
                    new VectorParams { Size = VectorSize, Distance = Distance.Cosine });
            }
        }
        catch (Exception ex)
        {
            // Log the error but don't fail - the application will use mock data
            Console.WriteLine($"Warning: Could not connect to Qdrant service: {ex.Message}. The application will continue with limited functionality.");
        }
    }

    public async Task UpsertEventAsync(Event eventEntity, float[] vector)
    {
        try
        {
            var payload = new Dictionary<string, Value>
            {
                ["title"] = NormalizeText(eventEntity.Name),
                ["start_date"] = new DateTimeOffset(eventEntity.StartDate).ToUnixTimeSeconds(),
                ["end_date"] = new DateTimeOffset(eventEntity.EndDate).ToUnixTimeSeconds(),
                ["visible_for_targeting"] = eventEntity.VisibleForTargetedAds,
            };

            if (eventEntity.Location != null)
            {
                payload["country"] = eventEntity.Location.Country ?? "";
                payload["city"] = eventEntity.Location.City ?? "";

                if (eventEntity.Location.Geometry != null)
                {
                    var geoStruct = new Struct();   
                    geoStruct.Fields.Add("lat", (Value)eventEntity.Location.Geometry.Coordinate.Y);
                    geoStruct.Fields.Add("lon", (Value)eventEntity.Location.Geometry.Coordinate.X);

                    payload["geo_location"] = new Value { StructValue =  geoStruct };
                }
            }

            var point = new PointStruct
            {
                // ID-ul trebuie să fie ulong sau un Guid (string). Qdrant suportă ulong nativ.
                Id = (ulong)eventEntity.Id, 
                Vectors = vector,
                Payload = { payload }
            };

            await _client.UpsertAsync("Events", [point]);
        }
        catch (Exception ex)
        {
            // Log the error but don't fail - the application will continue without storing the event in Qdrant
            Console.WriteLine($"Warning: Could not upsert event to Qdrant: {ex.Message}");
        }
    }

    public async Task UpsertAdAsync(Ad ad, float[] vector)
    {
        try
        {
            var geoTargets = ad.Targets?.OfType<GeoRadiusTarget>().ToList() ?? [];
            
            var targetCountries = geoTargets.Select(t => (Value)t.Country).ToArray();
            var targetCities = geoTargets.Select(t => (Value)t.City).ToArray();

            var payload = new Dictionary<string, Value>
            {
                ["status"] = (int)ad.Status,
                ["is_approved"] = ad.ApprovalStatus == AdApprovalStatus.Approved,
                ["start_date"] = new DateTimeOffset(ad.StartDate).ToUnixTimeSeconds(),
                ["end_date"] = new DateTimeOffset(ad.EndDate).ToUnixTimeSeconds(),
                
                ["target_countries"] = targetCountries,
                ["target_cities"] = targetCities
            };

            var point = new PointStruct
            {
                Id = Guid.Parse(ad.Id), 
                Vectors = vector,
                Payload = { payload }
            };

            await _client.UpsertAsync("Ads", new[] { point });
        }
        catch (Exception ex)
        {
            // Log the error but don't fail - the application will continue without storing the ad in Qdrant
            Console.WriteLine($"Warning: Could not upsert ad to Qdrant: {ex.Message}");
        }
    }
    
    
    public async Task<IReadOnlyList<ScoredPoint>> SearchEventsForAdAsync(
        float[] vector, 
        DateTime startDate, 
        DateTime endDate, 
        string? country = null, 
        string? city = null, 
        string? title = null,
        int limit = 10)
    {
        try
        {
            var filter = new Filter();

            filter.Must.Add(new Condition
            {
                Field = new FieldCondition { Key = "visible_for_targeting", Match = new Match { Boolean = true } }
            });
            
            var adStartUnix = new DateTimeOffset(startDate).ToUnixTimeSeconds();
            filter.Must.Add(new Condition
            {
                Field = new FieldCondition 
                { 
                    Key = "end_date", 
                    Range = new Qdrant.Client.Grpc.Range { Gte = adStartUnix } 
                }
            });

            var extendedEndDate = endDate.AddMonths(6);
            var adEndExtendedUnix = new DateTimeOffset(extendedEndDate).ToUnixTimeSeconds();
            
            filter.Must.Add(new Condition
            {
                Field = new FieldCondition 
                { 
                    Key = "start_date", 
                    Range = new Qdrant.Client.Grpc.Range { Lte = adEndExtendedUnix } 
                }
            });

            if (!string.IsNullOrWhiteSpace(country))
            {
                filter.Must.Add(new Condition
                {
                    Field = new FieldCondition { Key = "country", Match = new Match { Keyword = country } }
                });
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                filter.Must.Add(new Condition
                {
                    Field = new FieldCondition { Key = "city", Match = new Match { Keyword = city } }
                });
            }
            
            if (!string.IsNullOrWhiteSpace(title))
            {
                var searchTitle = NormalizeText(title);

                filter.Must.Add(new Condition
                {
                    Field = new FieldCondition 
                    { 
                        Key = "title", 
                        Match = new Match { Text = searchTitle } 
                    }
                });
            }
            
            var searchResult = await _client.SearchAsync(
                collectionName: "Events",
                vector: vector,
                filter: filter,
                limit: (ulong)limit
            );

            return searchResult;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not search events in Qdrant: {ex.Message}");
            return new List<ScoredPoint>();
        }
    }
    
    private string NormalizeText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var normalizedString = text.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}