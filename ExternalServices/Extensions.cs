namespace ExternalServices
{
    using System;
    using System.Text.RegularExpressions;

    public static class Extensions
    {
        /// <summary>
        /// The get enum.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <typeparam name="T">
        /// Type of
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public static T GetEnum<T>(this string config, T defaultValue)
            where T : struct
        {
            if (string.IsNullOrEmpty(config))
            {
                return defaultValue;
            }

            T output;
            return Enum.TryParse(config, true, out output) ? output : defaultValue;
        }

        /// <summary>
        /// The get bool.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="defaultValue">
        /// The default value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetBool(this object value, bool defaultValue = false)
        {
            if (value == null)
            {
                return defaultValue;
            }

            var config = value.ToString();
            if (string.IsNullOrEmpty(config))
            {
                return defaultValue;
            }

            bool output;
            return bool.TryParse(config, out output) ? output : defaultValue;
        }

        public static string StripHtml(this string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }
    }
}