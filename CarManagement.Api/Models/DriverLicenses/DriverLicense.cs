// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Models.DriverLicenses
{
	public class DriverLicense
	{
	    public Guid Id { get; set; }
	    public string FirstName { get; set; } = default!;
	    public string LastName { get; set; } = default!;
	    public string MiddleName { get; set; } = default!;
	    public DateTime BirthDate { get; set; }
	    public Guid AddressId { get; set; }
	    public DateTime ReceivedDate { get; set; }
	    public DateTime ExpirationDate { get; set; }
	    public Guid CategoryId { get; set; }
	    public string GivenLocation { get; set; } = default!;
	    public string Number { get; set; } = default!;
	    public Guid UserId { get; set; }
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}