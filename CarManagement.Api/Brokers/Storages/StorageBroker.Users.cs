// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace CarManagement.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<User> Users { get; set; }

        public async ValueTask<User> InsertUserAsync(User user) =>
            await InsertAsync(user);

        public IQueryable<User> SelectAllUsers() =>
            SelectAll<User>();

        public async ValueTask<User> SelectUserByIdAsync(Guid userId) =>
            await SelectAsync<User>(userId);

        public async ValueTask<User> DeleteUserAsync(User user) =>
            await DeleteAsync(user);

        public async ValueTask<User> UpdateUserAsync(User user) =>
            await UpdateAsync(user);
    }
}