﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using api.dto.locationDto;
using api.dto.ticketTypeDto;
using api.model;

namespace api.dto.eventDto;

public class CreateEventDto
{
	[Required]
	[MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
	[MaxLength(50, ErrorMessage = "Name must be at most 50 characters long.")]
	public string Name { get; set; } = string.Empty;
	
	[Required]
	public string Description { get; set; } = string.Empty;
	
	[Required]
	[DataType(DataType.Date)]
	public DateTime StartDate { get; set; }
	
	public DateTime EndDate { get; set; }
	
	[Required]
	public int Capacity { get; set; }
	
	[Required]
	public bool Overselling { get; set; }
	
	public bool VisibleForTargetedAds { get; set; } = false;
	
	[Required]
	[MinLength(1, ErrorMessage = "At least one ticket type must be provided.")]
	public List<CreateTicketTypeDto> TicketTypes { get; set; } = [];
	public List<string> SuperAdmins { get; set; } = [];
	public List<string> Admins { get; set; } = [];
	public List<string> Scanners { get; set; } = [];
	public LocationDto? Location { get; set; }
}