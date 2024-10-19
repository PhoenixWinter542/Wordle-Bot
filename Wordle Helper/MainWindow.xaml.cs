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
		(int row, int col) position = (0, 0);
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
					tmp.Margin = new Thickness(10, 10, 10, 10);
					tmp.HorizontalAlignment = HorizontalAlignment.Center;
					tmp.VerticalAlignment = VerticalAlignment.Center;
					tmp.Width = 70;
					tmp.FontSize = 50;
					tmp.Height = 70;
					tmp.Name = "b" + row.ToString() + (col - 1).ToString();
					Grid.SetRow(tmp, row);
					Grid.SetColumn(tmp, col);
					tmp.Click += new RoutedEventHandler(ToggleColor);
					buttonRow.Add(tmp);
					Layout.Children.Add(tmp);
					stateRow.Add(0);
				}
				buttons.Add(buttonRow);
				states.Add(stateRow);
			}
		}

		private void ToggleButton(Button btn, short inc)
		{
			//Find button
			int row = btn.Name[1] - '0';
			int col = btn.Name[2] - '0';
			if (row != position.row)
				return;

			//Update state
			short tmp = (short)(states[row][col] + inc);
			if (tmp < 0)
				tmp = 3;
			states[row][col] = (short)(tmp % 4);

			//Update color
			switch (states[row][col])
			{
				case 0:
					btn.Background = new SolidColorBrush(Colors.Transparent);
					break;
				case 1:
					btn.Background = new SolidColorBrush(Colors.PaleGreen);
					break;
				case 2:
					btn.Background = new SolidColorBrush(Colors.PaleGoldenrod);
					break;
				case 3:
					btn.Background = new SolidColorBrush(Colors.PaleVioletRed);
					break;
			}
		}

		private void ToggleColor(object sender, RoutedEventArgs e)
		{
			ToggleButton((Button)sender, 1);
		}

		private void MoveBack()
		{
			if (position.col > 0)
			{
				position.col--;
			}
			states[position.row][position.col] = 0;
			ToggleButton(buttons[position.row][position.col], 0);
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
			if(position.row < maxRow)
			{
				position.row++;
				position.col = 0;
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
			for(int col = 0; col < maxCol; col++)
			{
				runArgs.Add((buttons[row][col].Content.ToString()![0], states[row][col]));
			}

			return runArgs;
		}

		private void HandleKeyPress(object sender, KeyEventArgs e)
		{
			string key = e.Key.ToString();

			//If all words are entered don't run key events
			if (maxRow == position.row)
				return;

			//TODO handle enter and backspace
			if ("Return" == key)
			{
				if (maxCol == position.col)
				{
					//Ensure every letter in row has a state
					foreach(short state in states[position.row])
					{
						if (0 == state)
							return;	//At least one letter does not have a state
					}

					//TODO update wordle analyzer with row word and values
					an.Run(GetRunArgs());
					NextRow();
					
				}
				else if (0 == position.col)
				{
					//TODO get word suggestion by running analyzer and search
					string suggestion = new Search(an.Run(null), connectionString, tableString, columnString).Run();
					for(int i = 0; i < maxCol; i++)
					{
						buttons[position.row][i].Content = suggestion[i];
					}
					NextRow();
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
			if (0 <= col)
			{
				if ("Up" == key)
				{
					ToggleButton(buttons[position.row][col], 1);
					return;
				}
				if ("Down" == key)
				{
					ToggleButton(buttons[position.row][col], -1);
					return;
				}
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