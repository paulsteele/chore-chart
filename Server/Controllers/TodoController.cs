using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using hub.Server.Database;
using hub.Shared.Models.Todo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace hub.Server.Controllers
{
	[ApiController]
	[Route("todos")]
    public class TodoController : ControllerBase
    {
	    private readonly UserManager<IdentityUser> _userManager;
	    private readonly IDb _db;

	    public TodoController(
		    UserManager<IdentityUser> userManager,
		    IDb db
		)
	    {
		    _userManager = userManager;
		    _db = db;
	    }
	    
	    [HttpGet]
	    public async Task<IActionResult> GetAllTodos()
	    {
		    var user = await _userManager.GetUserAsync(HttpContext.User);

		    var todos = _db.DatabaseContext.Todos.Where(t => t.User == user).ToArray();

		    var completions = new List<TodoCompletion>();
		    foreach (var todo in todos)
		    {
			    completions.AddRange(_db.DatabaseContext.TodosCompletions.Where(c => c.User == user && c.TodoModel == todo));
		    }

		    var response = todos.Select(t =>
		    {
			    var completion = _db.DatabaseContext.TodosCompletions
				    .FirstOrDefault(c => c.User == user && c.TodoModel == t);

			    return new TodoCompletionPairing {TodoModel = t, TodoCompletion = completion};
		    }).ToArray();

		    return Ok(response);
	    }
	    
	    [HttpPut]
	    public async Task<IActionResult> UpdateTodo(TodoModel todoModel)
	    {
		    var user = await _userManager.GetUserAsync(HttpContext.User);
		    
		    // sanitization
		    todoModel.User = user;

		    var savedTodo = _db.DatabaseContext.Todos.Update(todoModel);

		    await _db.DatabaseContext.SaveChangesAsync();
		    return Ok(savedTodo.Entity);
	    }
	    
	    [HttpDelete]
	    [Route("{guid:Guid}")]
	    public async Task<IActionResult> DeleteTodo(Guid guid)
	    {
		    var user = await _userManager.GetUserAsync(HttpContext.User);
		    
		    // sanitization
		    var model = await _db.DatabaseContext.Todos.FindAsync(guid);
		    if (model == null || model.User != user)
		    {
			    return NotFound();
		    }
		    
		    _db.DatabaseContext.Todos.Remove(model);
		    _db.DatabaseContext.TodosCompletions.RemoveRange(_db.DatabaseContext.TodosCompletions.Where(c => c.TodoModel == model));

			await _db.DatabaseContext.SaveChangesAsync();
			return Ok();
	    }
	    
	    [HttpPut]
	    [Route("completions")]
	    public async Task<IActionResult> UpdateTodoCompletion(TodoCompletion todoCompletion)
	    {
		    var user = await _userManager.GetUserAsync(HttpContext.User);
		    
		    // sanitization
		    todoCompletion.User = user;
		    
		    var todo = await _db.DatabaseContext.Todos.FindAsync(todoCompletion.TodoModel.Id);
		    todoCompletion.TodoModel = todo;

		    var savedTodo = _db.DatabaseContext.TodosCompletions.Update(todoCompletion);

		    await _db.DatabaseContext.SaveChangesAsync();
		    return Ok(savedTodo.Entity);
	    }
	    
	    [HttpDelete]
	    [Route("completions/{guid:Guid}")]
	    public async Task<IActionResult> DeleteTodoCompletion(Guid guid)
	    {
		    var user = await _userManager.GetUserAsync(HttpContext.User);
		    
		    // sanitization
		    var model = await _db.DatabaseContext.TodosCompletions.FindAsync(guid);
		    if (model == null || model.User != user)
		    {
			    return NotFound();
		    }
		    
		    _db.DatabaseContext.TodosCompletions.Remove(model);

			await _db.DatabaseContext.SaveChangesAsync();
			return Ok();
	    }
    }
}