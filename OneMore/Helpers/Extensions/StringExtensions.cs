﻿//************************************************************************************************
// Copyright © 2015 Waters Corporation. All rights reserved.
//************************************************************************************************

namespace River.OneMoreAddIn
{
	using System;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Xml.Linq;


	/// <summary>
	/// Some extension methods for .NET types...
	/// </summary>

	internal static class Extensions
	{

		/// <summary>
		/// Compares this string with a given string ignoring case.
		/// </summary>
		/// <param name="s">The current string</param>
		/// <param name="value">The comparison string</param>
		/// <returns>True if both strings are equal</returns>

		public static bool EqualsICIC(this string s, string value)
		{
			return s.Equals(value, StringComparison.InvariantCultureIgnoreCase);
		}


		/// <summary>
		/// Determines if the given string starts with a given substring, ignoring case.
		/// </summary>
		/// <param name="s">The current string</param>
		/// <param name="value">The substring to use as a search</param>
		/// <returns>True if the current start starts with the given substring</returns>

		public static bool StartsWithICIC(this string s, string value)
		{
			return s.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
		}


		/// <summary>
		/// Extract the first word delimeted by a non-word boundary from the given
		/// string and returns the word and updated string.
		/// </summary>
		/// <param name="s">A string with one or more words</param>
		/// <returns>
		/// A two-part ValueTuple with the word and the updated string.
		/// </returns>
		public static (string, string) ExtractFirstWord(this string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				var match = Regex.Match(s, @"^(\w+)\b*");
				if (match.Success)
				{
					var capture = match.Captures[0];
					return (capture.Value, s.Remove(capture.Index, capture.Length));
				}
			}

			return (null, s);
		}


		/// <summary>
		/// Extract the last word delimeted by a non-word boundary from the given
		/// string and returns the word and updated string.
		/// </summary>
		/// <param name="s">A string with one or more words</param>
		/// <returns>
		/// A two-part ValueTuple with the word and the updated string.
		/// </returns>
		public static (string, string) ExtractLastWord(this string s)
		{
			if (!string.IsNullOrEmpty(s))
			{
				var match = Regex.Match(s, @"\b*(\w+)$");
				if (match.Success)
				{
					var capture = match.Captures[0];
					return (capture.Value, s.Remove(capture.Index, capture.Length));
				}
			}

			return (null, s);
		}


		/// <summary>
		/// Build an XML wrapper with the specified content, ensuring the content
		/// is propertly formed XML
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static XElement ToXmlWrapper(this string s)
		{
			// ensure proper XML

			// OneNote doesn't like &nbsp; but &#160; is ok and is the same as \u00A0 but 1-byte
			var value = s.Replace("&nbsp;", "&#160;");

			// XElement doesn't like <br> so replace with <br/>
			value = Regex.Replace(value, @"\<\s*br\s*\>", "<br/>");

			// quote unquote language attribute, e.g., lang=yo to lang="yo" (or two part en-US)
			value = Regex.Replace(value, @"(\s)lang=([\w\-]+)([\s/>])", "$1lang=\"$2\"$3");

			return XElement.Parse("<wrapper>" + value + "</wrapper>");
		}


		// StringBuilder...

		public static int IndexOf(this StringBuilder s, char c)
		{
			int i = 0;
			while (i < s.Length)
			{
				if (s[i] == c)
				{
					return i;
				}

				i++;
			}

			return -1;
		}
	}
}
