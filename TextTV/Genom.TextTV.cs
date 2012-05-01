using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Genom.TextTV
{
    /// <summary>
    /// TextTV Categories
    /// </summary>
    public enum Category
    {
        /// <summary>Nyheter</summary>
        Nyheter = 100,
        /// <summary>Ekonomi</summary>
        Ekonomi = 200,
        /// <summary>Sport</summary>
        Sport = 300,
        /// <summary>Väder</summary>
        Väder = 400,
        /// <summary>Nyheter</summary>
        Blandat = 500,
        /// <summary>TV</summary>
        TV = 600,
        /// <summary>Innehåll</summary>
        Innehåll = 700,
        /// <summary>UR</summary>
        UR = 800
    }

    /// <summary>
    /// Page has an invalid number, only integers between 100 and 999 are allowed
    /// </summary>
    public class InvalidPageException : Exception
    {
        /// <summary>
        /// The page number
        /// </summary>
        public int Number { get; private set; }

        internal InvalidPageException(int number)
            : base("Page " + number + " is not valid, only integers between 100 and 999 are allowed")
        {
            Number = number;
        }
    }

    /// <summary>
    /// Page is not in service exception
    /// </summary>
    public class EmptyPageException : Exception
    {
        /// <summary>
        /// The page number
        /// </summary>
        public int Number { get; private set; }

        internal EmptyPageException(int number)
            : base("Page " + number + " has no content right now.")
        {
            Number = number;
        }
    }

    /// <summary>
    /// A TextTV Page
    /// </summary>
    public class Page
    {
        /// <summary>
        /// The page number
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// The page content
        /// </summary>
        public string Text { get; internal set; }

        internal Page(int number)
        {
            if (number < 100 || number > 999)
                throw new InvalidPageException(number);

            Number = number;
        }
    }

    /// <summary>
    /// Handles downloading of TextTV pages
    /// </summary>
    public static class DownloadService
    {
        /// <summary>
        /// Base URL to use, defaults to http://svt.se/texttv/{0}.html
        /// </summary>
        public static string BaseUrl { get; set; }

        static DownloadService()
        {
            BaseUrl = "http://svt.se/texttv/{0}.html";
        }

        /// <summary>
        /// Returns a page object for the given category
        /// </summary>
        /// <param name="category">Category to use</param>
        /// <returns>Page object</returns>
        public static Page GetPage(Category category)
        {
            return GetPage((int)category);
        }

        /// <summary>
        /// Returns a page object for the given page number
        /// </summary>
        /// <param name="number">number to use</param>
        /// <returns>Page object</returns>
        public static Page GetPage(int number)
        {
            Page page = new Page(number);

            WebClient web = new WebClient();

            byte[] data = web.DownloadData(String.Format(BaseUrl, page.Number));

            string s = Encoding.GetEncoding("iso-8859-1").GetString(data);

            s = Regex.Replace(s, "^<.*", String.Empty, RegexOptions.Multiline);
            s = Regex.Replace(s, "<.*?>", String.Empty);

            if (s.Trim().Length == 0)
                throw new EmptyPageException(number);

            page.Text = s;

            return page;
        }
    }
}
