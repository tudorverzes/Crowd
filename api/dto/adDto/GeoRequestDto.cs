namespace api.dto.addDto;

public class GeoRequestDto
{
	public required List<LocationRequestDto> Locations { get; set; }
	public required DateTime FromDate { get; set; }
}

public class LocationRequestDto
{
	public double Latitude { get; set; }
	public double Longitude { get; set; }
	public int RadiusInKm { get; set; }
}