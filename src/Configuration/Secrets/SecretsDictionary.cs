
namespace Configuration.Secrets
{
    public static class SecretsDictionary
    {
        /// <summary>
        /// Dictionary for passing secrets to ROPC clients for third parties.
        /// The third party will never receive the actual secret, but rather its replacement.
        /// </summary>
        private static readonly Dictionary<string, string> secrets = new()
        {
                { "daf454sdf51sdf4sdf564sdf1", PassClient }
        };

        public static IReadOnlyDictionary<string, string> Secrets => secrets;

        public const string PassClient = "SADF444654fIOPJK6874521";
    }
}
