﻿using Microsoft.AspNetCore.Identity;
using Neumont_Ticketing_System.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Neumont_Ticketing_System.Areas.Identity.Data
{
    public class AppRoleStore : IRoleStore<AppRole>, IQueryableRoleStore<AppRole>
    {
        private readonly AppIdentityStorageService _storageService;

        public AppRoleStore(AppIdentityStorageService storageService)
        {
            _storageService = storageService;
        }

        public IQueryable<AppRole> Roles => _storageService.GetRoles().AsQueryable();

        public Task<IdentityResult> CreateAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));


            return new Task<IdentityResult>(() => {
                var result = new IdentityResult();
                _storageService.CreateRole(role);
                return result;
            });
        }

        public Task<IdentityResult> DeleteAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));


            return new Task<IdentityResult>(() => {
                var result = new IdentityResult();
                _storageService.RemoveRole(role);
                return result;
            });
        }

        public void Dispose()
        {
        }

        public Task<AppRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (roleId == null) throw new ArgumentNullException(nameof(roleId));


            return new Task<AppRole>(() => {
                var list = _storageService.GetRoles(role => role.MongoId == roleId);
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            });
        }

        public Task<AppRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (normalizedRoleName == null) throw new ArgumentNullException(nameof(normalizedRoleName));


            return new Task<AppRole>(() => {
                var list = _storageService.GetRoles(role => role.Name == normalizedRoleName);
                if (list.Count > 0)
                    return list[0];
                else
                    return null;
            });
        }

        public Task<string> GetNormalizedRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));


            return new Task<string>(() => {
                return role.Name;
            });
        }

        public Task<string> GetRoleIdAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));


            return new Task<string>(() => {
                return role.MongoId;
            });
        }

        public Task<string> GetRoleNameAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));


            return new Task<string>(() => {
                return role.Name;
            });
        }

        public Task SetNormalizedRoleNameAsync(AppRole role, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (normalizedName == null) throw new ArgumentNullException(nameof(normalizedName));


            return new Task(() => {
                role.Name = normalizedName;
                _storageService.UpdateRole(role);
            });
        }

        public Task SetRoleNameAsync(AppRole role, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (roleName == null) throw new ArgumentNullException(nameof(roleName));


            return new Task(() => {
                role.Name = roleName;
                _storageService.UpdateRole(role);
            });
        }

        public Task<IdentityResult> UpdateAsync(AppRole role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (role == null) throw new ArgumentNullException(nameof(role));


            return new Task<IdentityResult>(() => {
                var result = new IdentityResult();
                _storageService.UpdateRole(role);
                return result;
            });
        }
    }
}
