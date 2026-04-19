using api.dto.locationDto;

namespace api.dto.eventDto;

public class RecommendedEventDto
{
	public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public LocationDto? Location { get; set; }
    
    public int RelevanceScore { get; set; }
}