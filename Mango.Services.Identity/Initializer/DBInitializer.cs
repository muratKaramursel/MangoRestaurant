using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Mango.Services.Identity.Initializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitializer(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (_roleManager.FindByNameAsync(SD.Admin).Result != null)
                return;

            _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();

            ApplicationUser adminApplicationUser = new()
            {
                FirstName = "Murat",
                LastName = "Karamürsel",
                UserName = "murat.karamursel",
                Email = "murat.karamursel@obss.tech",
                EmailConfirmed = true,
                PhoneNumber = "+905446168710",
                PhoneNumberConfirmed = true,
            };

            _userManager.CreateAsync(adminApplicationUser, "Admin123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(adminApplicationUser, SD.Admin).GetAwaiter().GetResult();
            _userManager.AddClaimsAsync(
                adminApplicationUser,
                new List<Claim>()
                {
                    new Claim(JwtClaimTypes.Name, $"{adminApplicationUser.FirstName} {adminApplicationUser.LastName}"),
                    new Claim(JwtClaimTypes.GivenName, adminApplicationUser.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, adminApplicationUser.LastName),
                    new Claim(JwtClaimTypes.Role, SD.Admin),
                }
            ).GetAwaiter().GetResult();

            ApplicationUser customerApplicationUser = new()
            {
                FirstName = "Customer",
                LastName = "Karamürsel",
                UserName = "customer.karamursel",
                Email = "muratkaramursel@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "+905446168710",
                PhoneNumberConfirmed = true,
            };

            _userManager.CreateAsync(customerApplicationUser, "Customer123*").GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(customerApplicationUser, SD.Customer).GetAwaiter().GetResult();
            _userManager.AddClaimsAsync(
                customerApplicationUser,
                new List<Claim>()
                {
                    new Claim(JwtClaimTypes.Name, $"{customerApplicationUser.FirstName} {customerApplicationUser.LastName}"),
                    new Claim(JwtClaimTypes.GivenName, customerApplicationUser.FirstName),
                    new Claim(JwtClaimTypes.FamilyName, customerApplicationUser.LastName),
                    new Claim(JwtClaimTypes.Role, SD.Customer),
                }
            ).GetAwaiter().GetResult();
        }
    }
}