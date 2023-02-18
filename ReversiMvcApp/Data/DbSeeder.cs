using Microsoft.AspNetCore.Identity;

namespace ReversiMvcApp.Data;

public static class DbSeeder
{
    public static async void SeedUsers(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager){
        string[] roles = { "Beheerder", "Mediator", "Speler" };
     
        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole() { Name = role });
            }
        }
    }
}