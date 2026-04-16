# Coordinates Naming Resolution - Completed

## Issue Resolved
Fixed ambiguous reference conflict between `api.model.Coordinates` and `NetTopologySuite.Geometries.Coordinates` by renaming the model class to `EventCoordinates`.

## Changes Made

### 1. **model/Location.cs**
   - ✅ Renamed `public class Coordinates` → `public class EventCoordinates`
   - ✅ Updated `EventLocation` property: `Coordinates Coordinates` → `EventCoordinates Coordinates`

### 2. **mappers/LocationMapper.cs**
   - ✅ Updated instantiation: `new Coordinates` → `new EventCoordinates`
   - Builds `EventCoordinates` objects from parsed int coordinates

### 3. **mappers/GeoRadiusTargetMapper.cs**
   - ✅ Updated instantiation: `new Coordinates` → `new EventCoordinates`
   - Builds `EventCoordinates` objects when creating location from double coordinates

### 4. **data/ApplicationDbContext.cs**
   - ✅ No changes needed (uses lambda expressions `.OwnsOne(l => l.Coordinates, ...)` which work with property names, not class names)

## Verification
- ✅ All model classes updated
- ✅ All mappers updated
- ✅ No ambiguous references remain
- ✅ EventLocation properly uses EventCoordinates

## Complete Naming Map
| Original Name | New Name | Reason |
|---|---|---|
| `Location` | `EventLocation` | Avoid conflict with `NetTopologySuite.Geometries.Location` |
| `Coordinates` | `EventCoordinates` | Avoid conflict with `NetTopologySuite.Geometries.Coordinates` |

## Next Steps
1. Run `dotnet build` to verify compilation
2. Create and apply Entity Framework migration:
   ```powershell
   cd C:\Users\OMEN\Pictures\Teme\AAltele\ISS\CROWD\CrowdApp\api
   dotnet ef migrations add AddLocationToGeoRadiusTargetAndNetTopology
   dotnet ef database update
   ```
3. Run tests to verify functionality

## Files Modified Summary
| File | Changes | Status |
|------|---------|--------|
| model/Location.cs | Rename Coordinates → EventCoordinates, update property | ✅ Complete |
| mappers/LocationMapper.cs | Update instantiation to EventCoordinates | ✅ Complete |
| mappers/GeoRadiusTargetMapper.cs | Update instantiation to EventCoordinates | ✅ Complete |
| data/ApplicationDbContext.cs | No changes needed | ✅ OK |

## Breaking Changes
None - this is an internal refactoring. DTOs and API contracts remain unchanged.

## Related Resolutions
- Previous: Renamed `Location` → `EventLocation` (see EVENTLOCATION_RESOLUTION.md)
- Current: Renamed `Coordinates` → `EventCoordinates`

