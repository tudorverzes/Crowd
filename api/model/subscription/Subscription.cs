using System.ComponentModel.DataAnnotations.Schema;

namespace api.model.subscription;

[Table("Subscriptions")]
public class Subscription
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public double Price { get; set; }
	public int Days { get; set; }
}