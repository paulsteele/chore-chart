using System;
using Microsoft.AspNetCore.Identity;

namespace hub.Shared.Models.Todo
{
    public class TodoModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime ScheduledTime { get; set; }
        public IdentityUser User { get; set; }
    }
}