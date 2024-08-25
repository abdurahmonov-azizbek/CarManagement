// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.DriverLicenses;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<DriverLicense> InsertDriverLicenseAsync(DriverLicense driverLicense);
        IQueryable<DriverLicense> SelectAllDriverLicenses();
        ValueTask<DriverLicense> SelectDriverLicenseByIdAsync(Guid driverLicenseId);
        ValueTask<DriverLicense> DeleteDriverLicenseAsync(DriverLicense driverLicense);
        ValueTask<DriverLicense> UpdateDriverLicenseAsync(DriverLicense driverLicense);
    }
}