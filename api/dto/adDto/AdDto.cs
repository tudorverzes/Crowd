using api.dto.eventDto;
using api.model;

namespace api.dto.addDto;

public class AdDto
{
	public string Id { get; set; }
	public string Title { get; set; }
	public string Description { get; set; }
	
	public string MediaUrl { get; set; }
	
	public List<string> Keywords { get; set; }
	public List<GeoTargetDto> GeoTargets { get; set; }
	public List<EventTargetDto> SpecificEventTargets { get; set; }
	
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	
	public int? MaxImpressions { get; set; }
	public int ImpressionsCount { get; set; }
	
	public int ApprovalStatus { get; set; }
	public int Status { get; set; }
}

public class GeoTargetDto
{
	public string Country { get; set; }
	public string City { get; set; }
	
	public double Latitude { get; set; }
	public double Longitude { get; set; }
	
	public int RadiusInKm { get; set; }
}

public class EventTargetDto
{
	public int Id { get; set; }
	public string Name { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
}