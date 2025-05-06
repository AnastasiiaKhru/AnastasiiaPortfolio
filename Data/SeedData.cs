using Microsoft.AspNetCore.Identity;
using AnastasiiaPortfolio.Models;
using AnastasiiaPortfolio.Services;

namespace AnastasiiaPortfolio.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var mongoDBService = serviceProvider.GetRequiredService<MongoDBService>();

            // Check if admin user exists
            var adminEmail = "anastasiiakhru@gmail.com";
            var adminUser = await mongoDBService.GetUserByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Role = "Admin"
                };

                // Hash the password (you should implement proper password hashing)
                adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1!");
                
                await mongoDBService.CreateUserAsync(adminUser);
            }
        }
    }
} 