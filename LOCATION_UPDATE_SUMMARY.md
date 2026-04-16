# Backend API Update for Location Structure

## Overview
Updated the CrowdApp backend API to support the new Location structure with nested coordinates as required by the frontend React application. All changes follow EF Core best practices using owned types for clean database mapping.

---

## Files Created/Updated

### 1. **Model - Location.cs (NEW)**
**Path:** `api/model/Location.cs`

Created owned types for Location and Coordinates:
- `Coordinates` (owned type): Contains nullable `double?` Lat and Lng properties
- `Location` (owned type): Contains venue and address information, plus nested Coordinates object

Using EF Core's `[Owned]` attribute ensures these are stored as part of the Events table with prefixed columns.

### 2. **Model - Event.cs (UPDATED)**
**Path:** `api/model/Event.cs`

Added:
```csharp
public Location? Location { get; set; }
```

The Location property is nullable to support events without location data.

### 3. **DTO - LocationDto.cs (NEW)**
**Path:** `api/dto/locationDto/LocationDto.cs`

Created matching DTO classes:
- `CoordinatesDto`: Contains string Lat and Lng (from frontend), includes `ToParsedCoordinates()` method to safely parse strings to nullable doubles
- `LocationDto`: Mirrors the Location entity structure

### 4. **DTO Updates (EventDto, ShortEventDto, FullEventDto, CreateEventDto, UpdateEventDto)**

All event DTOs now include:
```csharp
public LocationDto? Location { get; set; }
```

This ensures the frontend's nested Location structure is properly reflected in all API responses and request payloads.

### 5. **Mapper - LocationMapper.cs (NEW)**
**Path:** `api/mappers/LocationMapper.cs`

Created extension methods for bidirectional conversion:
- `ToLocationDto(Location?)`: Converts entity to DTO, handling null gracefully
- `ToLocationEntity(LocationDto?)`: Converts DTO to entity, parsing string coordinates to nullable doubles

The parser safely handles:
- Null/empty coordinate strings
- Parse failures (returns null instead of throwing)
- Proper double precision storage

### 6. **Mapper - EventsMapper.cs (UPDATED)**
**Path:** `api/mappers/EventsMapper.cs`

Updated all mapping methods to include Location:
- `ToShortEventDto()`: Maps Location
- `ToEventDto()` (both overloads): Maps Location
- `ToFullEventDto()`: Maps Location
- `FromCreateEventDto()`: Converts incoming Location to entity

### 7. **Data Context - ApplicationDbContext.cs (UPDATED)**
**Path:** `api/data/ApplicationDbContext.cs`

Added Fluent API configuration in `OnModelCreating()`:
```csharp
builder.Entity<Event>()
    .OwnsOne(e => e.Location, locationBuilder =>
    {
        locationBuilder.Property(l => l.VenueName)
            .HasColumnName("Location_VenueName");
        locationBuilder.Property(l => l.AddressLine)
            .HasColumnName("Location_AddressLine");
        locationBuilder.Property(l => l.Country)
            .HasColumnName("Location_Country");
        locationBuilder.Property(l => l.StateOrRegion)
            .HasColumnName("Location_StateOrRegion");
        locationBuilder.Property(l => l.City)
            .HasColumnName("Location_City");
        
        locationBuilder.OwnsOne(l => l.Coordinates, coordinatesBuilder =>
        {
            coordinatesBuilder.Property(c => c.Lat)
                .HasColumnName("Location_Coordinates_Lat");
            coordinatesBuilder.Property(c => c.Lng)
                .HasColumnName("Location_Coordinates_Lng");
        });
    });
```

This configuration:
- Maps Location as an owned type within Events table
- Creates properly prefixed columns: `Location_VenueName`, `Location_AddressLine`, etc.
- Handles nested Coordinates with `Location_Coordinates_Lat` and `Location_Coordinates_Lng`
- Supports nullable double values for coordinates

---

## Database Schema Impact

When you generate a migration, the Events table will have these new columns:
- `Location_VenueName` (nvarchar)
- `Location_AddressLine` (nvarchar)
- `Location_Country` (nvarchar)
- `Location_StateOrRegion` (nvarchar)
- `Location_City` (nvarchar)
- `Location_Coordinates_Lat` (float/double, nullable)
- `Location_Coordinates_Lng` (float/double, nullable)

All columns are nullable in the Events table to match the `Location?` nullable property.

---

## Key Design Decisions

1. **Nullable Location Property**: Events can be created without location data, allowing flexibility
2. **String Coordinates in DTO**: Frontend sends coordinates as strings; backend safely parses to double?
3. **Owned Types**: Using EF Core's owned types keeps Location logic encapsulated without separate table
4. **Safe Parsing**: The `ToParsedCoordinates()` method returns null on parse failure, preventing exceptions
5. **No Data Migration**: Since the database is empty, no migration scripts needed for legacy data

---

## Next Steps

1. Generate a new EF Core migration:
   ```bash
   dotnet ef migrations add AddLocationToEvents
   ```

2. Update the database:
   ```bash
   dotnet ef database update
   ```

3. Test the API with sample Location data:
   ```json
   {
     "name": "Tech Conference 2024",
     "startDate": "2024-06-01",
     "endDate": "2024-06-03",
     "capacity": 500,
     "overselling": false,
     "location": {
       "venueName": "Convention Center",
       "addressLine": "123 Main Street",
       "country": "United States",
       "stateOrRegion": "California",
       "city": "San Francisco",
       "coordinates": {
         "lat": "37.7749",
         "lng": "-122.4194"
       }
     },
     "ticketTypes": [ /* ... */ ],
     "superAdmins": [ /* ... */ ]
   }
   ```

---

## Backward Compatibility

Since the database was empty and no existing records need to be preserved:
- No data migration required
- Fresh schema includes all new columns
- All event-related endpoints will support Location out of the box

