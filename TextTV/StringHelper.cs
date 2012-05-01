using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;

namespace StringHelper
{
	/// <summary>
	/// String helper methods, adapted from PHP
	/// </summary>
	static class StringHelper
	{
		/// <summary>
		/// Replace \n with <br>
		/// </summary>
		/// <returns>Reformatted string</returns>
		public static string NewLine2Html(this string str, bool isXhtml = true) {
			string replacement = (isXhtml) ? "<br />" : "<br>";
			return str.Replace("\n", "\n" + replacement);
		}

		/// <summary>
		/// Make the first letter of the string uppercase
		/// </summary>
		/// <returns>Reformatted string</returns>
		public static string ToUpperFirst(this string str) {
			string initial = str[0].ToString().ToUpper();
			return String.Format("{0}{1}", initial, str.Substring(1));
		}

		/// <returns>Reversed string</returns>
		public static string Reverse(this string str) {
			return new string(str.ToCharArray().Reverse().ToArray());
		}

		/// <summary>
		/// Count occurences of a substring in a string
		/// </summary>
		/// <param name="haystack">String to search in</param>
		/// <param name="needle">String to search for</param>
		/// <returns>Number of hits</returns>
		public static int SubstringCount(this string haystack, string needle) {
			return SubstringCount(haystack, needle, 0, haystack.Length);
		}

		/// <summary>
		/// Count occurences of a substring in a string
		/// </summary>
		/// <param name="haystack">String to search in</param>
		/// <param name="needle">String to search for</param>
		/// <param name="offset">Offset from string start</param>
		/// <returns>Number of hits</returns>
		public static int SubstringCount(this string haystack, string needle, int offset) {
			return SubstringCount(haystack, needle, offset, haystack.Length - offset);
		}

		/// <summary>
		/// Count occurences of a substring in a string
		/// </summary>
		/// <param name="haystack">String to search in</param>
		/// <param name="needle">String to search for</param>
		/// <param name="offset">Offset from string start</param>
		/// <param name="length">Length of string to search in</param>
		/// <returns>Number of hits</returns>
		public static int SubstringCount(this string haystack, string needle, int offset, int length) {
			if (offset + length > haystack.Length) {
				Console.WriteLine("Error: Out of bounds");
				return -1;
			}

			Regex search = new Regex(Regex.Escape(needle));
			return search.Matches(haystack.Substring(offset, length)).Count;
		}

		/// <returns>Number of lines in a string</returns>
		public static int LineCount(this string str) {
			return str.SubstringCount("\n") + 1;
		}

		/// <summary>
		/// Capitalize every word in the string
		/// </summary>
		/// <returns>Reformatted string</returns>
		public static string ToUpperWords(this string str) {
			string[] sentence = str.ToUpperFirst().Split(' ');
			for (int i = 0; i < sentence.Count(); i++)
				sentence[i] = sentence[i].ToUpperFirst();

			return String.Join(" ", sentence);
		}

		/// <summary>
		/// Wrap the string to 75 characters
		/// </summary>
		/// <returns>Wrapped string</returns>
		public static string WordWrap(this string str) {
			return str.WordWrap(75, "\n");
		}

		/// <summary>
		/// Wrap the string to the desired length
		/// </summary>
		/// <param name="str">String to wrap</param>
		/// <param name="length">Length to wrap to</param>
		/// <param name="separator">Character to insert when wrapping</param>
		/// <param name="cut">Cut off words?</param>
		/// <returns>Wrapped string</returns>
		public static string WordWrap(this string str, int length, string separator = "\n", bool cut = false) {
			string[] sentence = str.Split(' ');
			string thisLine = String.Empty;
			List<string> lines = new List<string>();
			for (int i = 0; i < sentence.Count(); i++) {

				if (thisLine == "")
					thisLine = sentence[i];

				else if ((thisLine + sentence[i]).Length + 1 > length) {
					lines.Add(thisLine);
					thisLine = sentence[i];
				}

				else
					thisLine += " " + sentence[i];

				if (i == sentence.Count() - 1)
					lines.Add(thisLine);
			}

			return String.Join<string>(separator, lines);
		}

		/// <summary>
		/// Checks if string is a palindrome
		/// </summary>
		/// <returns>Boolean</returns>
		public static bool IsPalindrome(this string str) {
			string firstHalf = string.Empty;
			string secondHalf = string.Empty;

			if (str.Length % 2 == 0) {
				firstHalf = str.Substring(0, (str.Length / 2) - 1);
				secondHalf = str.Substring(str.Length / 2);
			}

			else {
				firstHalf = str.Substring(0, (str.Length - 1) / 2);
				secondHalf = str.Substring(((str.Length - 1) / 2) + 1);
			}
			string reversed = new string(secondHalf.ToCharArray().Reverse().ToArray());
			return firstHalf.ToLower() == reversed.ToLower();
		}

		/// <summary>
		/// Centers a string padded to desired width
		/// </summary>
		/// <param name="str">String</param>
		/// <param name="width">Desired width</param>
		/// <param name="padding">Character to pad with</param>
		/// <returns>Padded/Centred string</returns>
		public static string PadCenter (this string str, int width, char padding = ' ') {
			str = str.PadLeft((int)Math.Floor( (double) str.Length + (width / 2) - (str.Length / 2) ), padding);
			str = str.PadRight(width, padding);	
			return str;
		}

		/// <summary>
		///	Decodes HTML entities in string 
		/// </summary>
		/// <returns>Decoded string</returns>
		public static string HtmlDecode(this string str) {
			return WebUtility.HtmlDecode(str);
		}
	}
}
