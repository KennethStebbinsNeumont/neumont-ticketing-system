using Microsoft.AspNetCore.Identity;
using Neumont_Ticketing_System.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppUserStore : IUserStore<AppUser>, IUserPasswordStore<AppUser>, 
        IUserSecurityStampStore<AppUser>, IUserEmailStore<AppUser>, IQueryableUserStore<AppUser>, 
        IUserLockoutStore<AppUser>
    {

        private readonly AppIdentityStorageService _storageService;

        public AppUserStore(AppIdentityStorageService storageService)
        {
            _storageService = storageService;
        }

        public Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task<IdentityResult>(() => {
                var result = new IdentityResult();
                _storageService.CreateUser(user);
                return result;
            });
        }

        public Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task<IdentityResult>(() => {
                var result = new IdentityResult();
                _storageService.RemoveUser(user);
                return result;
            });
        }

        public void Dispose()
        {
        }

        public Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (userId == null) throw new ArgumentNullException(nameof(userId));


            return new Task<AppUser>(() => {
                var list = _storageService.GetUsers(user => user.Id == userId);
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            });
        }

        public Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (normalizedUserName == null) throw new ArgumentNullException(nameof(normalizedUserName));


            return new Task<AppUser>(() => {
                var list = _storageService.GetUsers(user => user.Username == normalizedUserName);
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            });
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task<string>(() => {
                return user.Username;
            });
        }

        public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task<string>(() => {
                return user.Password;
            });
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task<string>(() => {
                return user.Id;
            });
        }

        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task<string>(() => {
                return user.Username;
            });
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task<bool>(() => {
                return user.Password != null && user.Password != "";
            });
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task(() => {
                user.Username = normalizedName;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task(() => {
                user.Password = passwordHash;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task(() => {
                user.Username = userName;
                _storageService.UpdateUser(user);
            });
        }

        public Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return new Task<IdentityResult>(() => {
                var result = new IdentityResult();
                _storageService.UpdateUser(user);
                return result;
            });
        }
    }
}
