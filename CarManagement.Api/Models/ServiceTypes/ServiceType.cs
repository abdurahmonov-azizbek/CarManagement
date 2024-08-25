// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

namespace CarManagement.Api.Models.ServiceTypes
{
	public class ServiceType
	{
	    public Guid Id { get; set; }
	    public string Name { get; set; } = default!;
	    public DateTimeOffset CreatedDate { get; set; }
	    public DateTimeOffset UpdatedDate { get; set; }
	}
}