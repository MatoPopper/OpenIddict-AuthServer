
# OpenIddict AuthServer

[![License](https://img.shields.io/badge/license-MIT-green.svg)](https://github.com/MatoPopper/OpenIddict-AuthServer/blob/main/LICENSE)

## Overview

**OpenIddict-AuthServer** is an **OAuth 2.0** and **OpenID Connect** Authorization Server built using **ASP.NET Core** and **OpenIddict**. It allows secure token issuance and user authentication for client applications, supporting modern authentication flows like Authorization Code with PKCE, Client Credentials, Resource Owner Password Credentials (ROPC), and Refresh Tokens.

This project demonstrates how to configure a robust authentication and authorization system using OpenIddict with ASP.NET Core. It is suitable for applications that require secure token-based authentication for APIs and client applications.

## Features

- **OAuth 2.0 & OpenID Connect Support**: Implements Authorization Code flow (with PKCE), Client Credentials flow, Resource Owner Password flow (ROPC), and Refresh Token flow.
- **Role-based authentication**: Secure API access based on scoped tokens with role-based claims.
- **ASP.NET Core Identity Integration**: Manage users and roles using Identity for authentication and authorization.
- **Token issuance**: Provides access tokens, refresh tokens, and ID tokens to client applications.
- **Data Protection**: Uses ASP.NET Core Data Protection for secure key management.
- **PostgreSQL**: Integration with PostgreSQL for persistent storage of users, tokens, and OpenIddict settings.

## Getting Started

### Prerequisites

Make sure you have the following installed on your development environment:

- .NET 6 SDK or higher
- PostgreSQL
- Postman (for testing OAuth flows)

### Setting Up the Project

1. **Clone the repository**:

   ```bash
   git clone https://github.com/MatoPopper/OpenIddict-AuthServer.git
   ```

2. **Set up the PostgreSQL database**:

   Ensure that your PostgreSQL server is running and you've configured the connection string in `appsettings.json` or via environment variables.

   Example connection string in `appsettings.json`:

   ```json
   "DbConfig": {
       "AuthStorage": "Host=localhost;Database=AuthServerDb;Username=yourUsername;Password=yourPassword",
       "UserStorage": "Host=localhost;Database=UserStorageDb;Username=yourUsername;Password=yourPassword"
   }
   ```

3. **Run database migrations** to apply the required schema:

   ```bash
   dotnet ef database update
   ```

4. **Run the application**:

   ```bash
   dotnet run
   ```

   The server will start on `https://localhost:5001` by default.

### Testing with Postman

You can use **Postman** to test the different OAuth 2.0 flows. Here's how to set up the Authorization Code flow:

1. Create a new **OAuth 2.0** request in Postman.
2. Set the **Authorization URL** to:
   
   ```plaintext
   https://localhost:7049/connect/authorize
   ```

3. Set the **Token URL** to:

   ```plaintext
   https://localhost:7049/connect/token
   ```

4. Set the **Callback URL** to:

   ```plaintext
   https://oauth.pstmn.io/v1/callback
   ```

5. Configure the **Client ID** and **Client Secret** according to the client credentials registered in the server.

   - Client ID: `postman`
   - Client Secret: `postman-secret`

6. Use the following **Scope**:

   ```plaintext
   postApi
   ```

7. Complete the flow by exchanging the authorization code for a token, and use the token to authenticate API requests.

## Project Structure

- **Controllers**: The primary controller is `AccountController`, which handles user authentication (login and logout).
- **Data Protection**: The app uses **ASP.NET Core Data Protection** to securely manage encryption keys for tokens and cookies.
- **OpenIddict Configuration**: The `Program.cs` contains the setup for OpenIddict flows and client registration.
- **Database**: Uses **Entity Framework Core** with **PostgreSQL** for user management and storing OpenIddict-related data.

## OAuth Flows Supported

1. **Authorization Code with PKCE**: Best practice for public clients such as SPAs and mobile apps.
2. **Client Credentials Flow**: For machine-to-machine communication (M2M).
3. **Resource Owner Password Flow (ROPC)**: For direct username and password login.
4. **Refresh Token Flow**: To refresh access tokens without requiring the user to log in again.

## Configuration

To configure new OAuth clients, update the following section in `SeedData`:

```csharp
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
```

## Security Considerations

1. **HTTPS**: Ensure that the application is always run using HTTPS to secure token exchanges.
2. **Client Secrets**: Avoid hardcoding client secrets in production environments.
3. **Key Management**: Use **ASP.NET Core Data Protection** to manage encryption keys securely.
4. **CORS**: Implement Cross-Origin Resource Sharing (CORS) if required for external applications.

## License

This project is licensed under the MIT License. See the [LICENSE](https://github.com/MatoPopper/OpenIddict-AuthServer/blob/main/LICENSE) file for details.

## Contribution

Feel free to contribute to the project by opening issues or submitting pull requests. Make sure to follow the project's guidelines and code of conduct.
