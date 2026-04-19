using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace api.dto.addDto;

public class CreateAdDt
{
	public string Title { get; set; }
	public string Description { get; set; }
	
	public IFormFile Media { get; set; }
	
	public List<string>? Keywords { get; set; }
	public List<CreateGeoTargetDto>? GeoTargets { get; set; }
	public List<int>? SpecificEventTargetIds { get; set; }
	
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	
	public int? MaxImpressions { get; set; }
	
	public bool IsDraft { get; set; } = true;
}

public class CreateGeoTargetDto
{
	public string Country { get; set; }
	public string City { get; set; }
	
	public double Latitude { get; set; }
	public double Longitude { get; set; }
	
	public int RadiusInKm { get; set; }
}