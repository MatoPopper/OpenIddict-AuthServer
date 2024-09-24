using System.Text;

namespace IdentityUsers.Extensions
{
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string str)
            => string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();

        public static string ToProperCase(this string str)
        {
            var chars = str.ToCharArray();
            if (chars.Length > 0)
            {
                chars[0] = char.ToUpper(chars[0]);
                var builder = new StringBuilder();
                for (var i = 0; i < chars.Length; i++)
                {
                    if ((chars[i]) == '-')
                    {
                        i += 1;
                        builder.Append(char.ToUpper(chars[i]));
                    }
                    else
                    {
                        builder.Append(chars[i]);
                    }
                }
                return builder.ToString();
            }
            return str;
        }

        public static string FirstLetterToLower(this string str)
            => char.ToLowerInvariant(str[0]) + str.Substring(1);

        public static bool IsNumeric(this string text)
            => double.TryParse(text, out _);

        public static string ToCamelCase(this string value)
           => string.IsNullOrEmpty(value) ? value : char.ToLowerInvariant(value[0]) + value.Substring(1);
    }
}
