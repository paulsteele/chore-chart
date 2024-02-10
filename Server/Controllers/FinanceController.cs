using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hub.Server.Database;
using hub.Server.NativeBle;
using hub.Shared.Models.Bluetooth;
using hub.Shared.Models.Finance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hub.Server.Controllers;

[ApiController]
[Route("finance")]
public class FinanceController(
	ILogger<FinanceController> logger,
	DatabaseContext database
) : ControllerBase{

	[HttpGet]
	[Route("categories")]
	public IList<Category> GetCategories()
	{
		return database.Categories.ToList();
	}

	[HttpPut]
	[Route("category")]
	public async Task<ActionResult> AddCategory([FromBody] Category category)
	{
		database.Categories.Add(category);
		await database.SaveChangesAsync();
		return Ok();
	}
	
	[HttpPut]
	[Route("category/{id}")]
	public async Task<ActionResult> AddCategory([FromRoute]int id, [FromBody] Category bodyCategory)
	{
		var category = database.Categories.FirstOrDefault(c => c.Id == id);

		if (category == null)
		{
			return NotFound(id);
		}

		category.Name = bodyCategory.Name;
		category.Budget = bodyCategory.Budget;
		category.Color = bodyCategory.Color;
		category.Emoji = bodyCategory.Emoji;
		
		await database.SaveChangesAsync();
		return Ok();
	}
	
	[HttpDelete]
	[Route("category/{id}")]
	public async Task<IActionResult> GetCategories([FromRoute]int id)
	{
		var category = database.Categories.FirstOrDefault(c => c.Id == id);

		if (category == null)
		{
			return NotFound(id);
		}

		database.Categories.Remove(category);
		
		await database.SaveChangesAsync();
		return Ok();
	}
}