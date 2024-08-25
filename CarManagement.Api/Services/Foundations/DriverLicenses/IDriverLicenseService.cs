// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.DriverLicenses;

namespace CarManagement.Api.Services.Foundations.DriverLicenses
{
    public interface IDriverLicenseService  
    {
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseValidationException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseDependencyValidationException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseDependencyException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseServiceException"></exception>
        ValueTask<DriverLicense> AddDriverLicenseAsync(DriverLicense driverLicense);

        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseDependencyException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseServiceException"></exception>
        IQueryable<DriverLicense> RetrieveAllDriverLicenses();

        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseDependencyException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseServiceException"></exception>
        ValueTask<DriverLicense> RetrieveDriverLicenseByIdAsync(Guid driverLicenseId);

        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseValidationException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseDependencyValidationException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseDependencyException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseServiceException"></exception>
        ValueTask<DriverLicense> ModifyDriverLicenseAsync(DriverLicense driverLicense);

        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseDependencyValidationException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseDependencyException"></exception>
        /// <exception cref="Models.DriverLicenses.Exceptions.DriverLicenseServiceException"></exception>
        ValueTask<DriverLicense> RemoveDriverLicenseByIdAsync(Guid driverLicenseId);
    }
}