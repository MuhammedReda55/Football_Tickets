using Football_Tickets.Data;
using Football_Tickets.DbInitionlizer;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (!_roleManager.Roles.Any())
            {
                _roleManager.CreateAsync(new(SD.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new(SD.UserRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new(SD.ManagerRole)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new()
                {
                    UserName = "Admin123@gmail.com",
                    Email = "Admin123@gmail.com",
                    Address = "Cairo",
                    Name = "mohamed reda"

                }, "358741enG@").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(_userManager.FindByNameAsync("Admin123@gmail.com").GetAwaiter().GetResult(), SD.AdminRole);
            }





        }
    }
}
