using Atlas.Template.Core.Enums;
using Atlas.Template.Core.Interfaces;
using Atlas.Template.Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Template.Infrastructure.DataSeeders
{
    public class RoleDataSeeder : IDataSeeder
    {
        public int Order => 1;

        private readonly RoleManager<UserRole> _roleManager;
        private readonly HashSet<string> _definedRoles;

        public RoleDataSeeder(RoleManager<UserRole> roleManager)
        {
            _roleManager = roleManager;
            _definedRoles = Enum.GetNames<AppUserRoles>().ToHashSet();
        }

        public async Task SeedAsync(CancellationToken token = default)
        {
            var existingRoles = _roleManager.Roles
                                            .Select(role => role.Name)
                                            .Where(name => name != null)
                                            .ToHashSet()!;

            foreach (var role in _definedRoles.Except(existingRoles))
            {
                var result = await _roleManager.CreateAsync(new UserRole { Name = role });
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create role '{role}': {errors}");
                }
            }
        }
    }
}
