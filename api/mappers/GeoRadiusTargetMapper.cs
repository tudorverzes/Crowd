using api.dto.addDto;
using api.model;
using NetTopologySuite.Geometries;

namespace api.mappers;

public static class GeoRadiusTargetMapper
{
	private static readonly GeometryFactory GeometryFactory = new(new PrecisionModel(), 4326); // WGS84 SRID
	
	/// <summary>
	/// Maps a CreateGeoTargetDto to a GeoRadiusTarget entity.
	/// Converts double coordinates from DTO to Location with NetTopology geometry.
	/// </summary>
	public static GeoRadiusTarget ToGeoRadiusTarget(this CreateGeoTargetDto dto)
	{
		return new GeoRadiusTarget
		{
			Country = dto.Country,
			City = dto.City,
			RadiusInKm = dto.RadiusInKm,
			Location = CreateLocationFromCoordinates(dto.Latitude, dto.Longitude, dto.City, dto.Country)
		};
	}

	/// <summary>
	/// Creates an EventLocation entity from latitude/longitude coordinates.
	/// </summary>
	private static EventLocation CreateLocationFromCoordinates(double latitude, double longitude, string city, string country)
	{
		return new EventLocation
		{
			Country = country,
			City = city,
			VenueName = string.Empty,
			AddressLine = string.Empty,
			StateOrRegion = string.Empty,
			Geometry = GeometryFactory.CreatePoint(new Coordinate(longitude, latitude))
		};
	}
}

