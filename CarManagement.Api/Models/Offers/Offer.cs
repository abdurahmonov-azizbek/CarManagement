// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Models.Offers
{
	public class Offer
	{
	    public Guid Id { get; set; }
	    public string CarNumber { get; set; } = default!;
	    public Guid TypeId { get; set; }
	    public DateTime Date { get; set; }
	    public DateTime ExpirationDate { get; set; }
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}