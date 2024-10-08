// --------------------------------------------------------
// Copyright (c) Coalition of Good-Hearted Engineers
// Developed by EgoDevs
//  --------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using CarManagement.Api.Brokers.DateTimes;
using CarManagement.Api.Brokers.Loggings;
using CarManagement.Api.Brokers.Storages;
using CarManagement.Api.Models.Users;
using CarManagement.Api.Services.Foundations.Security;

namespace CarManagement.Api.Services.Foundations.Users
{
    public partial class UserService : IUserService
    {

        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;
        private readonly IPasswordHasherService passwordHasherService;

        public UserService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker,
            IPasswordHasherService passwordHasherService)

        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
            this.passwordHasherService = passwordHasherService;
        }

        public ValueTask<User> AddUserAsync(User user) =>
        TryCatch(async () =>
        {
            ValidateUserOnAdd(user);

            user.Password = passwordHasherService.HashPassword(user.Password);

            return await this.storageBroker.InsertUserAsync(user);
        });

        public IQueryable<User> RetrieveAllUsers() =>
            TryCatch(() => this.storageBroker.SelectAllUsers());

        public ValueTask<User> RetrieveUserByIdAsync(Guid userId) =>
           TryCatch(async () =>
           {
               ValidateUserId(userId);

               User maybeUser =
                   await storageBroker.SelectUserByIdAsync(userId);

               ValidateStorageUser(maybeUser, userId);

               return maybeUser;
           });

        public ValueTask<User> ModifyUserAsync(User user) =>
            TryCatch(async () =>
            {
                ValidateUserOnModify(user);

                User maybeUser =
                    await this.storageBroker.SelectUserByIdAsync(user.Id);

                ValidateAgainstStorageUserOnModify(inputUser: user, storageUser: maybeUser);

                user.Password = this.passwordHasherService.HashPassword(user.Password);

                return await this.storageBroker.UpdateUserAsync(user);
            });

        public ValueTask<User> RemoveUserByIdAsync(Guid userId) =>
           TryCatch(async () =>
           {
               ValidateUserId(userId);

               User maybeUser =
                   await this.storageBroker.SelectUserByIdAsync(userId);

               ValidateStorageUser(maybeUser, userId);

               return await this.storageBroker.DeleteUserAsync(maybeUser);
           });
    }
}