// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Addresss;

namespace CarManagement.Api.Services.Foundations.Addresss
{
    public interface IAddressService  
    {
        /// <exception cref="Models.Addresss.Exceptions.AddressValidationException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressDependencyValidationException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressDependencyException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressServiceException"></exception>
        ValueTask<Address> AddAddressAsync(Address address);

        /// <exception cref="Models.Addresss.Exceptions.AddressDependencyException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressServiceException"></exception>
        IQueryable<Address> RetrieveAllAddresss();

        /// <exception cref="Models.Addresss.Exceptions.AddressDependencyException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressServiceException"></exception>
        ValueTask<Address> RetrieveAddressByIdAsync(Guid addressId);

        /// <exception cref="Models.Addresss.Exceptions.AddressValidationException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressDependencyValidationException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressDependencyException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressServiceException"></exception>
        ValueTask<Address> ModifyAddressAsync(Address address);

        /// <exception cref="Models.Addresss.Exceptions.AddressDependencyValidationException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressDependencyException"></exception>
        /// <exception cref="Models.Addresss.Exceptions.AddressServiceException"></exception>
        ValueTask<Address> RemoveAddressByIdAsync(Guid addressId);
    }
}