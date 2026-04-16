# Usage Guide for Location and NetTopology Mappers

## Overview
This guide explains how to use the updated mappers when creating Events and Ads with locations.

## Creating an Event with Location

### DTO Input (from Frontend - Unchanged)
```csharp
var createEventDto = new CreateEventDto
{
    Name = "Tech Conference 2024",
    StartDate = new DateTime(2024, 6, 15),
    EndDate = new DateTime(2024, 6, 17),
    Capacity = 500,
    Overselling = false,
    Location = new LocationDto
    {
        VenueName = "Convention Center",
        AddressLine = "123 Main St",
        Country = "USA",
        StateOrRegion = "California",
        City = "San Francisco",
        Coordinates = new CoordinatesDto
        {
            Lat = "37",      // String from country-state-city library
            Lng = "-122"     // String from country-state-city library
        }
    }
};
```

### Backend Conversion
```csharp
// In EventController or EventService
var eventEntity = createEventDto.FromCreateEventDto(); // Uses existing mapper
// LocationDto.ToLocationEntity() is automatically called internally
// Result: Location entity with:
// - Coordinates: Lat=37, Lng=-122 (as ints)
// - Geometry: Point(-122, 37) in WGS84 projection
```

## Creating an Ad with Geo Targets

### DTO Input (from Frontend - Unchanged)
```csharp
var createAdDto = new CreateAdDt
{
    Title = "Summer Sale",
    Description = "50% off everything",
    Keywords = new List<string> { "sale", "summer" },
    GeoTargets = new List<CreateGeoTargetDto>
    {
        new CreateGeoTargetDto
        {
            Country = "USA",
            City = "San Francisco",
            Latitude = 37.7749,      // Double from country-state-city library
            Longitude = -122.4194,   // Double from country-state-city library
            RadiusInKm = 10
        },
        new CreateGeoTargetDto
        {
            Country = "USA",
            City = "New York",
            Latitude = 40.7128,
            Longitude = -74.0060,
            RadiusInKm = 15
        }
    },
    SpecificEventTargetIds = new List<int> { 1, 2 },
    StartDate = DateTime.UtcNow,
    EndDate = DateTime.UtcNow.AddMonths(1),
    IsDraft = false
};
```

### Backend Conversion
```csharp
// In AdController or AdService
var ad = new Ad
{
    Title = createAdDto.Title,
    Description = createAdDto.Description,
    OwnerId = userId,
    Targets = new List<AdTarget>()
};

// Map geo targets using the new GeoRadiusTargetMapper
foreach (var geoTarget in createAdDto.GeoTargets)
{
    ad.Targets.Add(geoTarget.ToGeoRadiusTarget());
    // Result: GeoRadiusTarget with:
    // - Country: "USA"
    // - City: "San Francisco"
    // - RadiusInKm: 10
    // - Location: Location entity with:
    //   - Coordinates: Lat=37, Lng=-122 (converted from double to int)
    //   - Geometry: Point(-122.4194, 37.7749) in WGS84 projection
}

// Map event targets
foreach (var eventId in createAdDto.SpecificEventTargetIds)
{
    ad.Targets.Add(new SpecificEventTarget { EventId = eventId });
}

await adRepository.CreateAsync(ad);
```

## Querying Locations with Geometry

### Getting an Event with Its Location Geometry
```csharp
var @event = await _eventRepository.GetByIdAsync(eventId);
// Location.Geometry is now available for spatial queries
// Example: Point(-122, 37) with spatial operations

if (@event?.Location?.Geometry != null)
{
    // Ready for distance calculations
    // distance = eventLocation.Geometry.Distance(adTarget.Location.Geometry);
}
```

### Getting Ads with Geo Target Locations
```csharp
var ads = await adRepository.GetAllAsync();
// Each GeoRadiusTarget.Location is eagerly loaded with Geometry data

foreach (var ad in ads)
{
    foreach (var geoTarget in ad.Targets.OfType<GeoRadiusTarget>())
    {
        if (geoTarget.Location?.Geometry != null)
        {
            // Ready for spatial operations
            var radiusKm = geoTarget.RadiusInKm;
            // Can now calculate if event is within ad's geo radius
        }
    }
}
```

## Returning Locations in API Responses

### Event API Response (DTO)
```csharp
// In EventMapper or similar
var eventDto = @event.ToEventDto(userPermission);
// eventDto.Location will be LocationDto with:
// - Coordinates: Lat="37", Lng="-122" (as strings)
// - Geometry is NOT serialized to DTO (kept server-side only)
```

### Ad API Response
```csharp
// Create a similar mapper for Ad DTOs
var adDto = new AdDto
{
    Id = ad.Id,
    Title = ad.Title,
    Targets = ad.Targets.Select(t =>
    {
        if (t is GeoRadiusTarget geoTarget)
        {
            return new GeoTargetDto
            {
                Country = geoTarget.Country,
                City = geoTarget.City,
                Latitude = geoTarget.Location?.Coordinates?.Lat ?? 0,
                Longitude = geoTarget.Location?.Coordinates?.Lng ?? 0,
                RadiusInKm = geoTarget.RadiusInKm
            };
        }
        // Handle SpecificEventTarget...
        return null;
    }).ToList()
};
```

## Key Points to Remember

1. **DTOs remain unchanged**: Frontend sends coordinates as strings (LocationDto) or doubles (CreateGeoTargetDto)
2. **Server-side conversion**: Mappers automatically convert to Location entities with geometry
3. **Coordinate storage**: Integers stored in database for backward compatibility and DTO serialization
4. **Geometry storage**: Points stored as geography columns for spatial queries (NOT serialized to JSON)
5. **SRID 4326**: All geometries use WGS84 standard projection (latitude/longitude)
6. **Coordinate ordering**: NetTopology uses (longitude, latitude) order per WGS84 standard
7. **Eager loading**: Always use Include/ThenInclude to load Location navigation properties

## Future Spatial Query Examples

Once this foundation is in place, you can implement:

```csharp
// Find all ads targeting an event location
var eventGeometry = @event.Location.Geometry;
var targetingAds = ads
    .Where(a => a.Targets.OfType<GeoRadiusTarget>()
        .Any(gt => eventGeometry.Distance(gt.Location.Geometry) <= gt.RadiusInKm * 1000)) // meters
    .ToList();

// Find all events within an ad's geo radius
var adTarget = ad.Targets.OfType<GeoRadiusTarget>().First();
var withinAds = events
    .Where(e => e.Location.Geometry.Distance(adTarget.Location.Geometry) <= adTarget.RadiusInKm * 1000)
    .ToList();
```

