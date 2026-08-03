using Atlas.Template.Core.Enums;
using Atlas.Template.Core.Interfaces;
using Atlas.Template.Core.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Template.Infrastructure.DataSeeders
{
    public class AdminDataSeeder : IDataSeeder
    {
        public int Order => 2;

        // Add admin users to the list below
        private readonly List<AppUser> _admins = new List<AppUser>
        {
            new AppUser()
            {
                FirstName = "Mahmoud",
                LastName = "Nader",
                Email = "medonader567@gmail.com",
                UserName = "MahmoudNader",
                PhoneNumber = "01000000000"
            }
        };

        private readonly UserManager<AppUser> _usermanager;
        public AdminDataSeeder(UserManager<AppUser> usermanager)
        {
            _usermanager = usermanager;
        }

        public async Task SeedAsync(CancellationToken token = default)
        {

            foreach(var admin in _admins)
            {
                if (admin.Email == null)
                    continue;

                var user = await _usermanager.FindByEmailAsync(admin.Email);
                if (user != null)
                    continue;

                var result = await _usermanager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await _usermanager.AddToRoleAsync(admin, AppUserRoles.Admin.ToString());
                    string emailConfirmationToken = await _usermanager.GenerateEmailConfirmationTokenAsync(admin);
                    await _usermanager.ConfirmEmailAsync(admin, emailConfirmationToken);
                }
            }
        }
    }
}
