using System;
using System.Linq;
using StringHelper;
using Genom.TextTV;

namespace TextTVapp
{
	/// <summary>
	/// User interface class
	/// </summary>
	public class UI {

		/// <summary>
		/// Print the header bar
		/// <returns>Length of left header text (for printing multipage numbers)</returns>
		/// </summary>
		public static void PrintHeader(Browser texttv, bool multipage)
		{
			Page page = texttv.Page;
			UI.Color(0, 15);
			Console.SetCursorPosition(0, 0);

			// Get page category
			Category category = (Category)page.Number - (page.Number % 100);
			string left = String.Format(" {0} - {1}", page.Number, category);

			if (multipage)
				left += String.Format(" {0}/{1}", texttv.PageChunkIndex + 1, texttv.PageCount);

			string right = String.Format("{0} ", DateTime.Now.ToString());
			string center = "SVT Text".PadCenter(Console.WindowWidth - left.Length - right.Length);
			Console.Write("{0}{1}{2}\n", left, center, right);
		}

		public static void PrintFooter(int pages = 1) {
			if (pages > 1)
				{
					UI.Color(4, 15);
					// Display multipage help and page count
					Console.SetCursorPosition(0, 26);
					Console.WriteLine("Använd piltangenterna (vänster/höger) för att bläddra i sidan"
						.PadCenter(Console.WindowWidth));
					UI.Color(0,15);
				}

			UI.Color(4, 15);
			// Print page footer/help
			Console.SetCursorPosition(0, 27);
			Console.WriteLine("g Gå till :: b Föregående sida :: n Nästa sida :: h Historik :: q Avsluta".PadCenter(Console.WindowWidth));
			Console.SetWindowPosition(0, 0);
		}

		/// <summary>
		/// Set console colours
		/// </summary>
		/// <param name="fg">int value of foreground colour</param>
		/// <param name="bg">int value of background colour</param>
		public static void Color(int fg = -1, int bg = -1) {
			if(bg != -1)
				Console.BackgroundColor = (ConsoleColor)bg;
			if(fg != -1)
				Console.ForegroundColor = (ConsoleColor)fg;
		}

		/// <summary>
		/// Replacement for Console.Clear()
		/// </summary>
		public static void Clear() {
			int height = Console.WindowHeight;

			// Printing to full Window height in mono throws an exception, let's prevent that.
			if (Environment.OSVersion.ToString().Substring(0, 4) == "Unix")
				height--;
			
			Box clearbox = new Box(0, 0, Console.WindowWidth, height, ConsoleColor.Black, ConsoleColor.Black);
			clearbox.Draw();
		}


		/// <summary>
		/// Box drawing class
		/// </summary>
		public class Box
		{
			public int XPosition { get; set; }
			public int YPosition { get; set; }
			public int Width { get; set; }
			int Height { get; set; }

			public ConsoleColor ForegroundColor { get; set; }
			ConsoleColor BackgroundColor;

			/// <summary>
			/// Create a box
			/// </summary>
			/// <param name="x">Left position</param>
			/// <param name="y">Top position</param>
			/// <param name="width">Width</param>
			/// <param name="height">Height</param>
			/// <param name="fg">Foreground colour</param>
			/// <param name="bg">Background colour</param>
			public Box(int x, int y, int width, int height, 
				ConsoleColor fg = ConsoleColor.Black, 
				ConsoleColor bg = ConsoleColor.White) { 
				
				XPosition = x;
				YPosition = y;
				Width = width;
				Height = height;
				ForegroundColor = fg;
				BackgroundColor = bg;
			}


			/// <summary>
			/// Draw this box
			/// </summary>
			public void Draw() {
				Console.BackgroundColor = BackgroundColor;
				Console.ForegroundColor = BackgroundColor;
				Console.SetCursorPosition(XPosition, YPosition);

				// Drawing the box
				for (int row = 1; row <= Height; row++) {

				/* Refactored this... must be more efficient to use Pad 
				   rather than looping for every single character */
					Console.Write("".PadLeft(Width));

					/* for (int col = 1; col <= Width; col++) {
						Console.Write(" ");
					}*/
					Console.SetCursorPosition(XPosition, YPosition + row);
				}
			}

		}

		/// <summary>
		/// Message box class
		/// </summary>
		public class MessageBox {
			Box box;
			string[] Message { get; set; }
			string MessageHeader { get; set; }
			string MessageClass { get; set; }

			/// <summary>
			/// Create a message box
			/// </summary>
			/// <param name="message">Message to display</param>
			/// <param name="messageHeader">Message box heading</param>
			/// <param name="messageClass">Message class (error/info)</param>
			/// <param name="fg">Text colour</param>
			/// <param name="bg">Background colour</param>
			public MessageBox(string message, string messageHeader, string messageClass, 
				ConsoleColor fg = ConsoleColor.Black, ConsoleColor bg = ConsoleColor.White) {
				
				Message = message.Split('\n');
				MessageHeader = messageHeader;
				MessageClass = messageClass;

				// Set width of box to length of longest line + 2 (for padding)
				int width = Message.OrderByDescending(s => s.Length).First().Length + 2;
				int height;

				// Allow extra space for keyboard prompt in error messages
				if(messageClass == "error") 
					height = Message.Count() + 5;
				else
					height = Message.Count() + 3;


				// Center the box in the window
				int x = (Console.WindowWidth / 2) - (width / 2);
				int y = (Console.WindowHeight / 2) - (height / 2);

				box = new Box(x, y, width, height, fg, bg);
			}

			/// <summary>
			/// Show a message
			/// </summary>
			public void ShowMessage()
			{
				box.Draw();

				// Add the heading, padded and centered in the box
				Console.SetCursorPosition(box.XPosition + 1, box.YPosition);
				Console.ForegroundColor = box.ForegroundColor;
				Console.WriteLine(String.Format(" {0} ", MessageHeader).PadCenter(box.Width - 2, '_'));
				Console.SetCursorPosition(box.XPosition + 1, box.YPosition + 2);

				for (int i = 0; i < Message.Count(); i++)
				{
					Console.WriteLine(Message[i]);
					Console.SetCursorPosition(box.XPosition + 1, box.YPosition + i+3);
				}

				if (MessageClass == "error")
				{
					Console.SetCursorPosition(box.XPosition + 1, Console.CursorTop + 1);
					Console.WriteLine("(tryck Escape)".PadCenter(box.Width - 2));
				}

			}
		}
	}
}
