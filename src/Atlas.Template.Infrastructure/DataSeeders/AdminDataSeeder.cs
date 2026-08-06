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
        private readonly (AppUser User, string Password)[] _admins =
        {
            (new AppUser()
            {
                FirstName = "Mahmoud",
                LastName = "Nader",
                Email = "medonader567@gmail.com",
                UserName = "MahmoudNader",
                PhoneNumber = "01000000000"
            }, "MahmoudNader@admin123"),
            (new AppUser()
            {
                FirstName = "Admin",
                LastName = "02",
                Email = "admin@gmail.com",
                UserName = "Admin02",
                PhoneNumber = "01000000000"
            }, "Admin2@admin123"),

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
                if (admin.User.Email == null)
                    continue;

                var user = await _usermanager.FindByEmailAsync(admin.User.Email);
                if (user != null)
                    continue;

                var result = await _usermanager.CreateAsync(admin.User, admin.Password);
                if (result.Succeeded)
                {
                    await _usermanager.AddToRoleAsync(admin.User, AppUserRoles.Admin.ToString());
                    string emailConfirmationToken = await _usermanager.GenerateEmailConfirmationTokenAsync(admin.User);
                    await _usermanager.ConfirmEmailAsync(admin.User, emailConfirmationToken);
                }
            }
        }
    }
}
