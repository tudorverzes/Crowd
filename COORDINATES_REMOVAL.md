# EventCoordinates Removal - Simplified to Point-Only Storage

## Overview
Removed the redundant `EventCoordinates` class since the NetTopology `Point` geometry already stores all the spatial data we need. The system is now cleaner and eliminates duplication.

## Architecture Simplification

### Before
```
EventLocation {
    VenueName, AddressLine, Country, StateOrRegion, City
    EventCoordinates { Lat: int, Lng: int }
    Point? Geometry
}
```

### After
```
EventLocation {
    VenueName, AddressLine, Country, StateOrRegion, City
    Point? Geometry  // Single source of truth for coordinates
}
```

## Changes Made

### 1. **model/Location.cs**
   - ✅ Removed `EventCoordinates` class entirely
   - ✅ Removed `Coordinates` property from `EventLocation`
   - Keeps only: VenueName, AddressLine, Country, StateOrRegion, City, Geometry

### 2. **mappers/LocationMapper.cs**
   - ✅ Updated `ToLocationDto()` to extract coordinates from Point geometry using new helper
   - ✅ Added `ExtractCoordinatesFromGeometry()` helper method to read coordinates from Point
   - ✅ Updated `ToLocationEntity()` to only set Geometry, not EventCoordinates
   - ✅ Coordinates are now derived from Point when needed for DTO serialization

### 3. **mappers/GeoRadiusTargetMapper.cs**
   - ✅ Removed `EventCoordinates` instantiation
   - ✅ Only sets `Geometry` property when creating `EventLocation`

### 4. **data/ApplicationDbContext.cs**
   - ✅ Removed `OwnsOne(l => l.Coordinates)` configuration from GeoRadiusTarget Location
   - ✅ Removed `OwnsOne(l => l.Coordinates)` configuration from Event Location
   - ✅ Simplified to only configure the 5 string properties + geometry

## Benefits

1. **Single Source of Truth**: Coordinates are only stored in the Point geometry
2. **Reduced Storage**: No duplicate coordinate columns in database
3. **Cleaner Code**: Simpler model structure
4. **Spatial Ready**: Point geometry is ready for distance calculations and spatial queries
5. **No Rounding Loss**: Direct use of double precision geometry (not rounded to int)

## Data Flow

### Creating EventLocation from DTO
```
LocationDto (Lat: "37", Lng: "-122")
    ↓
ToParsedCoordinates() → (lat: 37, lng: -122)
    ↓
CreateGeometry(37, -122) → Point(-122, 37)
    ↓
EventLocation { ... Geometry: Point(-122, 37) }
```

### Returning EventLocation to DTO
```
EventLocation { Geometry: Point(-122, 37) }
    ↓
ExtractCoordinatesFromGeometry() → (lat: "37", lng: "-122")
    ↓
LocationDto { Coordinates: CoordinatesDto { Lat: "37", Lng: "-122" } }
```

## Database Migration

When you run the migration, it will:
1. ✅ Remove columns: `Location_Coordinates_Lat`, `Location_Coordinates_Lng` from both tables
2. ✅ Add/update column: `Location_Geometry` (geography type) for spatial data
3. ✅ Keep: VenueName, AddressLine, Country, StateOrRegion, City columns

Old migration files will be superseded by the new one.

## Files Modified Summary

| File | Changes | Status |
|------|---------|--------|
| model/Location.cs | Removed EventCoordinates class and property | ✅ Complete |
| mappers/LocationMapper.cs | Extract from Point, removed EventCoordinates | ✅ Complete |
| mappers/GeoRadiusTargetMapper.cs | Removed EventCoordinates instantiation | ✅ Complete |
| data/ApplicationDbContext.cs | Removed Coordinates owned type config | ✅ Complete |

## Next Steps

1. Run `dotnet build` to verify compilation
2. Create new migration (this will supersede previous ones):
   ```powershell
   cd api
   dotnet ef migrations add RemoveCoordinatesUsePointOnly
   ```
3. Apply migration:
   ```powershell
   dotnet ef database update
   ```
4. Test Events and Ads creation with locations
5. Verify coordinate extraction works correctly in API responses

## Notes

- Point coordinates use double precision (accurate to ~10 meters at equator)
- Point order is (longitude, latitude) per WGS84 standard
- Coordinates are extracted on-the-fly for DTOs, not stored
- This is the single source of truth for spatial data going forward

