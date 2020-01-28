using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppUserManager : UserManager<AppUser>
    {

        public AppUserManager(IUserStore<AppUser> store, IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<AppUser> passwordHasher, IEnumerable<IUserValidator<AppUser>> userValidators, 
            IEnumerable<IPasswordValidator<AppUser>> passwordValidators, ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<AppUser>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, 
                  errors, services, logger)
        {
        }

        /// <summary>
        /// Creates the specified <paramref name="user"/> in the backing store with given password,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the user to hash and store.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public override async Task<IdentityResult> CreateAsync(AppUser user, string password)
        {
            ThrowIfDisposed();
            var passwordStore = Store as IUserPasswordStore<AppUser>;
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }
            var result = await UpdatePasswordHash(passwordStore, user, password);
            if (!result.Succeeded)
            {
                return result;
            }
            return await CreateAsync(user);
        }

        /// <summary>
        /// Updates a user's password hash.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="validatePassword">Whether to validate the password.</param>
        /// <returns>Whether the password has was successfully updated.</returns>
        protected override Task<IdentityResult> UpdatePasswordHash(AppUser user, string newPassword, bool validatePassword)
            => UpdatePasswordHash(Store as IUserPasswordStore<AppUser>, user, newPassword, validatePassword);

        private async Task<IdentityResult> UpdatePasswordHash(IUserPasswordStore<AppUser> passwordStore,
            AppUser user, string newPassword, bool validatePassword = true)
        {
            if (validatePassword)
            {
                var validate = await ValidatePasswordAsync(user, newPassword);
                if (!validate.Succeeded)
                {
                    return validate;
                }
            }
            var hash = newPassword != null ? PasswordHasher.HashPassword(user, newPassword) : null;
            await passwordStore.SetPasswordHashAsync(user, hash, CancellationToken);
            await UpdateSecurityStampInternal(user);
            return IdentityResult.Success;
        }

        // Update the security stamp if the store supports it
        private async Task UpdateSecurityStampInternal(AppUser user)
        {
            if (SupportsUserSecurityStamp)
            {
                await (Store as IUserSecurityStampStore<AppUser>).SetSecurityStampAsync(user, NewSecurityStamp(), CancellationToken);
            }
        }

        private static string NewSecurityStamp()
        {
            byte[] bytes = new byte[20];
#if NETSTANDARD2_0
            _rng.GetBytes(bytes);
#else
            RandomNumberGenerator.Fill(bytes);
#endif
            return "f";
        }
    }
}
