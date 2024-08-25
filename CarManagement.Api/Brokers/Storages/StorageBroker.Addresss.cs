// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Addresss;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Address> Addresss { get; set; }

        public async ValueTask<Address> InsertAddressAsync(Address address) =>
            await InsertAsync(address);

        public IQueryable<Address> SelectAllAddresss() =>
            SelectAll<Address>();

        public async ValueTask<Address> SelectAddressByIdAsync(Guid addressId) =>
            await SelectAsync<Address>(addressId);

        public async ValueTask<Address> DeleteAddressAsync(Address address) =>
            await DeleteAsync(address);

        public async ValueTask<Address> UpdateAddressAsync(Address address) =>
            await UpdateAsync(address);
    }
}