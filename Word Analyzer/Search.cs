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
			int comp = other.heur.CompareTo(heur);
			if (comp != 0)
				return comp;

			if (offset.SequenceEqual(other.offset))
				return 0;

			return 1;	//We don't care which node is less or greater, only if they are equal or not
		}
	}

	public class Search
	{
		private static string connectionString = "server=DESKTOP-SV6S892;trusted_connection=Yes";

		public List<List<(char letter, int sum)>> letterPos;	//Tracks the number of words containing each letter for each position
		public List<(char letter, int sum)> letterInc;   //Tracks the number of words containing each letter
		public int heurChoice = 0;

		public SortedSet<Node> frontier;
		public SortedSet<Node> closed;

		public Search(List<List<(char, int)>> pos, List<(char, int)> inc)
		{
			letterPos = pos;
			letterInc = inc;

			frontier = new SortedSet<Node>();
			closed = new SortedSet<Node>();

			Node first = new Node();
			first.offset = new List<short>();
			for (int i = 0; i < pos.Count; i++)
			{
				first.offset.Add(0);
			}
			Heuristic(ref first);
			frontier.Add(first);
		}

		public void Heuristic(ref Node node)
		{
			switch(heurChoice)
			{
				case 0:
					HeurBasic(ref node);
					break;
			}
		}

		public void HeurBasic(ref Node node)
		{
			long heur = 0;
			for(int i = 0; i < node.offset.Count; i++)
			{
				heur += letterPos[i][node.offset[i]].sum;
			}
			node.heur = heur;
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
					HeurBasic(ref newNode);
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
				if(true == IsWord(frontier.First()))
				{
					return GetWord(frontier.First());
				}
				else
				{
					Expand(frontier.First());
					closed.Add(frontier.First());
					frontier.Remove(frontier.First());
				}
			}
		}

	}
}
