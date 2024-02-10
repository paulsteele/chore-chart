using System.Drawing;
using Microsoft.EntityFrameworkCore;

namespace hub.Shared.Models.Finance;

[PrimaryKey(nameof(Id))]
public class Category
{
	public int Id { get; set; }
	public string Name { get; set; }
	public decimal Budget { get; set; }
	public char Emoji { get; set; }
	public Color Color { get; set; }
}