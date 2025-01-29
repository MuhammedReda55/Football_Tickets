using Football_Tickets.Data;
using Football_Tickets.Models;
using Football_Tickets.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Football_Tickets.DbInitionlizer
{
    public class DbInitioniser : IDbInitionlizer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public DbInitioniser(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._dbContext = dbContext;
        }

        public void Initionlize()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Any())
                {
                    _dbContext.Database.Migrate();
                }

                if (!_roleManager.Roles.Any())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.UserRole)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.EmployeeRole)).GetAwaiter().GetResult();

                    var adminUser = new ApplicationUser
                    {
                        UserName = "Mohamed_Reda",
                        Email = "Admin123@gmail.com",
                        Address = "Cairo",
                        Name = "Mohamed Reda"
                    };

                    var result = _userManager.CreateAsync(adminUser, "358741enG@").GetAwaiter().GetResult();

                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(adminUser, SD.AdminRole).GetAwaiter().GetResult();
                    }
                    else
                    {
                        Console.WriteLine(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Initialization failed: {ex.Message}");
            }
        }

    }
}
