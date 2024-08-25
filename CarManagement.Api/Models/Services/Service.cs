// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Models.Services
{
	public class Service
	{
	    public Guid Id { get; set; }
	    public Guid TypeId { get; set; }
	    public string Name { get; set; }
	    public string SertificateNumber { get; set; }
	    public string OwnerFIO { get; set; } = default!;
	    public string Address { get; set; } = default!;
	    public string PhoneNumber { get; set; }
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}