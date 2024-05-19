using System;
using Microsoft.EntityFrameworkCore;

namespace hub.Shared.Models.Finance;

[PrimaryKey(nameof(Id))]
[Index(nameof(PostingDate))]
public class Transaction
{
	public int Id { get; set; }
	public DateTime PostingDate { get; set; }
	public string Description { get; set; }
	public decimal Amount { get; set; }
	public Category Category { get; set; }
}