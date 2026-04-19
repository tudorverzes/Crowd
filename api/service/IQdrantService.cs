using api.model;
using Qdrant.Client.Grpc;

namespace api.service;

public interface IQdrantService
{
	Task EnsureCollectionsExistAsync();
	Task UpsertEventAsync(Event eventEntity, float[] vector);
	Task UpsertAdAsync(Ad ad, float[] vector);
	Task<IReadOnlyList<ScoredPoint>> SearchEventsForAdAsync(
		float[] vector, 
		DateTime startDate, 
		DateTime endDate, 
		string? country = null, 
		string? city = null,
		string? title = null,
		int limit = 10);
}