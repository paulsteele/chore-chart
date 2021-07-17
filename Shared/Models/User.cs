using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace hub.Shared.Models {
	public class User {
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
	}
}
