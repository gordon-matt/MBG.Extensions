using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MBG.Extensions.Core
{
    public static partial class StringExtensions
    {
        private static readonly Regex RegexHtmlTag = new Regex("<.*?>", RegexOptions.Compiled);

        #region Pluralization

        private static readonly IList<string> unpluralizables = new List<string>
        {
            "equipment",
            "information",
            "rice",
            "money",
            "species",
            "series",
            "fish",
            "sheep",
            "deer" 
        };
        private static readonly IDictionary<string, string> pluralizations = new Dictionary<string, string>
        {
            // Start with the rarest cases, and move to the most common
            { "person", "people" },
            { "ox", "oxen" },
            { "child", "children" },
            { "foot", "feet" },
            { "tooth", "teeth" },
            { "goose", "geese" },
            // And now the more standard rules.
            { "(.*)fe?", "$1ves" },         // ie, wolf, wife
            { "(.*)man$", "$1men" },
            { "(.+[aeiou]y)$", "$1s" },
            { "(.+[^aeiou])y$", "$1ies" },
            { "(.+z)$", "$1zes" },
            { "([m|l])ouse$", "$1ice" },
            { "(.+)(e|i)x$", @"$1ices"},    // ie, Matrix, Index
            { "(octop|vir)us$", "$1i"},
            { "(.+(s|x|sh|ch))$", @"$1es"},
            { "(.+)", @"$1s" }
        };
        private static readonly IDictionary<string, string> singularizations = new Dictionary<string, string>
        {
            // Start with the rarest cases, and move to the most common
            {"people", "person"},
            {"oxen", "ox"},
            {"children", "child"},
            {"feet", "foot"},
            {"teeth", "tooth"},
            {"geese", "goose"},
            // And now the more standard rules.
            {"(.*)ives?", "$1ife"},
            {"(.*)ves?", "$1f"},
            // ie, wolf, wife
            {"(.*)men$", "$1man"},
            {"(.+[aeiou])ys$", "$1y"},
            {"(.+[^aeiou])ies$", "$1y"},
            {"(.+)zes$", "$1"},
            {"([m|l])ice$", "$1ouse"},
            {"matrices", @"matrix"},
            {"indices", @"index"},
            {"(.*)ices", @"$1ex"},
            // ie, Matrix, Index
            {"(octop|vir)i$", "$1us"},
            {"(.+(s|x|sh|ch))es$", @"$1"},
            {"(.+)s", @"$1"}
        };

        #endregion
    }
}