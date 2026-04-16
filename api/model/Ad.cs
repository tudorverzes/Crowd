using System;
using System.Collections.Generic;

namespace api.model;

public enum AdStatus
{
	Draft = 0,
	Active = 1,
	Paused = 2,
}

public class Ad
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	
	public string Title { get; set; }
	public string Description { get; set; }

	public string OwnerId { get; set; }
	public AppUser Owner { get; set; }

	public string MediaUrl { get; set; }

	public List<AdTarget> Targets { get; set; } = [];
	
	public List<AdKeyword> Keywords { get; set; } = [];

	public bool IsApproved { get; set; } = false;
	public AdStatus Status { get; set; } = AdStatus.Draft;

	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
        
	public int? MaxImpressions { get; set; } 
        
	public int ImpressionsCount { get; set; } = 0;
}

public abstract class AdTarget
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
        
	public string AdId { get; set; }
	public Ad? Ad { get; set; }
}

public class GeoRadiusTarget : AdTarget
{
	public string Country { get; set; }
	public string City { get; set; }
	
	public EventLocation? Location { get; set; }
        
	public int RadiusInKm { get; set; }
}

public class SpecificEventTarget : AdTarget
{
	public int EventId { get; set; }
	public Event? Event { get; set; }
}

public class AdKeyword
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
		
	public string AdId { get; set; }
	public Ad? Ad { get; set; }
		
	public string Keyword { get; set; }
}