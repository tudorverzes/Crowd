using api.dto.locationDto;
using api.model;
using NetTopologySuite.Geometries;

namespace api.mappers;

public static class LocationMapper
{
	private static readonly GeometryFactory GeometryFactory = new(new PrecisionModel(), 4326); // WGS84 SRID
	
	/// <summary>
	/// Maps an EventLocation entity to a LocationDto for API responses.
	/// Extracts coordinates from Point geometry for DTO serialization.
	/// </summary>
	public static LocationDto? ToLocationDto(this EventLocation? location)
	{
		if (location == null)
			return null;

		var (lat, lng) = ExtractCoordinatesFromGeometry(location.Geometry);

		return new LocationDto
		{
			VenueName = location.VenueName,
			AddressLine = location.AddressLine,
			Country = location.Country,
			StateOrRegion = location.StateOrRegion,
			City = location.City,
			Coordinates = new CoordinatesDto
			{
				Lat = lat,
				Lng = lng
			}
		};
	}

	/// <summary>
	/// Maps a LocationDto from the API request to an EventLocation entity.
	/// Parses string coordinates to nullable integers and creates NetTopology geometry.
	/// </summary>
	public static EventLocation? ToLocationEntity(this LocationDto? locationDto)
	{
		if (locationDto == null)
			return null;

		var (lat, lng) = locationDto.Coordinates?.ToParsedCoordinates() ?? (null, null);

		return new EventLocation
		{
			VenueName = locationDto.VenueName,
			AddressLine = locationDto.AddressLine,
			Country = locationDto.Country,
			StateOrRegion = locationDto.StateOrRegion,
			City = locationDto.City,
			Geometry = CreateGeometry(lat, lng)
		};
	}

	/// <summary>
	/// Creates a NetTopology Point geometry from coordinates.
	/// Note: Point uses (longitude, latitude) order per WGS84 standard.
	/// </summary>
	private static Point? CreateGeometry(double? latitude, double? longitude)
	{
		if (latitude == null || longitude == null)
			return null;
		
		var lat = latitude.Value;
		var lng = longitude.Value;

		return GeometryFactory.CreatePoint(new Coordinate(lng, lat));
	}

	/// <summary>
	/// Extracts latitude and longitude coordinates from a Point geometry.
	/// Returns string representation for DTO serialization.
	/// Note: Point stores (longitude, latitude) order, we convert back to (latitude, longitude).
	/// </summary>
	private static (string lat, string lng) ExtractCoordinatesFromGeometry(Point? geometry)
	{
		if (geometry == null || geometry.IsEmpty)
			return (string.Empty, string.Empty);

		// Point coordinate order is (longitude, latitude) per WGS84
		var latitude = geometry.Coordinate.Y;
		var longitude = geometry.Coordinate.X;

		return (latitude.ToString("F6"), longitude.ToString("F6"));
	}
}

