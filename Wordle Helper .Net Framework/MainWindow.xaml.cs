using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Word_Analyzer;

namespace Wordle_Helper
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		(int row, int col, int colorCol) position = (0, 0, 0);
		readonly List<List<Button>> buttons = new List<List<Button>>();
		readonly int maxRow = 6;
		readonly int maxCol = 5;
		Analyzer an;
		string connectionString;
		string tableString = "english.dbo.words";
		string columnString = "words";

		List<List<short>> states = new List<List<short>>();

		public MainWindow()
		{
			InitializeComponent();
			connectionString = "server=DESKTOP-SV6S892;trusted_connection=Yes";
			an = new Analyzer(5, connectionString, tableString, columnString);
			for (int row = 0; row < 6; row++)
			{
				List<Button> buttonRow = new List<Button>();
				List<short> stateRow = new List<short>();
				for (int col = 1; col < 6; col++)
				{
					Button tmp = new Button();
					tmp.Background = new SolidColorBrush(Colors.Transparent);
					tmp.Content = "";
					tmp.Focusable = false;
					tmp.Margin = new Thickness(10, 10, 10, 10);
					tmp.HorizontalAlignment = HorizontalAlignment.Center;
					tmp.VerticalAlignment = VerticalAlignment.Center;
					tmp.Width = 70;
					tmp.FontSize = 50;
					tmp.Height = 70;
					tmp.Name = "b" + row.ToString() + (col - 1).ToString();
					Grid.SetRow(tmp, row);
					Grid.SetColumn(tmp, col);
					tmp.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(ColorUp);
					tmp.PreviewMouseRightButtonDown += new MouseButtonEventHandler(ColorDown);
					buttonRow.Add(tmp);
					Layout.Children.Add(tmp);
					stateRow.Add(0);
				}
				buttons.Add(buttonRow);
				states.Add(stateRow);
			}
		}

		private void UpdateButtonColor(Button btn, short state)
		{
			switch (state)
			{
				case 0:
					btn.Background = new SolidColorBrush(Colors.Transparent);
					btn.Foreground = new SolidColorBrush(Colors.Black);
					break;
				case 1:
					btn.Background = new SolidColorBrush(Colors.SeaGreen);
					btn.Foreground = new SolidColorBrush(Colors.White);
					break;
				case 2:
					btn.Background = new SolidColorBrush(Colors.Goldenrod);
					btn.Foreground = new SolidColorBrush(Colors.White);
					break;
				case 3:
					btn.Background = new SolidColorBrush(Colors.SlateGray);
					btn.Foreground = new SolidColorBrush(Colors.White);
					break;
			}
		}

		private void ToggleButton(Button btn, short inc)
		{
			//Don't toggle buttons without letters
			if (false == btn.Content.ToString().Any())
				return;

			//Find button for states	|	button name is b[Row][Col]
			int row = btn.Name[1] - '0';
			int col = btn.Name[2] - '0';
			if (row != position.row)
				return;

			//Update state	|	Won't go to 0
			short tmp = (short)(states[row][col] + inc);
			if (tmp < 1)	//Ensure 1 <= tmp
				tmp = 3;
			tmp--;      //Rescale to 0 <= tmp
			tmp %= 3;   //Ensure 0 <= tmp <= 2
			tmp++;		//Rescale to 1 <= tmp <= 3
			states[row][col] = tmp;

			//Update color
			UpdateButtonColor(btn, states[row][col]);
		}

		private void ColorUp(object sender, EventArgs e)
		{
			ToggleButton((Button)sender, 1);
		}

		private void ColorDown(object sender, EventArgs e)
		{
			ToggleButton((Button)sender, -1);
		}

		private void MoveBack()
		{
			if (position.col > 0)
			{
				position.col--;
				if (position.colorCol >= position.col && position.colorCol > 0)  //keep colorCol behind col	|	matching at 0 is fine and handled
				{
					position.colorCol--;
				}
			}
			states[position.row][position.col] = 0;
			UpdateButtonColor(buttons[position.row][position.col], 0);
			buttons[position.row][position.col].Content = "";
		}

		private void MoveForward()
		{
			if (position.col < maxCol)
			{
				position.col++;
			}
		}

		private void NextRow()
		{
			if (position.row < maxRow)
			{
				position.row++;
				position.col = 0;
				position.colorCol = 0;
			}
		}

		private void ColorPosLeft()
		{
			if(0 < position.colorCol)
			{
				position.colorCol--;
			}
		}

		private void ColorPosRight()
		{
			if(position.colorCol < position.col && position.colorCol + 1 < maxCol)
			{
				if (buttons[position.row][position.colorCol + 1].ToString().Any())
					position.colorCol++;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var window = Window.GetWindow(this);
			window.KeyDown += HandleKeyPress;
		}

		/// <summary>
		/// No buttons in the row can have null content
		/// </summary>
		/// <returns>The arguments for Analyzer.Run(args)</returns>
		private List<(char, short)> GetRunArgs()
		{
			List<(char, short)> runArgs = new List<(char, short)>();

			int row = position.row;
			for (int col = 0; col < maxCol; col++)
			{
				short state = 0;
				switch (states[row][col])
				{
					case 1: //Correct Pos
						state = 2;
						break;
					case 2: //Correct Letter Incorrect Pos
						state = 1;
						break;
					case 3: //Incorrect Letter
						state = 0;
						break;
					default:
						return null;
				}
				runArgs.Add((buttons[row][col].Content.ToString()[0], state));
			}

			return runArgs;
		}

		private void HandleKeyPress(object sender, KeyEventArgs e)
		{
			string key = e.Key.ToString();

			//If all words are entered don't run key events
			if (maxRow == position.row)
				return;

			//handle enter and backspace
			if ("Return" == key)
			{
				if (0 != states[position.row][position.colorCol])   //Advance colorCol if current letter has been evaluated
				{
					ColorPosRight();
				}

				if (maxCol == position.col)		//Submit word
				{
					//Ensure every letter in row has a state
					foreach (short state in states[position.row])
					{
						if (0 == state)
							return; //At least one letter does not have a state
					}

					//update wordle analyzer with row word and values
					var args = GetRunArgs();
					var result = an.Run(args);
					NextRow();
				}
				else if (0 == position.col)		//Suggest word
				{
					//get word suggestion by running analyzer and search
					var result = an.Run(null);
					string suggestion = new Search(result, connectionString, tableString, columnString).Run();
					for (int i = 0; i < maxCol; i++)
					{
						buttons[position.row][i].Content = char.ToUpper(suggestion[i]);
					}
					position.col = maxCol;
				}
				else
					return;
			}
			if ("Back" == key)
			{
				MoveBack();
				return;
			}
			//handle up and down arrows
			int col = position.col;
			if (col == maxCol)
				col--;
			if ("" == buttons[position.row][col].Content.ToString())
				col--;

			if ("Up" == key)
			{
				ToggleButton(buttons[position.row][position.colorCol], 1);
				return;
			}
			if ("Down" == key)
			{
				ToggleButton(buttons[position.row][position.colorCol], -1);
				return;
			}
			if("Left" == key)
			{
				ColorPosLeft();
			}
			if("Right" == key)
			{
				ColorPosRight();
			}

			if (position.col >= maxCol)
				return;

			if (key.Count() != 1)
				return;
			if (!Char.IsLetter(key[0]))
				return;

			buttons[position.row][position.col].Content = key;
			MoveForward();
		}
	}
}