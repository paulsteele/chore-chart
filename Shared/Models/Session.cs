using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace hub.Shared.Models {
	public class Session {

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public DateTime ExpirationTime { get; set; }
	}
}
