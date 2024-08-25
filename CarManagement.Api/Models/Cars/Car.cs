// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Models.Cars
{
	public class Car
	{
	    public Guid Id { get; set; }
	    public Guid TypeId { get; set; }
	    public Guid ModelId { get; set; }
	    public string Color { get; set; } = default!;
	    public int Year { get; set; }
	    public int LambNumber { get; set; }
	    public int EngineNumber { get; set; }
	    public int HorsePower { get; set; }
	    public string Number { get; set; } = default!;
	    public string TexPassportNumber { get; set; } = default!;
	    public Guid UserId { get; set; }
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}