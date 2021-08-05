using System;
using System.Collections.Generic;

namespace hub.Client.ViewModels.Todo
{
    public interface ITodoViewModel
    {
        List<Shared.Models.Todo.Todo> Todos { get; }

    }
    
    public class TodoViewModel : ITodoViewModel
    {
        public TodoViewModel()
        {
            Todos = new List<Shared.Models.Todo.Todo>()
            {
                new Shared.Models.Todo.Todo() {Title = "Code", ScheduledTime = new DateTime(2021, 4, 6)},
                new Shared.Models.Todo.Todo() {Title = "Code Some", ScheduledTime = new DateTime(2021, 5, 6)},
                new Shared.Models.Todo.Todo() {Title = "Code More", ScheduledTime = new DateTime(2021, 6, 6)}
            };
        }
        
        public List<Shared.Models.Todo.Todo> Todos { get; }
    }
}