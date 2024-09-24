
namespace Configuration.Configurations
{
    /// <summary>
    /// Model pro konfiguraci autentizačního serveru
    /// </summary>
    public class AuthConfig
    {
        /// <summary>
        /// Umístění auth serveru
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Je vyžadován HTTPS?
        /// </summary>
        public bool RequireHttps { get; set; }

        /// <summary>
        /// Název API - použivá se pro ověření, zda klient má povolenou Scope (tabulka ClientScopes)
        /// </summary>
        public string? ApiName { get; set; }

        /// <summary>
        /// Scoper pre ROPC Yolk
        /// </summary>
        public string? Scopes { get; set; }

        /// <summary>
        /// Secret pre ROPC Yolk
        /// </summary>
        public string? ClientSecret { get; set; }
    }
}
