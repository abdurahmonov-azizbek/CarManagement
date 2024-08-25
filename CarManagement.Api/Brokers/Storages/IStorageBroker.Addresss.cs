// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Addresss;

namespace CarManagement.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Address> InsertAddressAsync(Address address);
        IQueryable<Address> SelectAllAddresss();
        ValueTask<Address> SelectAddressByIdAsync(Guid addressId);
        ValueTask<Address> DeleteAddressAsync(Address address);
        ValueTask<Address> UpdateAddressAsync(Address address);
    }
}