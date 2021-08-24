using System;
using Microsoft.AspNetCore.Identity;

namespace hub.Shared.Models.Todo
{
    public class TodoCompletion
    {
        public Guid Id { get; set; }
        public TodoModel TodoModel { get; set; }
        public DateTime Timestamp { get; set; }
        public IdentityUser User { get; set; }
    }
}