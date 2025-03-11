
namespace Configuration.Configurations
{
    /// <summary>
    /// Enum for database types.
    /// </summary>
    public enum DatabaseType
    {
        MySQL,
        MSSQL,
        PostgreSQL
    }

    public class DbConfig
    {
        /// <summary>
        /// Host
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public string? Port { get; set; }

        public string? AuthStorage { get; set; }
        public string? UserStorage { get; set; }
        public string? CoreStorage { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Returns the connection string based on the provided database type and name.
        /// </summary>
        public string GetConnectionString(DatabaseType dbType, string databaseName)
        {
            return dbType switch
            {
                DatabaseType.MSSQL => GetMssqlConnectionString(databaseName),
                DatabaseType.PostgreSQL => GetPostgreConnectionString(databaseName),
                _ => throw new ArgumentException("Unsupported database type."),
            };
        }

        private string GetMssqlConnectionString(string databaseName)
        {
            // Default port for MSSQL is 1433
            var port = !string.IsNullOrEmpty(Port) ? Port : "1433";

            return $"Server={Host},{port};Database={databaseName};User Id={Username};Password={Password};TrustServerCertificate=true;MultipleActiveResultSets=true;";
        }

        private string GetPostgreConnectionString(string databaseName)
        {
            // Default port for PostgreSQL is 5432
            var port = !string.IsNullOrEmpty(Port) ? Port : "5432";

            return $"Host={Host};Port={port};Database={databaseName};Username={Username};Password={Password};SSL Mode=Prefer;Trust Server Certificate=true;";
        }
    }
}
