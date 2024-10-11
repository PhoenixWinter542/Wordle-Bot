using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word_Analyzer
{
	public struct Node : IComparable<Node>
	{
		public long heur;
		public List<short> offset;	//Saves the word based on positions in letterPos

		public int CompareTo(Node other)
		{
			int comp = heur.CompareTo(other.heur);
			if (comp != 0)
				return comp;

			if (offset.SequenceEqual(other.offset))
				return 0;

			return 1;	//We don't care which offset is less or greater, only if they are equal or not
		}
	}

	public class Search
	{
		private static string connectionString = "server=DESKTOP-SV6S892;trusted_connection=Yes";

		public List<List<(char letter, int sum)>> letterPos;	//Tracks the number of words containing each letter for each position
		public List<(char letter, int sum)> letterInc;   //Tracks the number of words containing each letter
		public int heurChoice;

		public SortedSet<Node> frontier;
		public SortedSet<Node> closed;

		public Search((List<List<(char, int)>> pos, List<(char, int)> inc) input)
		{
			heurChoice = 1;
			letterPos = input.pos;
			letterInc = input.inc;

			frontier = new SortedSet<Node>();
			closed = new SortedSet<Node>();

			Node first = new Node();
			first.offset = new List<short>();
			for (int i = 0; i < input.pos.Count; i++)
			{
				first.offset.Add(0);
			}
			Heuristic(ref first);
			frontier.Add(first);
		}
		public Search((List<List<(char, int)>> pos, List<(char, int)> inc) input, int heur) : this(input)
		{
			heurChoice = heur;
		}

		public void Heuristic(ref Node node)
		{
			switch(heurChoice)
			{
				case 0:
					node.heur = HeurBasic(node);
					break;
				default:
				case 1:
					node.heur = HeurWordsIncLetter(node);
					break;
			}
		}

		//Sums the # of words each letter pos is in
		public long HeurBasic(Node node)
		{
			long heur = 0;
			for(int i = 0; i < node.offset.Count; i++)
			{
				heur += letterPos[i][node.offset[i]].sum;
			}
			return heur;
		}

		//performs HeurBasic and adds the # of words each letter is in
		//Duplicate letters are ignored
		public long HeurWordsIncLetter(Node node)
		{
			long heur = HeurBasic(node);
			string used = "";
			for (int i = 0; i < node.offset.Count; i++)
			{
				char letter = letterPos[i][node.offset[i]].letter;
				if (false == used.Contains(letter))
				{
					used += letter;
					foreach ((char letterTest, int sum) in letterInc)
					{
						if (letterTest == letter)
						{
							heur += sum;
							break;
						}
					}
				}
			}
			return heur;
		}

		public bool IsWord(Node node)
		{
			string word = GetWord(node);
			string query = "SELECT * FROM english.dbo.words WHERE words = '" + word + "';";
			SqlConnection conn = new SqlConnection(connectionString);
			SqlCommand cmd = new SqlCommand(query, conn);

			conn.Open();
			SqlDataReader reader = cmd.ExecuteReader();
			reader.Read();
			bool isWord = reader.HasRows;
			reader.Close();
			conn.Close();

			return isWord;
		}

		public string GetWord(Node node)
		{
			string word = "";
			short i = 0;
			foreach(short pos in node.offset)
			{
				word += letterPos[i][pos].letter;
				i++;
			}

			return word;
		}

		public void Expand(Node node)
		{
			for(int i = 0; i < node.offset.Count; i++)
			{
				Node newNode = new Node();
				newNode.offset = node.offset.ToList();
				newNode.offset[i]++;
				if (letterPos[i].Count > newNode.offset[i])
				{
					Heuristic(ref newNode);
					if (false == closed.Contains(newNode) && false == frontier.Contains(newNode))
						frontier.Add(newNode);
				}
			}
		}

		public string Run()
		{
			while (true)
			{
				if(false == frontier.Any())
				{
					return null;
				}
				if(true == IsWord(frontier.Max()))
				{
					return GetWord(frontier.Max());
				}
				else
				{
					Expand(frontier.Max());
					closed.Add(frontier.Max());
					frontier.Remove(frontier.Max());
				}
			}
		}

	}
}
