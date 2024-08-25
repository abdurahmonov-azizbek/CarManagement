// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Models.Penalties
{
	public class Penalty
	{
	    public Guid Id { get; set; }
	    public string TexPassportNumber { get; set; } = default!;
	    public string CarNumber { get; set; } = default!;
	    public Guid EmployeeId { get; set; }
	    public Guid DriverId { get; set; }
	    public DateTime Date { get; set; }
	    public DateTime PaymentDate { get; set; }
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}