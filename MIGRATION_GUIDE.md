# Entity Framework Migration Instructions

## Overview
After the model and DbContext changes, you need to create and apply a new Entity Framework migration to update your database schema.

## Step-by-Step Migration Process

### Step 1: Create the Migration
Open PowerShell in the api directory and run:

```powershell
cd C:\Users\OMEN\Pictures\Teme\AAltele\ISS\CROWD\CrowdApp\api
dotnet ef migrations add AddLocationToGeoRadiusTargetAndNetTopology
```

**Expected Output:**
```
Build started...
Build completed successfully.
Done. To undo this action, use 'ef migrations remove'
```

This creates a new migration file in the `Migrations` folder with timestamp prefix.

### Step 2: Review the Migration
Open the newly created migration file and verify it contains:

1. **GeoRadiusTarget Location columns** (in the `AdTarget` table):
   - `Location_VenueName` (nvarchar(max))
   - `Location_AddressLine` (nvarchar(max))
   - `Location_Country` (nvarchar(max))
   - `Location_StateOrRegion` (nvarchar(max))
   - `Location_City` (nvarchar(max))
   - `Location_Coordinates_Lat` (int, nullable)
   - `Location_Coordinates_Lng` (int, nullable)
   - `Location_Geometry` (geography, nullable)

2. **Event Location Geometry column** (in the `Events` table):
   - `Location_Geometry` (geography, nullable)

### Step 3: Update Database
Apply the migration to your database:

```powershell
dotnet ef database update
```

**Expected Output:**
```
Build started...
Build completed successfully.
Applying migration '20260415XXXXXX_AddLocationToGeoRadiusTargetAndNetTopology'.
Done.
```

### Step 4: Verify Database Schema
Run SQL Server Management Studio or another SQL client to verify:

#### Check AdTarget table
```sql
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'AdTarget'
ORDER BY ORDINAL_POSITION;
```

You should see columns like:
- `Location_VenueName`
- `Location_Country`
- `Location_City`
- `Location_Coordinates_Lat`
- `Location_Coordinates_Lng`
- `Location_Geometry`

#### Check Events table
```sql
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Events'
WHERE COLUMN_NAME LIKE 'Location_%'
ORDER BY ORDINAL_POSITION;
```

You should see:
- `Location_VenueName`
- `Location_Country`
- `Location_City`
- `Location_Coordinates_Lat`
- `Location_Coordinates_Lng`
- `Location_Geometry`

## Handling Existing Data

### For New Ads (GeoRadiusTarget)
New records will have NULL Location data initially. The Location is now created when:
1. Ad is created with GeoTargets via the API
2. `CreateGeoTargetDto.ToGeoRadiusTarget()` is called
3. Location entity is populated with coordinates and geometry

### For Existing Ads
If you have existing ad records with old `Latitude`/`Longitude` doubles, you'll need a data migration:

```sql
-- Optional: Backfill existing GeoRadiusTarget records with Location data
-- This assumes old double coordinates are still available somewhere

UPDATE AdTarget
SET 
    Location_Country = 'USA',  -- or whatever the original country was
    Location_City = 'Unknown',  -- or map from old data
    Location_Coordinates_Lat = CAST(Latitude AS INT),
    Location_Coordinates_Lng = CAST(Longitude AS INT)
WHERE TargetType = 'GeoRadius'
    AND Location_Country IS NULL
    AND Latitude IS NOT NULL
    AND Longitude IS NOT NULL;
```

**Note:** If you've already dropped the `Latitude` and `Longitude` columns, you cannot recover that data. In that case, existing geo-targeted ads would need to be recreated via the API.

## For New Event Locations
Events created after the migration will automatically have Location.Geometry populated when:
1. Event is created with LocationDto via the API
2. `LocationDto.ToLocationEntity()` is called
3. Location.Geometry is automatically created from coordinates

## Rollback (If Needed)

If you need to revert the migration:

```powershell
dotnet ef migrations remove
dotnet ef database update
```

This will:
1. Remove the last migration file
2. Restore the previous database schema
3. Restore previous model structure

## Troubleshooting

### Migration Fails with "Build error"
Make sure your project compiles first:
```powershell
dotnet build
```

### Geography Type Not Supported Error
Ensure you have:
1. SQL Server 2012 or later (supports Geography type)
2. NetTopologySuite NuGet package installed
3. EntityFrameworkCore.SqlServer NuGet package installed
4. `.UseNetTopologySuite()` configured in Program.cs

### Existing Data Errors
If migration fails due to existing data:
1. Backup your database first
2. Check NULL values in existing Location columns
3. Consider manual data migration script before applying EF migration

## Verification Queries

### Count GeoRadius targets with Location data
```sql
SELECT 
    COUNT(*) as Total,
    COUNT(Location_Country) as WithLocation,
    COUNT(Location_Geometry) as WithGeometry
FROM AdTarget
WHERE TargetType = 'GeoRadius';
```

### View sample GeoRadius target with geometry
```sql
SELECT TOP 1
    Id,
    Location_Country,
    Location_City,
    Location_Coordinates_Lat,
    Location_Coordinates_Lng,
    Location_Geometry.STAsText() as GeometryWKT
FROM AdTarget
WHERE TargetType = 'GeoRadius'
    AND Location_Geometry IS NOT NULL;
```

### Check Event locations with geometry
```sql
SELECT TOP 5
    Id,
    Name,
    Location_City,
    Location_Coordinates_Lat,
    Location_Coordinates_Lng,
    Location_Geometry.STAsText() as GeometryWKT
FROM Events
WHERE Location_Geometry IS NOT NULL;
```

## Next Steps After Migration

1. ✅ Run integration tests to ensure Events and Ads still create properly
2. ✅ Test Event creation with LocationDto
3. ✅ Test Ad creation with GeoTargets (CreateGeoTargetDto)
4. ✅ Verify Location data is persisted correctly
5. ✅ Test spatial distance queries (future implementation)
6. ✅ Monitor application logs for any EF Core navigation loading issues

## Important Notes

- **Always backup your database before running migrations** in production
- **Test migrations in a development/staging environment first**
- **Keep migration files in version control** for team synchronization
- **Document any manual data migrations** for future reference

