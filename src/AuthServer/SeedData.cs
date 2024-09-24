using Configuration.Secrets;
using IdentityUsers;
using IdentityUsers.Models;
using IdentityUsers.Models.Enums;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;

namespace AuthServer
{
    public class SeedData(IServiceProvider serviceProvider) : IHostedService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var AuthContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            await AuthContext.Database.EnsureDeletedAsync(cancellationToken);
            await AuthContext.Database.EnsureCreatedAsync(cancellationToken);

            var UsersContext = scope.ServiceProvider.GetRequiredService<AuthUsersStorageDbContext>();
            await UsersContext.Database.EnsureDeletedAsync(cancellationToken);
            await UsersContext.Database.EnsureCreatedAsync(cancellationToken);

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "postman",
                ClientSecret = "postman-secret",
                DisplayName = "Postman",
                RedirectUris = { new Uri("https://localhost:4000/") },
                Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Authorization,
                        OpenIddictConstants.Permissions.Endpoints.Token,

                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "postApi",
                        OpenIddictConstants.Permissions.ResponseTypes.Code
                    }
            }, cancellationToken);

            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "pass-api",
                ClientSecret = SecretsDictionary.PassClient,
                DisplayName = "pass",
                Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.GrantTypes.Password,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "pass-api",
                        OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles,
                        OpenIddictConstants.Permissions.Prefixes.Scope + "openid",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "offline_access"
                    }
            }, cancellationToken);

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
            var testUserRole = new[] { "Superadmin", "Admin", "User" };
            await UserGenerator.CreateRoles(roleManager, testUserRole);

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AuthUser>>();
            var authUser1 = await UserGenerator.CreateUser(userManager, "test@auth.com", "Kfhskdjh_126!", RoleEnum.Superadmin, true);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
