using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word_Analyzer
{
	public class Analyzer : IDisposable
	{
		private static string connectionString = "server=DESKTOP-SV6S892;trusted_connection=Yes";
		SqlConnection conn;
		SqlTransaction transaction;
		short length;

		List<char> fullAlphabet;
		List<char> bannedLetters;
		List<char> reqLetters;
		List<(char, short)> bannedPos;
		List<(char, short)> reqPos;

		public Analyzer(short length)
		{
			conn = new SqlConnection(connectionString);
			conn.Open();
			transaction = conn.BeginTransaction();
			fullAlphabet = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
			bannedLetters = new List<char>();
			bannedPos = new List<(char, short)>();
			reqPos = new List<(char, short)>();
			reqLetters = new List<char>();
			this.length = length;
		}

		public void Dispose()
		{
			transaction.Rollback();
			conn.Close();
		}

		public string CreateRegex(int pos, int length, char letter)
		{
			string regex = "";

			for (int i = 0; i < pos; i++) { regex += "_"; }
			regex += letter;
			for (int i = 0; i < length; i++) { regex += "_"; }
			
			return regex;
		}

		//0 - char not in word	|	1 - char in word, not in position	|	2 - char in word, in correct position
		public List<List<(char, long)>> Run(List<(char, short)> feedback)
		{
			//Create list of allowed letters
			for(short i = 0; i < feedback.Count; i++)
			{
				(char letter, short status) pos = feedback[i];
				switch (pos.status)
				{
					case 0:
						bannedLetters.Add(pos.letter);
						break;
					case 1:
						bannedPos.Add((pos.letter, i));
						reqLetters.Add(pos.letter);
						break;
					case 2:
						reqPos.Add((pos.letter, i));
						break;
				}
			}

			List<char> alphabet = new List<char>();
			foreach(char letter in fullAlphabet)
			{
				if(false == bannedLetters.Contains(letter))
					alphabet.Add(letter);
			}

			//Remove invalid words from dataset
			string query = "";
			if (feedback != null)
			{
				SqlCommand cmd = new SqlCommand();
				cmd.Connection = conn;
				cmd.Transaction = transaction;

				//Remove words without letters in required positions
				cmd.CommandText = "DELETE FROM english.dbo.words WHERE words NOT LIKE '";
				bool first = true;
				foreach ((char letter, short pos) req in reqPos)
				{
					if (first)
						first = false;
					else
						cmd.CommandText += "|";
                    cmd.CommandText += CreateRegex(req.pos, length, req.letter);
				}
				cmd.CommandText += "';";
				cmd.ExecuteNonQuery();

				//Remove words without required letters, no position reqs
				foreach (char letter in reqLetters)
				{
					cmd.CommandText = "DELETE FROM english.dbo.words WHERE words NOT LIKE '%" +  + "%';";
					cmd.ExecuteNonQuery();
				}

				//Remove words with letters in invalid positions


			}

			//Compute
			List<List<(char, long)>> results = new List<List<(char, long)>>();
			SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
			adapter.SelectCommand.Transaction = transaction;
			DataSet data = new DataSet();
			for (int i = 0; i < length; i++)
			{
				List<(char, long)> column = new List<(char, long)>();
				foreach (char letter in alphabet)
				{
					query = "SELECT count(*) FROM english.dbo.words WHERE words like " + CreateRegex(i, length, letter) + ";";
					adapter.SelectCommand.CommandText = query;
					data.Clear();
					adapter.Fill(data);
					column.Add((letter, long.Parse(data.Tables[0].Rows[0][0].ToString())));
				}
				column.Sort((x, y) => y.Item2.CompareTo(x.Item2));
				results.Add(column);
			}

			return results;
		}


	}
}
