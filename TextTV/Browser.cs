using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Genom.TextTV;
using StringHelper;

namespace TextTVapp
{
	/// <summary>
	/// Main TextTV class
	/// </summary>
	public class Browser
	{
		/// <summary>
		/// The page object
		/// </summary>
		public Page Page { get; private set; }

		/// <summary>
		/// The number of chunks in a multipage
		/// </summary>
		public int PageCount { get; private set; }

		/// <summary>
		/// The current chunk index on a multipage
		/// </summary>
		public int PageChunkIndex { get; set; }

		/// <summary>
		/// Number of lines
		/// </summary>
		int LineCount { get; set; }

		/// <summary>
		/// Latest error/status message issued by the program
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// Browsing history
		/// </summary>
		List<int> History = new List<int>();


		/// <summary>
		/// Browser object constructor, simply loads the default page
		/// </summary>
		/// <param name="pagenum">Initial page</param>
		public Browser(int pagenum = 100)
		{
			TryPage(pagenum);
		}

		/// <summary>
		///	Analyzes page object and adds page number to history.
		/// </summary>
		/// <param name="pagenum">Page number to get</param>
		public void GetPageInfo()
		{
			LineCount = Page.Text.LineCount();
			PageCount = (int)Math.Ceiling((decimal)LineCount / 47); // Page count

			// If history is empty or page number has changed, add it to history
			if(History.Count == 0 || Page.Number != History[History.Count - 1])
				History.Add(Page.Number);
		}

		/// <summary>
		/// Gets page object, catching exceptions.
		/// Re-throws empty page exception to skip empty pages with the NextPage/PreviousPage methods.
		/// </summary>
		/// <param name="pagenum">Page number to get</param>
		public void TryPage(int pagenum) {
			try
			{
				Page = DownloadService.GetPage(pagenum);
				Message = String.Empty;
			}
			catch (InvalidPageException)
			{
				Message = String.Format("Sidnumret du angav ({0}) är inte giltigt, ange ett sidnummer mellan 100 och 999", pagenum);
			}

			catch (EmptyPageException)
			{
				Message = String.Format("Sida {0} är tom eller inte i sändning just nu", pagenum);
				throw;
			}

			catch (Exception)
			{
				Message = "Ett nätverksfel uppstod. Vänligen kontrollera din internetanslutning.";
			}
		}

		/// <summary>
		/// Go to the next page that has content
		/// </summary>
		public void NextPage() {
			NextPage(Page.Number);
		}

		/// <summary>
		/// Go to the next page that has content
		/// </summary>
		/// <param name="lastpage">The last tried page</param>
		public void NextPage(int lastpage) {
			int nextpage = lastpage + 1;

			Message = String.Empty;
			PageChunkIndex = 0;
			if (nextpage <= 900)
			{
				try
				{
					TryPage(nextpage);
					History.Add(nextpage);
					//PrintPage();
				}
				catch (EmptyPageException)
				{
					new UI.MessageBox("Söker nästa sida...", "Info", "info").ShowMessage();
					NextPage(nextpage);
				}
			}
			else
				TryPage(100);
		}

		/// <summary>
		/// Go to the previous page that has content
		/// </summary>
		public void PreviousPage() {
			PreviousPage(Page.Number);
		}

		/// <summary>
		/// Go to the previous page that has content
		/// </summary>
		/// <param name="lastpage">The last tried page</param>
		public void PreviousPage(int lastpage) {
			int prevpage = lastpage - 1;

			Message = String.Empty;
			PageChunkIndex = 0;
			if (prevpage >= 100)
			{
				try
				{
					TryPage(prevpage);
					History.Add(prevpage);
				}

				catch (EmptyPageException)
				{
					new UI.MessageBox("Söker föregående sida...", "Info", "info").ShowMessage();
					PreviousPage(prevpage);
				}
			}
			else
				PreviousPage(899);
		}

		/// <summary>
		/// Prompt user for page to load
		/// </summary>
		void GoToPage()
		{
			// Erase old page number and colour prompt, and show cursor
			Console.SetCursorPosition(1, 0);
			UI.Color(15, 0);
			Console.Write("   ");
			Console.SetCursorPosition(1, 0);
			Console.CursorVisible = true;

			// Get page number from user
			int pagenum;
			string userInput = Console.ReadLine();
			Console.CursorVisible = false;
			Int32.TryParse(userInput, out pagenum);

			// If input is empty or page number hasn't changed, 
			// just re-enter the current page number
			if (userInput == String.Empty || pagenum == Page.Number)
			{
				Console.SetCursorPosition(1, 0);
				UI.Color(0, 15);
				Console.WriteLine(Page.Number);
			}

			else
			{
				try
				{
					this.TryPage(pagenum);
					PageChunkIndex = 0;
				}

				// Catch the exception re-thrown by TryPage. 
				// Error message already issued, so no action needed.
				catch (EmptyPageException) { }
			}
		}

		/// <summary>
		/// Helper method to print the history
		/// </summary>
		/// <param name="selected">Index of the selected page</param>
		void PrintHistory(int selected) { 
			UI.Color(0, 15);
			Console.SetCursorPosition(0, 26);
			Console.Write("".PadLeft(Console.WindowWidth));
			Console.SetCursorPosition(0, 26);

			Console.Write(" Historik: ");

			for (int i = 0; i < History.Count && i < 10; i++) {

				if (i == selected)
					UI.Color(15, 0);
				
				Console.Write(History[History.Count - 1 - i]);

				if (i == selected)
					UI.Color(0,15);

				Console.Write("  ");
			}
		}

		/// <summary>
		/// Browse the last 10 pages visited
		/// </summary>
		void BrowseHistory() {
			
			int selected = 0;
			
			int length = History.Count > 10 ? 10 : History.Count;
			PrintHistory(selected);
			bool pause = true;
			while (pause) {
				ConsoleKeyInfo key = Console.ReadKey(true);
				switch (key.Key) { 

					case ConsoleKey.LeftArrow:
						if (selected > 0)
							PrintHistory(--selected);
						break;

					case ConsoleKey.RightArrow:
						if (selected < length - 1)
							PrintHistory(++selected);
						break;

					case ConsoleKey.Enter:
						TryPage(History[History.Count - 1 - selected]);
						pause = false;
						break;

					case ConsoleKey.Escape:
						Console.SetCursorPosition(0, 26);
						UI.Color(15, 0);
						Console.WriteLine("".PadLeft(Console.WindowWidth));
						UI.PrintFooter(PageCount);
						pause = false;
						break;

					default:
						break;
				}
			}
		}


		/// <summary>
		/// Output page contents
		/// </summary>
		public void PrintPage()
		{

			UI.Clear();

			UI.PrintHeader(this, PageCount > 1);
			
			// Print the page text
			UI.Color(15, 0);
			if (Page.Text.LineCount() < 27)
			{
				Console.WriteLine(Page.Text.HtmlDecode());
				UI.PrintFooter();
			}

			else
			{
				//int arrSize = lineCount < 47 ? 47 : lineCount + (47 - (lineCount % 47);

				string[] lines = Page.Text.HtmlDecode().Split('\n');

				for (int pageLineIndex = PageChunkIndex * 46; true; pageLineIndex++)
				{
					string col1;
					string col2;

					if (LineCount - 1 < pageLineIndex)
						col1 = "".PadRight(40);
					else
						col1 = lines[pageLineIndex].PadRight(40);

					if (LineCount - 1 < pageLineIndex + 23)
						col2 = "".PadRight(40);
					else
						col2 = lines[pageLineIndex + 23].PadRight(40);

					Console.WriteLine("{0}{1}", col1, col2);

					if (pageLineIndex == PageChunkIndex * 46 + 22)
						break;
				}
				UI.PrintFooter(PageCount);
			}
		}
		
			/* The above is a rewrite of all of this shit... wow.
			else {	

				int lineIndex = 0; // Total line count
				int columnIndex = 0; // Current column index
				int columnLine = 1; // Line count in current column
				int chunkIndex = PageChunkIndex; // Current chunk index (pair of columns)
				int lineCount = Page.Text.LineCount(); // The number of lines on the page

				string[] lines = new string[lineCount + (47 - (lineCount % 47))]; // Lines array
				lines = Page.Text.HtmlDecode().Split('\n');

				int totalLines = lineCount; // The total number of lines (including padding)

				if(lineCount % 47 != 0) // Print blank lines on pages that aren't full to avoid artifacts from previous pages
					totalLines = lineCount + (47 - (lineCount % 47));

				for(; lineIndex < totalLines; lineIndex++) {
					bool brk = false; // Use to break the loop
					string line = String.Empty;

					if(lineIndex + 1 < lineCount) 
						line = lines[lineIndex]; 
					else
						line = "".PadRight(40);

					Console.CursorLeft = columnIndex * 40; // Shift to right to print second column
					Console.CursorTop = columnLine + 1;
					UI.Color(15,0);

					// Get rid of annoying shit...
					Match match = Regex.Match(line.Trim(), @"^Forts(ättning)? följer");
					if (match.Success == false)
						Console.Write(line);
					else
						Console.Write("".PadLeft(40));

					if (columnLine++ == 23) {
						columnLine = 1;
						columnIndex++;

						if ((columnIndex > 1 && lineCount > 47) || lineIndex + 2 >= totalLines) {

							UI.Color(4, 15);

							UI.PrintFooter(PageCount);

							bool pause = true;
							while (pause) // Wait for user input, then evaluate
							{
								
								// Evaluate user key input
								EvalKey(Console.ReadKey(), true);

								switch (Command)
								{
									case "forward": // RightArrow pressed
										if (lines.Length > lineIndex + 2)
										{
											columnIndex = 0; // Reset columnIndex (output next page on left side)
											pause = false;
											chunkIndex++; // Next chunk/page
										}
										break;
									
									case "back": // LeftArrow pressed
										if (chunkIndex > 0)
										{
											columnIndex = 0; // Left column...
											lineIndex = (--chunkIndex * 46) - 1; // Go back to the last page
											pause = false;
										}
										break;

									case "quit":      // q, b, n or g
									case "previous":  //
									case "next":      //
									case "goto":      //
										columnIndex = 0;
										pause = false;
										brk = true; // We want to break out of the loops before executing these commands
										break;

									default:        // Any other key
										break;
								}
							}
						}
					}

					if (brk) // If we received a quit/previous/next/goto Command, break out of for loop first...
						break;
					
					// Otherwise, clear the Command
					Command = String.Empty;
				}

				Execute(Command); // ...then execute the Command
				Command = String.Empty;
				
			}*/

		/// <summary>
		/// Evaluate key presses and run appropriate methods
		/// </summary>
		/// <param name="key">ConsoleKeyInfo object</param>
		/// <param name="multipage">Boolean for multipages</param>
		/// <returns>True if a command was run, otherwise false</returns>
		bool EvalKey(ConsoleKeyInfo key, bool multipage = false) {

			if (multipage)
				switch (key.Key)
				{
					// First, check for arrow keys
					case ConsoleKey.LeftArrow:
						if (PageChunkIndex > 0)
							PageChunkIndex--;
						return true;
					case ConsoleKey.RightArrow:
						if (PageChunkIndex < PageCount - 1)
							PageChunkIndex++;
						return true;
					default: // Not arrow keys, check for other commands
						break;
				}

			switch (key.KeyChar)
			{
				case 'g': // Goto page...
					GoToPage();
					return true;
				case 'h': // History
					BrowseHistory();
					return true;
				case 'n':
					NextPage();
					return true;
				case 'b':
					PreviousPage();
					return true;
				case 'q': // Quit the program
					Environment.Exit(0);
					return true;
				default:
					return false;
				}
		}

		/// <summary>
		/// Loop to listen for keyboard commands
		/// </summary>
		public void WatchKeys()
		{
			while (true)
			{
				if (EvalKey(Console.ReadKey(true), PageCount > 1))
					break;
			}
		}
	}
}