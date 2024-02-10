using Microsoft.EntityFrameworkCore;

namespace hub.Shared.Models.Finance;

[PrimaryKey(nameof(Id))]
public class Category
{
	public int Id { get; set; }
	public string Name { get; set; }
	public decimal Budget { get; set; }
}