using System.Globalization;

namespace api.dto.locationDto;

public class CoordinatesDto
{
	public string? Lat { get; set; }
	public string? Lng { get; set; }
	
	/// <summary>
	/// Converts string coordinates to nullable integers for database storage.
	/// Returns (null, null) if parsing fails or inputs are null/empty.
	/// </summary>
	public (double? latValue, double? lngValue) ToParsedCoordinates()
	{
		double? latValue = null;
		double? lngValue = null;

		if (!string.IsNullOrWhiteSpace(Lat) &&
		    double.TryParse(Lat, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedLat))
		{
			latValue = parsedLat;
		}

		if (!string.IsNullOrWhiteSpace(Lng) &&
		    double.TryParse(Lng, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedLng))
		{
			lngValue = parsedLng;
		}

		return (latValue, lngValue);
	}
}

public class LocationDto
{
	public string VenueName { get; set; } = string.Empty;
	public string AddressLine { get; set; } = string.Empty;
	public string Country { get; set; } = string.Empty;
	public string StateOrRegion { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	public CoordinatesDto Coordinates { get; set; } = new();
}

