// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Models.Addresss
{
	public class Address
	{
	    public Guid Id { get; set; }
	    public string Country { get; set; } = default!;
	    public string Region { get; set; } = default!;
	    public string Street { get; set; } = default!;
	    public string HomeNumber { get; set; } = default!;
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}