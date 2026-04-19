using api.dto.addDto;
using api.dto.eventDto;
using api.mappers;
using api.model;
using api.repository;
using api.service;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using Point = NetTopologySuite.Geometries.Point;

namespace api.controller;

[Route("api/ad")]
public class AdController : ControllerBase
{
	private readonly IAdRepository _adRepository;
	private readonly IEventRepository _eventRepo;
	private readonly IPermissionRepository _permissionRepo;
	private readonly ITicketTypeRepository _ticketTypeRepo;
	private readonly IWebHostEnvironment _environment;
	
	private readonly IQdrantService _qdrantService;
	private readonly IAiService _aiService;

	public AdController(IAdRepository adRepository, IEventRepository eventRepo, IPermissionRepository permissionRepo, ITicketTypeRepository ticketTypeRepo, IAiService aiService, IQdrantService qdrantService, IWebHostEnvironment environment)
	{
		_adRepository = adRepository;
		_eventRepo = eventRepo;
		_permissionRepo = permissionRepo;
		_ticketTypeRepo = ticketTypeRepo;
		_aiService = aiService;
		_qdrantService = qdrantService;
		_environment = environment;
	}
	
	// Endpoints for recommendation engine
	[HttpPost("events-for-ad")]
	public async Task<ActionResult<List<RecommendedEventDto>>> GetRecommendedEvents([FromBody] RecommendationRequestDto request)
	{
		if (request == null)
		{
			return BadRequest("Request cannot be null");
		}
		
		var vTitle = _aiService.GenerateVector(request.Title);
		var vDescription = _aiService.GenerateVector(request.Description);
        
		var keywordsString = request.Keywords.Count != 0
            ? string.Join(" ", request.Keywords) 
			: "";
		var vKeywords = _aiService.GenerateVector(keywordsString);
		
		var finalAdVector = _aiService.MergeVectorsWithWeights([
			(vTitle, 1.0f),
			(vDescription, 2.0f),
			(vKeywords, 5.0f)
		]);
		
		var qdrantResults = await _qdrantService.SearchEventsForAdAsync(
			vector: finalAdVector,
			startDate: request.StartDate,
			endDate: request.EndDate,
			country: request.Country,
			city: request.City,
			limit: 10
		);

		if (qdrantResults.Count == 0)
		{
			return Ok(new List<RecommendedEventDto>());
		}
		
		var eventIds = qdrantResults.Select(q => (int)q.Id.Num).ToList();
		var recommendedEvents = new List<RecommendedEventDto>();
		
		foreach (var eventId in eventIds)
		{
			var ev = await _eventRepo.GetByIdAsync(eventId);
			if (ev == null) continue;
			
			var recommendedEvent = new RecommendedEventDto
			{
				Id = ev.Id,
				Name = ev.Name,
				Description = ev.Description,
				StartDate = ev.StartDate,
				EndDate = ev.EndDate,
				Location = ev.Location?.ToLocationDto(),
				RelevanceScore = (int)(qdrantResults.First(q => (int)q.Id.Num == eventId).Score * 100)
			};
			
			recommendedEvents.Add(recommendedEvent);
		}
		
		return Ok(recommendedEvents);
	}

	[HttpPost("events-for-location")]
	public async Task<ActionResult<int>> GetEventsForLocations(
		[FromBody] GeoRequestDto request)
	{
		if (request == null)
		{
			return BadRequest("Request cannot be null");
		}
		
		var eventIds = new HashSet<int>();
		foreach (var location in request.Locations)
		{
			var events = await _eventRepo.GetEventsByGeoLocationAsync(new Point(location.Longitude, location.Latitude) { SRID = 4326 }, location.RadiusInKm, request.FromDate);
			foreach (var ev in events)
			{
				eventIds.Add(ev.Id);
			}
		}
		
		return Ok(eventIds.Count);
	}

	[HttpGet("ad-for-event/{eventId}")]
	public async Task<ActionResult<List<ShortAdDto>>> GetAdForEvent(int eventId)
	{
		
		return Ok();
	}
	
	[HttpPost]
	public async Task<ActionResult> CreateAd([FromForm] CreateAdDt createAdDto)
	{
		if (createAdDto == null)
		{
			return BadRequest("Ad data cannot be null");
		}
		
		var userId = HttpContext.User?.FindFirst("userId")?.Value;

		if (userId != null) {
			var targets = new List<AdTarget>();
			
			// Event targets
			if (createAdDto.SpecificEventTargetIds != null)
			{
				foreach (var eventId in createAdDto.SpecificEventTargetIds)
				{
					var ev = await _eventRepo.GetByIdAsync(eventId);
					if (ev == null)
					{
						return BadRequest($"Event with ID {eventId} does not exist");
					}

					if (ev.VisibleForTargetedAds ||
					    (await _permissionRepo.GetOnlyUserPermissionForEventAsync(userId, eventId))?.PermissionType is
					    PermissionType.SuperAdmin or PermissionType.Admin)
					{
						targets.Add(new SpecificEventTarget { EventId = eventId });
					}
					else
					{
						return BadRequest(
							$"Event with ID {eventId} is not visible for targeted ads or you don't have permission to target it");
					}
				}
			}

			// Geo targets
			if (createAdDto.GeoTargets != null)
			{
				foreach (var geoTarget in createAdDto.GeoTargets)
				{
					targets.Add(new GeoRadiusTarget
					{
						Country = geoTarget.Country,
						City = geoTarget.City,
						Geometry = new Point(geoTarget.Longitude, geoTarget.Latitude) { SRID = 4326 },
						RadiusInKm = geoTarget.RadiusInKm
					});
				}
			}

			string mediaUrl;

			if (createAdDto.Media != null && createAdDto.Media.Length > 0)
			{
				var file = createAdDto.Media;

				// Max 5MB
				const long maxFileSize = 5 * 1024 * 1024;
				if (file.Length > maxFileSize)
				{
					return BadRequest("Size of the uploaded file exceeds the 5MB limit.");
				}

				// Extensions
				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
				var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

				if (!allowedExtensions.Contains(fileExtension))
				{
					return BadRequest("Only .jpg, .jpeg, and .png image formats are allowed.");
				}

				// Dimensions
				try
				{
					await using (var stream = file.OpenReadStream())
					{	
						var imageInfo = await Image.IdentifyAsync(stream);

						if (imageInfo == null)
						{
							return BadRequest("Corrupt image or unsupported format.");
						}

						if (imageInfo.Width != 2480 || imageInfo.Height != 956)
						{
							return BadRequest($"Image dimensions must be exactly 2480x956 pixels. Uploaded image is {imageInfo.Width}x{imageInfo.Height} pixels.");
						}
					}
				}
				catch (UnknownImageFormatException)
				{
					return BadRequest("Uploaded file is not a valid image format.");
				}

				var uploadsFolder = Path.Combine(_environment.WebRootPath ?? "wwwroot", "uploads", "ads");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
				var physicalFilePath = Path.Combine(uploadsFolder, uniqueFileName);

				await using (var fileStream = new FileStream(physicalFilePath, FileMode.Create))
				{
					await file.CopyToAsync(fileStream);
				}

				mediaUrl = $"/uploads/ads/{uniqueFileName}";
			}
			else 
			{
				return BadRequest("Media file is required for the ad.");
			}
			
			var ad = new Ad
			{
				Title = createAdDto.Title,
				Description = createAdDto.Description,
				StartDate = createAdDto.StartDate,
				EndDate = createAdDto.EndDate,
				MaxImpressions = createAdDto.MaxImpressions,
				Status = createAdDto.IsDraft ? AdStatus.Draft : AdStatus.Active,
				Targets = targets,
				OwnerId = userId,
				Keywords = createAdDto.Keywords?.Select(k => new AdKeyword { Keyword = k }).ToList() ?? [],
				MediaUrl = mediaUrl
			};
			
			await _adRepository.CreateAsync(ad);
			
			// Compute vector for the new ad and add to Qdrant
			var vTitle = _aiService.GenerateVector(ad.Title);
			var vDescription = _aiService.GenerateVector(ad.Description);
			var keywordsString = ad.Keywords.Count != 0
				? string.Join(" ", ad.Keywords.Select(k => k.Keyword)) 
				: "";
			var vKeywords = _aiService.GenerateVector(keywordsString);
			var finalAdVector = _aiService.MergeVectorsWithWeights([
				(vTitle, 1.0f),
				(vDescription, 2.0f),
				(vKeywords, 5.0f)
			]);	
			
			await _qdrantService.UpsertAdAsync(ad, finalAdVector);

			return Ok();
		}
		
		return Unauthorized();
	}
	
	[HttpGet]
	public async Task<ActionResult<List<ShortAdDto>>> GetMyAds()
	{
		var userId = HttpContext.User?.FindFirst("userId")?.Value;

		if (userId != null) {
			var ads = await _adRepository.GetAllForUserAsync(userId);
			return Ok(ads.Select(a => new ShortAdDto
			{
				Id = a.Id,
				Title = a.Title,
				Description = a.Description,
				MediaUrl = a.MediaUrl,
				ApprovalStatus = (int)a.ApprovalStatus,
				Status = (int)a.Status,
				StartDate = a.StartDate,
				EndDate = a.EndDate,
				MaxImpressions = a.MaxImpressions,
				ImpressionsCount = a.ImpressionsCount
			}).ToList());
		}
		
		return Unauthorized();
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<AdDto>> GetAdById(string id)
	{
		var userId = HttpContext.User?.FindFirst("userId")?.Value;
		
		var ad = await _adRepository.GetByIdAsync(id);
		if (ad == null || ad.OwnerId != userId)
		{
			return Unauthorized();
		}
		
		var targetEventIds = ad.Targets.OfType<SpecificEventTarget>().Select(t => t.EventId).ToList();
		var events = await _eventRepo.GetByIdsAsync(targetEventIds);
		
		return Ok(ad.ToAdDto(events));
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> DeleteAd(string id)
	{
		var userId = HttpContext.User?.FindFirst("userId")?.Value;
		
		var ad = await _adRepository.GetByIdAsync(id);
		if (ad == null || ad.OwnerId != userId)
		{
			return Unauthorized();
		}
		
		await _adRepository.DeleteAsync(id);
		
		return Ok();
	}
	
	[HttpPut("{id}/{status:int}")]
	public async Task<ActionResult> UpdateAdStatus(string id, int status)
	{
		var userId = HttpContext.User?.FindFirst("userId")?.Value;
		
		var ad = await _adRepository.GetByIdAsync(id);
		if (ad == null || ad.OwnerId != userId)
		{
			return Unauthorized();
		}

		if (!Enum.IsDefined(typeof(AdStatus), status))
		{
			return BadRequest("Invalid status value");
		}
		
		await _adRepository.ChangeAdStatusAsync(ad.Id, (AdStatus)status);
		
		return Ok();
	}
}