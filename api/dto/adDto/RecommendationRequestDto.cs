namespace api.dto.addDto;

public class RecommendationRequestDto
{
	public string Title { get; set; }
	public string Description { get; set; }
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	public List<string> Keywords { get; set; }
	public string? Country { get; set; } = null;
	public string? City { get; set; } = null;
}