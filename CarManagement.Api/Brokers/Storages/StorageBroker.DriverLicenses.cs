// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.DriverLicenses;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<DriverLicense> DriverLicenses { get; set; }

        public async ValueTask<DriverLicense> InsertDriverLicenseAsync(DriverLicense driverLicense) =>
            await InsertAsync(driverLicense);

        public IQueryable<DriverLicense> SelectAllDriverLicenses() =>
            SelectAll<DriverLicense>();

        public async ValueTask<DriverLicense> SelectDriverLicenseByIdAsync(Guid driverLicenseId) =>
            await SelectAsync<DriverLicense>(driverLicenseId);

        public async ValueTask<DriverLicense> DeleteDriverLicenseAsync(DriverLicense driverLicense) =>
            await DeleteAsync(driverLicense);

        public async ValueTask<DriverLicense> UpdateDriverLicenseAsync(DriverLicense driverLicense) =>
            await UpdateAsync(driverLicense);
    }
}