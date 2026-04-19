using api.dto.addDto;
using api.model;

namespace api.mappers;

public static class AdMapper
{
	public static ShortAdminAdDto ToShortAdminAdDto(this Ad ad)
	{
		return new ShortAdminAdDto
		{
			Id = ad.Id,
			Title = ad.Title,
			Description = ad.Description,
			MediaUrl = ad.MediaUrl,
			ApprovalStatus = (int)ad.ApprovalStatus,
			Status = (int)ad.Status,
			StartDate = ad.StartDate,
			EndDate = ad.EndDate
		};
	}

	public static AdDto ToAdDto(this Ad ad, List<Event> relatedEvents)
	{
		return new AdDto
		{
			Id = ad.Id,
			Title = ad.Title,
			Description = ad.Description,
			MediaUrl = ad.MediaUrl,
			ApprovalStatus = (int)ad.ApprovalStatus,
			Status = (int)ad.Status,
			StartDate = ad.StartDate,
			EndDate = ad.EndDate,
			MaxImpressions = ad.MaxImpressions,
			ImpressionsCount = ad.ImpressionsCount,
			Keywords = ad.Keywords.Select(k => k.Keyword).ToList(),
			GeoTargets = ad.Targets.OfType<GeoRadiusTarget>().Select(t => new GeoTargetDto
			{
				Country = t.Country,
				City = t.City,
				Latitude = t.Geometry?.Y ?? 0,
				Longitude = t.Geometry?.X ?? 0,
				RadiusInKm = t.RadiusInKm
			}).ToList(),
			SpecificEventTargets = ad.Targets.OfType<SpecificEventTarget>().Select(t => {
				var ev = relatedEvents.FirstOrDefault(e => e.Id == t.EventId);
				return new EventTargetDto
				{
					Id = t.EventId,
					Name = ev?.Name ?? "Unknown Event",
					StartDate = ev?.StartDate ?? DateTime.MinValue,
					EndDate = ev?.EndDate ?? DateTime.MinValue,
				};
			}).ToList()
		};
	}
}