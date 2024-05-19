using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hub.Server.Database;
using hub.Shared.Models.Finance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
	public ActionResult<IList<Category>> GetCategories()
	{
		return Ok(database.Categories.OrderBy(c => c.Order).ToList());
	}

	[HttpPut]
	[Route("category")]
	public async Task<ActionResult<Category>> AddCategory([FromBody] Category category)
	{
		database.Categories.Add(category);
		await database.SaveChangesAsync();
		return Ok(category);
	}
	
	[HttpGet]
	[Route("category/{id:int}")]
	public ActionResult GetCategory([FromRoute]int id)
	{
		var category = database.Categories.FirstOrDefault(c => c.Id == id);

		if (category == null)
		{
			return NotFound(id);
		}

		return Ok(category);
	}
	[HttpPut]
	[Route("category/{id:int}")]
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
	[Route("category/{id:int}")]
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
	
	[HttpGet]
	[Route("transactions")]
	public ActionResult<IList<Category>> GetTransactions([FromQuery] int? month, [FromQuery] int? year, [FromQuery] bool? onlyUncategorized)
	{
		IQueryable<Transaction> query = database.Transactions.OrderByDescending(c => c.PostingDate);

		if (month.HasValue && year.HasValue)
		{
			query = query.Where(t =>
				t.PostingDate.Year == year &&
				t.PostingDate.Month == month 
			);
		}

		if (onlyUncategorized.HasValue && onlyUncategorized.Value)
		{
			query = query.Where(t => t.Category == null);
		}
		else
		{
			query = query.Include(t => t.Category);
		}
		
		return Ok(query);
	}

	[HttpPut]
	[Route("transaction/{id:int}")]
	public async Task<ActionResult> SetTransactionCategory([FromRoute]int id, [FromBody] Category categoryBody)
	{
		var transaction = database.Transactions.FirstOrDefault(c => c.Id == id);
		var category = database.Categories.FirstOrDefault(c => c.Id == categoryBody.Id);

		if (transaction == null)
		{
			return NotFound();
		}

		transaction.Category = category;
		
		await database.SaveChangesAsync();
		return Ok();
	}
	
	[HttpGet]
	[Route("balance")]
	public ActionResult<decimal> GetBalance()
	{
		var latestTransaction = database.Transactions.OrderByDescending(c => c.PostingDate).FirstOrDefault();
		
		return Ok(latestTransaction?.Balance ?? 0m);
	}
	
	[HttpPost]
	[Route("import")]
	public async Task<ActionResult> Import([FromBody] List<string> fileContents)
	{
		var parsed = fileContents.Select(Transaction.TryParse).Where(t => t != null).ToList();

		if (parsed.Count == 0)
		{
			return NoContent();
		}

		var addable = parsed
			.Where(transaction => !database.Transactions
				.Any(t =>
					t.PostingDate.Equals(transaction.PostingDate) && t.Description.Equals(transaction.Description)
				)
			)
			.Reverse();
		
		database.Transactions.AddRange(addable);
		await database.SaveChangesAsync();

		return Ok();
	}
}