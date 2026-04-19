using System.Collections.Generic;
using System.Threading.Tasks;
using api.dto.eventDto;
using api.model;
using NetTopologySuite.Geometries;

namespace api.repository;

public interface IEventRepository
{
	Task<List<Event>> GetAllAsync();
	Task<List<Event>> GetByIdsAsync(List<int> ids);
	Task<Event?> GetByIdAsync(int id);
	Task<Event?> GetByUniqueCodeAsync(string uniqueCode);
	Task<Event?> CreateAsync(Event eventModel);
	Task<Event?> UpdateAsync(Event eventModel);
	Task<Event?> DeleteAsync(int id);
	Task<Event?> ChangeScanningStateAsync(int id, bool state);

	Task<List<Event>> GetEventsByGeoLocationAsync(Point location, double radiusInKm, DateTime? fromDate = null,
		DateTime? toDate = null);
}