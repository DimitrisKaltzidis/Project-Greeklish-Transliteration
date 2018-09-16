namespace LinguisticTools
{
    using System;

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
        public static T GetEnum<T>(this string config, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(config))
            {
                return defaultValue;
            }

            T output;
            return Enum.TryParse(config, true, out output) ? output : defaultValue;
        }
    }
}
