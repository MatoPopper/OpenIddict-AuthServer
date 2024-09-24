using IdentityUsers.Models.Enums;
using IdentityUsers.Models.Results;
using IdentityUsers.Models;
using Microsoft.AspNetCore.Identity;
using IdentityUsers.Extensions;

namespace IdentityUsers
{
    public class UserGenerator
    {
        public static async Task CreateRoles(RoleManager<Role> roleManager, string[] roles)
        {
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(new Role() { Name = role });
            }
        }

        /// <summary>
        /// User create
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <param name="emailConfirmed"></param>
        /// <returns></returns>
        public static async Task<CreateUserResult> CreateUser(UserManager<AuthUser> userManager, string email, string? password, RoleEnum role, bool emailConfirmed)
        {
            var newUser = new AuthUser()
            {
                UserName = email,
                Email = email,
                EmailConfirmed = emailConfirmed,
            };
            IdentityResult createdUser;
            if (password != null)
            {
                createdUser = await userManager.CreateAsync(newUser, password);
            }
            else
            {
                createdUser = await userManager.CreateAsync(newUser);
            }

            List<IdentityError>? errors = [];

            if (!createdUser.Succeeded)
            {
                errors.AddRange(createdUser.Errors);
            }
            else
            {
                IdentityResult createdRole = await userManager.AddToRoleAsync(newUser, role.ToString().ToCamelCase());

                if (!createdRole.Succeeded)
                {
                    errors.AddRange(createdRole.Errors);
                }
            }

            return new CreateUserResult
            {
                Errors = errors.Count != 0 ? errors.GroupBy(a => a.Description).Select(group => group.First()).ToList() : null,
                CreatedUser = newUser
            };
        }
    }
}
