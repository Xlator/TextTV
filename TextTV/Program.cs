using System;
using StringHelper;

namespace TextTVapp
{
	class Program
	{
		static void Main(string[] args)
		{
			// Basic console settings
			Console.SetWindowSize(82, 28);
			Console.CursorVisible = false;
			Console.CursorSize = 100;
			
			// Show welcome message until loading is complete
			new UI.MessageBox("SVT Text-TV i C#\n\nav Viktor Jackson, i samarbete med Genom AB\nMars-April 2012\n\nwww.svt.se/texttv",
				"Välkommen!", "info", ConsoleColor.Black, ConsoleColor.Yellow).ShowMessage();

			// Create the browser object and get the start page
			Browser TextTV = new Browser();
			System.Threading.Thread.Sleep(3000);
			//TextTV.PrintPage();

			while (true) // Begin program loop
			{
				// If an exception was caught by TryPage(), print an error message
				if (TextTV.Message != String.Empty)
				{
					new UI.MessageBox(TextTV.Message.WordWrap(40), "Felmeddelande", "error",
						ConsoleColor.White, ConsoleColor.DarkRed).ShowMessage();

					while (true)
						// Wait for user to press escape, then reprint the last page.
						if (Console.ReadKey(true).Key == ConsoleKey.Escape)
							break;
				}

				// Output the page, then wait for input
				TextTV.GetPageInfo();
				TextTV.PrintPage();
				TextTV.WatchKeys();
			}
		}
	}
}