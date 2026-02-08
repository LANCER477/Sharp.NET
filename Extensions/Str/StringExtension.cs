using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpKnP321.Extensions.Str
{
    public static class StringExtension
    {
        /// <summary>
        /// Transforms from "the_snake_case" naming to "TheCamelCase" style
        /// </summary>
        /// <param name="str">name to transform</param>
        /// <returns>name in Capital Camel Case</returns>
        public static String SnakeToCamel(this String str)
        {
            return String.Join(
                String.Empty,
                str
                .Split('_', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s[0].ToString().ToUpper() + s[1..].ToLower())
            );
            // Slices: s[0] - один символ, s[1..4] - діапазон з 1 по 4 символи
            // s[1..] - з першого до останнього, s[..3] - з початку до 3-го
            // s[1..^1] - з першого до передостаннього, s[..^1] - без останнього
        }
    }
}
