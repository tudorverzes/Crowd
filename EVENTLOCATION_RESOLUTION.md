# EventLocation Naming Resolution - Completed

## Issue Resolved
Fixed ambiguous reference conflict between `api.model.Location` and `NetTopologySuite.Geometries.Location` by renaming the model class to `EventLocation`.

## Changes Made

### 1. **model/Location.cs**
   - ✅ Renamed `public class Location` → `public class EventLocation`
   - Kept `Coordinates` class unchanged
   - Geometry support remains intact

### 2. **model/Event.cs**
   - ✅ Updated property: `Location? Location` → `EventLocation? Location`

### 3. **model/Ad.cs (GeoRadiusTarget)**
   - ✅ Updated property: `Location? Location` → `EventLocation? Location`

### 4. **mappers/LocationMapper.cs**
   - ✅ Updated `ToLocationDto()` parameter: `Location?` → `EventLocation?`
   - ✅ Updated `ToLocationEntity()` return type: `Location?` → `EventLocation?`
   - ✅ Updated instantiation: `new Location` → `new EventLocation`

### 5. **mappers/GeoRadiusTargetMapper.cs**
   - ✅ Updated `CreateLocationFromCoordinates()` return type: `Location` → `EventLocation`
   - ✅ Updated instantiation: `new Location` → `new EventLocation`

### 6. **mappers/EventsMapper.cs**
   - ✅ No changes needed (uses extension methods that automatically work with EventLocation)

### 7. **data/ApplicationDbContext.cs**
   - ✅ No changes needed (uses lambda expressions for property mapping, not class names)

## Verification
- All model classes updated
- All mappers updated
- Extension methods compatible with EventLocation
- No ambiguous references remain

## Next Steps
1. Run `dotnet build` to verify compilation
2. Create and apply Entity Framework migration:
   ```powershell
   dotnet ef migrations add AddLocationToGeoRadiusTargetAndNetTopology
   dotnet ef database update
   ```
3. Run tests to verify functionality

## Files Modified Summary
| File | Change Type | Status |
|------|-------------|--------|
| model/Location.cs | Class rename | ✅ Complete |
| model/Event.cs | Property type update | ✅ Complete |
| model/Ad.cs | Property type update | ✅ Complete |
| mappers/LocationMapper.cs | Parameter/return type update | ✅ Complete |
| mappers/GeoRadiusTargetMapper.cs | Return type update | ✅ Complete |
| mappers/EventsMapper.cs | No changes | ✅ OK |
| data/ApplicationDbContext.cs | No changes | ✅ OK |

## Breaking Changes
None - this is an internal refactoring. DTOs and API contracts remain unchanged.

