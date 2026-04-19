using api.model;

namespace api.dto.addDto;

public class ShortAdDto
{
	public string Id { get; set; } = Guid.NewGuid().ToString();
	
	public string Title { get; set; }
	public string Description { get; set; }
	
	public string MediaUrl { get; set; }
	public int ApprovalStatus { get; set; } = (int)AdApprovalStatus.Pending;
	public int Status { get; set; } = (int)AdStatus.Draft;
	
	public DateTime StartDate { get; set; }
	public DateTime EndDate { get; set; }
	
	public int? MaxImpressions { get; set; } 
	public int ImpressionsCount { get; set; } = 0;
}