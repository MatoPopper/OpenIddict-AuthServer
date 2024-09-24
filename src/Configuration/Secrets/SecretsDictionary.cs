
namespace Configuration.Secrets
{
    public static class SecretsDictionary
    {

        /// <summary>
        /// Dictionary for passing secrets to ROPC clients for third parties.
        /// The third party will never receive the actual secret, but rather its replacement.
        /// </summary>
        public static Dictionary<string, string> Secrets = new()
        {
                { "daf454sdf51sdf4sdf564sdf1", PassClient }
            };


        public const string PassClient = "SADF444654fIOPJK6874521";
    }
}
