using System.Collections.Generic;
using System.Threading.Tasks;
using api.data;
using api.dto.eventDto;
using api.mappers;
using api.model;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace api.repository;

public class EventRepository : IEventRepository
{
	private readonly ApplicationDbContext _context;

	public EventRepository(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<List<Event>> GetAllAsync()
	{
		return await _context.Events.ToListAsync();
	}

	public async Task<List<Event>> GetByIdsAsync(List<int> ids)
	{
		return await _context.Events.Where(e => ids.Contains(e.Id)).ToListAsync();
	}

	public async Task<Event?> GetByIdAsync(int id)
	{
		return await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
	}

	public async Task<Event?> GetByUniqueCodeAsync(string uniqueCode)
	{
		return await _context.Events.FirstOrDefaultAsync(e => e.UniqueCode == uniqueCode);
	}

	public async Task<Event?> CreateAsync(Event eventModel)
	{
		await _context.Events.AddAsync(eventModel);
		await _context.SaveChangesAsync();
		return eventModel;
	}

	public async Task<Event?> UpdateAsync(Event eventModel)
	{
		var existingEvent = await _context.Events.FindAsync(eventModel.Id);
		if (existingEvent == null)
		{
			return null;
		}

		foreach (var permission in eventModel.Permissions)
		{
			await _context.Permissions.AddAsync(permission);
		}

		existingEvent.Name = eventModel.Name;
		existingEvent.StartDate = eventModel.StartDate;
		existingEvent.EndDate = eventModel.EndDate;
		existingEvent.Capacity = eventModel.Capacity;
		existingEvent.Overselling = eventModel.Overselling;
		await _context.SaveChangesAsync();
		return existingEvent;
	}

	public async Task<Event?> DeleteAsync(int id)
	{
		var eventModel = await _context.Events
			.Include(ev => ev.Permissions)
			.Include(ev => ev.TicketTypes)
			.Include(ev => ev.Tickets)
			.FirstOrDefaultAsync(e => e.Id == id);
		if (eventModel == null)
		{
			return null;
		}
		
		_context.Tickets.RemoveRange(eventModel.Tickets);
		_context.TicketTypes.RemoveRange(eventModel.TicketTypes);
		_context.Permissions.RemoveRange(eventModel.Permissions);
		_context.Events.Remove(eventModel);
		await _context.SaveChangesAsync();
		return eventModel;
	}

	public async Task<Event?> ChangeScanningStateAsync(int id, bool state)
	{
		var eventModel = await _context.Events
			.FindAsync(id);
		if (eventModel == null)
		{
			return null;
		}
		
		if (eventModel.ScanningState == state)
		{
			return null;
		}
		
		eventModel.ScanningState = state;
		await _context.SaveChangesAsync();
		return eventModel;
	}

	public async Task<List<Event>> GetEventsByGeoLocationAsync(Point location, double radiusInKm, DateTime? fromDate = null, DateTime? toDate = null)
	{
		var radiusInMeters = radiusInKm * 1000;

		var events = await _context.Events
			.Include(e => e.Location)
			.Where(e => e.Location != null && e.Location.Geometry != null)
			.Where(e => e.Location != null && e.Location.Geometry != null && e.Location.Geometry.IsWithinDistance(location, radiusInMeters))
			.Where(e => (fromDate == null || e.EndDate >= fromDate) && (toDate == null || e.StartDate <= toDate))
			.ToListAsync();

		return events;
	}
}