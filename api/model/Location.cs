﻿using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace api.model;

[Owned]
public class EventLocation
{
	public string VenueName { get; set; } = string.Empty;
	public string AddressLine { get; set; } = string.Empty;
	public string Country { get; set; } = string.Empty;
	public string StateOrRegion { get; set; } = string.Empty;
	public string City { get; set; } = string.Empty;
	
	/// <summary>
	/// NetTopology point geometry for spatial queries (longitude, latitude order per WGS84 standard)
	/// </summary>
	public Point? Geometry { get; set; }
}

