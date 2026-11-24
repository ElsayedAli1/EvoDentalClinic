using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.DAL.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(
            IServiceProvider services, string email, string password, string fullName)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. الأدوار
            foreach (var role in new[] { Roles.Doctor, Roles.Receptionist, Roles.Patient })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. حساب الدكتور
            var doctor = await userManager.FindByEmailAsync(email);
            if (doctor is null)
            {
                doctor = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(doctor, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to seed doctor: {errors}");
                }

                await userManager.AddToRoleAsync(doctor, Roles.Doctor);
            }
        }
    }
}
