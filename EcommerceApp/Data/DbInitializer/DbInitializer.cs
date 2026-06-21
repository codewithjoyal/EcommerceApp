using Microsoft.AspNetCore.Identity;
namespace EcommerceApp.Data.DbInitializer
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }
            var adminUser = await userManager.FindByEmailAsync("admin@ecommerce.com");
            if (adminUser == null) {
                var user = new IdentityUser
                {
                    UserName= "admin@ecommerce.com",
                    Email = "admin@ecommerce.com",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }          
        }
    }
}
