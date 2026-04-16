using api.repository;
using Microsoft.AspNetCore.Mvc;

namespace api.controller;

[Route("api/ad")]
public class AdController : ControllerBase
{
	private readonly IAdRepository adRepository;
	private readonly IEventRepository _eventRepo;
	private readonly IPermissionRepository _permissionRepo;
	private readonly ITicketTypeRepository _ticketTypeRepo;

	public AdController(IAdRepository adRepository, IEventRepository eventRepo, IPermissionRepository permissionRepo, ITicketTypeRepository ticketTypeRepo)
	{
		this.adRepository = adRepository;
		_eventRepo = eventRepo;
		_permissionRepo = permissionRepo;
		_ticketTypeRepo = ticketTypeRepo;
	}
	
	// Endpoints for recommendation engine
	
}