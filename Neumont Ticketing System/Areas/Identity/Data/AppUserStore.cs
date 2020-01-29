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
        IUserLockoutStore<AppUser>, IUserRoleStore<AppUser>
    {
        private readonly AppIdentityStorageService _storageService;

        IQueryable<AppUser> IQueryableUserStore<AppUser>.Users => _storageService.GetUsers().AsQueryable();

        public AppUserStore(AppIdentityStorageService storageService)
        {
            _storageService = storageService;
        }

        public Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<IdentityResult>(() => {
                var result = new IdentityResult();
                _storageService.CreateUser(user);
                return result;
            });
        }

        public Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<IdentityResult>(() => {
                var result = new IdentityResult();
                _storageService.RemoveUser(user);
                return result;
            });
        }

        public void Dispose()
        {
        }

        public Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (normalizedEmail == null) throw new ArgumentNullException(nameof(normalizedEmail));


            return Task.Run<AppUser>(() => {
                var list = _storageService.GetUsers(user => user.NormalizedEmail == normalizedEmail);
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            });
        }

        public Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (userId == null) throw new ArgumentNullException(nameof(userId));


            return Task.Run<AppUser>(() => {
                var allUsers = _storageService.GetUsers();
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


            return Task.Run<AppUser>(() => {
                var list = _storageService.GetUsers(user => user.Username == normalizedUserName);
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            });
        }

        public Task<int> GetAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<int>(() => {
                return user.FailedLoginAttempts;
            });
        }

        public Task<string> GetEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<string>(() => {
                return user.Email;
            });
        }

        public Task<bool> GetEmailConfirmedAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<bool>(() => {
                return user.EmailConfirmed;
            });
        }

        public Task<bool> GetLockoutEnabledAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<bool>(() => {
                return user.LockoutEnabled;
            });
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<DateTimeOffset?>(() => {
                return user.LockedOutUntil;
            });
        }

        public Task<string> GetNormalizedEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<string>(() => {
                return user.NormalizedEmail;
            });
        }

        public Task<string> GetNormalizedUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<string>(() => {
                return user.Username;
            });
        }

        public Task<string> GetPasswordHashAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => user.PasswordHash);
        }

        public Task<string> GetSecurityStampAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => user.SecurityStamp);
        }

        public Task<string> GetUserIdAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => user.Id);
        }

        public Task<string> GetUserNameAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => {
                return user.Username;
            });
        }

        public Task<bool> HasPasswordAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => {
                return user.PasswordHash != null && user.PasswordHash != "";
            });
        }

        public Task<int> IncrementAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => {
                user.FailedLoginAttempts++;
                _storageService.UpdateUser(user);
                return user.FailedLoginAttempts;
            });
        }

        public Task ResetAccessFailedCountAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => {
                user.FailedLoginAttempts = 0;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetEmailAsync(AppUser user, string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (email == null) throw new ArgumentNullException(nameof(email));


            return Task.Run(() => {
                user.Email = email;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetEmailConfirmedAsync(AppUser user, bool confirmed, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => {
                user.EmailConfirmed = true;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetLockoutEnabledAsync(AppUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => {
                user.LockoutEnabled = true;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetLockoutEndDateAsync(AppUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (lockoutEnd == null) throw new ArgumentNullException(nameof(lockoutEnd));



            return Task.Run(() => {
                if (lockoutEnd.HasValue)
                    user.LockedOutUntil = lockoutEnd.Value.UtcDateTime;
                else
                    user.LockedOutUntil = null;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetNormalizedEmailAsync(AppUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (normalizedEmail == null) throw new ArgumentNullException(nameof(normalizedEmail));


            return Task.Run(() => {
                user.NormalizedEmail = normalizedEmail;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetNormalizedUserNameAsync(AppUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));


            return Task.Run(() => {
                user.NormalizedUsername = normalizedName;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetPasswordHashAsync(AppUser user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (passwordHash == null) throw new ArgumentNullException(nameof(passwordHash));


            return Task.Run(() => {
                user.PasswordHash = passwordHash;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetSecurityStampAsync(AppUser user, string stamp, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (stamp == null) throw new ArgumentNullException(nameof(stamp));


            return Task.Run(() => {
                user.SecurityStamp = stamp;
                _storageService.UpdateUser(user);
            });
        }

        public Task SetUserNameAsync(AppUser user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (userName == null) throw new ArgumentNullException(nameof(userName));


            return Task.Run(() => {
                user.Username = userName;
                _storageService.UpdateUser(user);
            });
        }

        public Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => {
                var result = new IdentityResult();
                _storageService.UpdateUser(user);
                return result;
            });
        }
        public Task AddToRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (roleName == null) throw new ArgumentNullException(nameof(user));


            return Task.Run(() => {
                var role = _storageService.GetRoleByName(roleName);
                role.Users.Add(user);
            });
        }

        public Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));


            return Task.Run<IList<string>>(() => {
                var result = new List<string>();
                _storageService.GetRoles(role => role.Users.Contains(user)).ForEach(role => result.Add(role.Name));
                return result;
            });
        }

        public Task<IList<AppUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (roleName == null) throw new ArgumentNullException(nameof(roleName));


            return Task.Run<IList<AppUser>>(() => {
                var role = _storageService.GetRoleByName(roleName);
                return role.Users;
            });
        }

        public Task<bool> IsInRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (roleName == null) throw new ArgumentNullException(nameof(roleName));


            return Task.Run<bool>(() => {
                var role = _storageService.GetRoleByName(roleName);
                return role.Users.Contains(user);
            });
        }

        public Task RemoveFromRoleAsync(AppUser user, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (roleName == null) throw new ArgumentNullException(nameof(roleName));


            return Task.Run(() => {
                var role = _storageService.GetRoleByName(roleName);
                role.Users.Remove(user);
                _storageService.UpdateRole(role);
            });
        }
    }
}
