// Ported from https://github.com/jeremyrsellars/cecil/blob/5ef0e385227f568c8b9e83a0d57c1b3d56ba8028/src/cecil/src/cecil/util.cljc

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Datalect.Util
{
    public static class SqlUtil
    {
        internal const string CecilTokenPattern = @"(?i)\""(?:\""\""|[^\""]+)*\""|'(?:''|[^']+)*'|--[^\r\n]*[\r\n]*|;[^\r\n]*[\r\n]*|/\*(?:(?!\*/)[\s\S])*\*/|\s+|\d+(?:\.\d+)?|!=|<>|(?:inner|left|right|full)\s+(?:outer\s+)?join\b|order\s+by\b|group(?:\s+by)?\b|:?\w+|.";

        public static Regex TokenRegex = new Regex(CecilTokenPattern, RegexOptions.Compiled);

        /// <summary>
        /// Yields the sqlText as a series of substrings (tokens).
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public static IEnumerable<string> Tokenize(string sqlText)
        {
            var match = TokenRegex.Match(sqlText ?? "");
            while (match.Success)
            {
                yield return match.Value;
                match = match.NextMatch();
            }
        }

        /// <summary>
        /// Yields a whitespace-normalized version of the specified SQL text.
        /// </summary>
        /// <param name="sqlText"></param>
        /// <returns></returns>
        public static string CanonicalWhitespace(string sqlText) =>
            TokenRegex.Replace(sqlText ?? "",
                m => string.IsNullOrEmpty(m.Value) ? " " : m.Value);
    }
}
