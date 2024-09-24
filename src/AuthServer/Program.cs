using AuthServer;
using Configuration.Configurations;
using IdentityUsers;
using IdentityUsers.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configuration for forward headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();  // Remove default networks if you're using Docker
    options.KnownProxies.Clear();   // Remove default proxies if you're using a reverse proxy
});

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    config.AddFilter("Microsoft", LogLevel.Information);
    config.AddFilter("System", LogLevel.Information);
});

builder.Logging.AddConsole().SetMinimumLevel(LogLevel.Debug);

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<AuthDbContext>()
    .SetApplicationName("AuthServerApp") // more instance
    .UseCryptographicAlgorithms(new Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.AuthenticatedEncryptorConfiguration()
    {
        EncryptionAlgorithm = Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ValidationAlgorithm.HMACSHA256
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = "/account/login";  // Path to the login page
                    options.LogoutPath = "/account/logout"; // Path to the logout page
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Duration of the authentication cookie
                    options.SlidingExpiration = true;  // Renews the cookie's expiration time with each request
                    options.Cookie.HttpOnly = true; // Ensures that the cookie is accessible only via HTTP, not JavaScript
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // Ensures the cookie is sent over HTTPS only if the request is HTTPS

                });

var dbConfig = builder.Configuration.GetSection("DbConfig").Get<DbConfig>();

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(dbConfig?.GetConnectionString(DatabaseType.PostgreSQL, dbConfig.AuthStorage ?? string.Empty), o =>
    {
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
        o.MigrationsAssembly("AuthServer");
    }
    );
    options.UseOpenIddict();
});

builder.Services.AddDbContext<AuthUsersStorageDbContext>(options =>
{
    options.UseNpgsql(dbConfig?.GetConnectionString(DatabaseType.PostgreSQL, dbConfig.UserStorage ?? string.Empty), o =>
    {
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
        o.MigrationsAssembly("UserStorege");
    }
    );
});

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<AuthDbContext>();

builder.Services.AddIdentity<AuthUser, Role>()
            .AddEntityFrameworkStores<AuthUsersStorageDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

builder.Services.AddOpenIddict()

                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    // Configure OpenIddict to use the EF Core stores/models.
                    options.UseEntityFrameworkCore()
                        .UseDbContext<AuthDbContext>();
                })

                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                  //  options.UseAspNetCore().DisableTransportSecurityRequirement();


                    options
                        .AllowClientCredentialsFlow()
                        .AllowPasswordFlow()
                        .AllowAuthorizationCodeFlow()
                        .RequireProofKeyForCodeExchange()
                        .AllowRefreshTokenFlow();

                    options
                        .SetTokenEndpointUris("/connect/token")
                        .SetAuthorizationEndpointUris("/connect/authorize")
                        .SetUserinfoEndpointUris("/connect/userinfo");

                    options.SetIssuer(new Uri("https://auth.popelar.badcat.eu/"));

                    // Encryption and signing of tokens
                    options
                        .AddEphemeralEncryptionKey()
                        .AddEphemeralSigningKey()
                        .DisableAccessTokenEncryption();

                    // Register scopes (permissions)
                    options.RegisterScopes(OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Profile, "roles", "popelar-api");

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    options
                            .UseAspNetCore()
                            .EnableTokenEndpointPassthrough()
                            .EnableAuthorizationEndpointPassthrough()
                            .EnableUserinfoEndpointPassthrough();

                    options.AddDevelopmentEncryptionCertificate()
                            .AddDevelopmentSigningCertificate();
                }).AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

builder.Services.AddHostedService<SeedData>();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
