﻿using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Amg.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Cut of the tail of a string if it is longer than maxLength
        /// </summary>
        /// <param name="x"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Truncate(this string x, int maxLength)
        {
            return (x.Length > maxLength)
                ? x.Substring(0, maxLength)
                : x;
        }

        /// <summary>
        /// Replace line breaks by ' '
        /// </summary>
        /// <returns></returns>
        public static string OneLine(this string x)
        {
            return x.SplitLines().Join(" ");
        }

        /// <summary>
        /// True, if abbreviation is a valid abbreviation of word.
        /// </summary>
        /// Abbreviation means that all characters of abbreviation appear in word in 
        /// exactly the order they appear in abbreviation.
        /// <param name="abbreviation"></param>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsAbbreviation(this string abbreviation, string? word)
        {
            if (abbreviation.Length == 0)
            {
                return true;
            }

            if (word == null || word.Length == 0)
            {
                return false;
            }

            if (char.ToLower(word[0]) == char.ToLower(abbreviation[0]))
            {
                if (abbreviation.Length == 1)
                {
                    return true;
                }
                else if (word.Length == 1)
                {
                    return false;
                }
                else
                {
                    var restAbbreviation = abbreviation.Substring(1);
                    var restWords = Enumerable.Range(1, word.Length - 1).Select(_ => word.Substring(_));
                    return restWords.Max(_ => restAbbreviation.IsAbbreviation(_));
                }
            }
            else
            {
                return false;
            }
        }

        public static string ToCsharpIdentifier(this string text)
        {
            const string underscore = "_";
            var parts = Regex.Split(text, "[^0-9A-Za-z_]");
            if (!Regex.IsMatch(parts[0], "^[a-zA-Z]"))
            {
                parts = new[] { underscore }.Concat(parts).ToArray();
            }
            return parts.Select(Word).Join(String.Empty);
        }

        static string Word(string x)
        {
            if (x.Length == 0)
            {
                return x;
            }
            else
            {
                return x.Substring(0, 1).ToUpper() + x.Substring(1).ToLower();
            }
        }

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string Quote(this string x)
        {
            return "\"" + x.Replace("\"", "\\\"") + "\"";
        }

        /// <summary>
        /// Quotes a string.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static string Quote(this string x, string quotes)
        {
            return quotes[0] + x.Replace("\"", "\\\"") + quotes[1];
        }
    }
}
