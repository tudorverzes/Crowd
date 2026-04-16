# Location and NetTopology Migration Implementation

## Overview
This implementation migrates the CrowdApp backend from using raw int/double coordinates to a structured `Location` entity with NetTopology support for both `GeoRadiusTarget` (Ad geo-targeting) and `Event` locations.

## Key Changes

### 1. **NuGet Package Additions** (`api.csproj`)
- **NetTopologySuite 2.5.1**: Provides geometric types and spatial operations
- **Microsoft.EntityFrameworkCore.Nts 8.0.0**: Enables EF Core integration with NetTopology

### 2. **Model Updates**

#### Location.cs
- Added `Point? Geometry` property to `Location` class for NetTopology spatial data
- Maintains existing `Coordinates` (int-based) structure for backward compatibility and DTO serialization
- Point geometry uses WGS84 coordinate reference system (SRID 4326)

#### Ad.cs (GeoRadiusTarget)
- **Removed**: Raw `double Latitude` and `double Longitude` properties
- **Added**: `Location? Location` property to reference the new Location entity
- Preserves `Country`, `City`, and `RadiusInKm` fields for business logic

### 3. **Mapper Implementations**

#### LocationMapper.cs (Enhanced)
- Updated `ToLocationEntity()` to create `Point` geometry from int coordinates
- Uses `GeometryFactory` with WGS84 SRID (4326)
- Converts int coordinates to double for spatial calculations (assumes standard lat/lng range)
- Maintains string coordinate serialization in DTOs

#### GeoRadiusTargetMapper.cs (New)
- Maps `CreateGeoTargetDto` (with double lat/lng from JS) to `GeoRadiusTarget` entity
- Creates complete `Location` object with:
  - Coordinates from DTO values
  - NetTopology Point geometry from coordinates
  - City and Country metadata
- Centralizes coordinate conversion logic for ad targeting

### 4. **Database Configuration** (ApplicationDbContext.cs)

#### GeoRadiusTarget Location Configuration
```csharp
builder.Entity<GeoRadiusTarget>()
    .OwnsOne(g => g.Location, locationBuilder =>
    {
        // Coordinates as nested owned type
        locationBuilder.OwnsOne(l => l.Coordinates, coordinatesBuilder =>
        {
            coordinatesBuilder.Property(c => c.Lat).HasColumnName("Location_Coordinates_Lat");
            coordinatesBuilder.Property(c => c.Lng).HasColumnName("Location_Coordinates_Lng");
        });
        
        // NetTopology geometry for spatial queries
        locationBuilder.Property(l => l.Geometry)
            .HasColumnName("Location_Geometry")
            .HasColumnType("geography");
    });
```

#### Event Location Configuration
- Updated with same NetTopology geometry support as GeoRadiusTarget
- Both now support distance-based spatial queries

### 5. **Program.cs (Service Configuration)**
```csharp
// Added NetTopology support to DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    );
});

// Registered AdRepository in DI
builder.Services.AddScoped<IAdRepository, AdRepository>();
```

### 6. **AdRepository.cs (Enhanced)**
- Updated all query methods to include Location navigation property via Include/ThenInclude
- Methods affected:
  - `GetAllAsync()`
  - `GetAllForUserAsync()`
  - `GetAllUnapprovedAsync()`
  - `GetByIdAsync()`
- Ensures Location data is eagerly loaded with GeoRadiusTarget entities

## DTO Contract (Unchanged)
Both `LocationDto` and `CreateGeoTargetDto` remain unchanged:
- `LocationDto.Coordinates` still contains `string Lat` and `string Lng`
- `CreateGeoTargetDto` still receives `double Latitude` and `double Longitude`
- Conversion to structured Location entities happens transparently on the API side

## Database Migration Required
A new Entity Framework migration is needed:
```powershell
cd api
dotnet ef migrations add AddLocationToGeoRadiusTargetAndNetTopology
dotnet ef database update
```

This migration will:
1. Add Location_VenueName, Location_AddressLine, Location_Country, Location_StateOrRegion, Location_City columns to AdTarget table
2. Add Location_Coordinates_Lat, Location_Coordinates_Lng columns to AdTarget table
3. Add Location_Geometry column (geography type) to AdTarget table
4. Add Location_Geometry column (geography type) to Events table
5. Handle null values for existing records appropriately

## Backward Compatibility Notes
- Existing int coordinates in the `Coordinates` property are preserved
- Old raw coordinates from ad targeting data will need to be migrated to the Location entity structure
- Migration script should handle mapping old `Latitude`/`Longitude` doubles to new `Location.Coordinates` ints

## Future Enhancements Enabled
With this foundation, the following spatial queries are now possible:
- Find all ads within X km of an event location
- Find events within a geo-targeted ad radius
- Spatial distance calculations using NetTopology
- Advanced geospatial filtering for the recommendation engine

## File Modification Summary
1. ✅ `api.csproj` - Added NuGet packages
2. ✅ `model/Location.cs` - Added Point geometry
3. ✅ `model/Ad.cs` - GeoRadiusTarget now uses Location
4. ✅ `mappers/LocationMapper.cs` - Enhanced with geometry creation
5. ✅ `mappers/GeoRadiusTargetMapper.cs` - New mapper for ad geo targets
6. ✅ `data/ApplicationDbContext.cs` - Configuration for both locations
7. ✅ `Program.cs` - NetTopology setup and DI registration
8. ✅ `repository/AdRepository.cs` - Enhanced with Location loading

## Testing Checklist
- [ ] Migration successfully applies to database
- [ ] Event creation with location still works
- [ ] Ad creation with geo targets creates Location entities correctly
- [ ] Location queries include geometry data
- [ ] Coordinate conversion from int to geometry is accurate
- [ ] Spatial distance calculations work correctly

