﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Word_Analyzer
{
	public class Analyzer : IDisposable
	{
		private static string connectionString = "server=DESKTOP-SV6S892;trusted_connection=Yes";
		SqlConnection conn;
		SqlTransaction transaction;
		SqlCommand cmd;
		short length;

		public readonly List<char> fullAlphabet;
		public List<char> bannedLetters;
		public List<char> reqLetters;
		public List<(char, short)> bannedPos;
		public List<(char, short)> reqPos;

		public Analyzer(short length)
		{
			conn = new SqlConnection(connectionString);
			startConnection();
			fullAlphabet = new List<char>() { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
			bannedLetters = new List<char>();
			bannedPos = new List<(char, short)>();
			reqPos = new List<(char, short)>();
			reqLetters = new List<char>();
			this.length = length;
		}

		public Analyzer(short length, string connection) : this(length)
		{
			connectionString = connection;
		}

		public void startConnection()
		{
			if (ConnectionState.Closed != conn.State)
				return;
			conn.Open();
			transaction = conn.BeginTransaction();
			cmd = conn.CreateCommand();
			cmd.Transaction = transaction;
		}

		public void endConnection()
		{
			if (conn.State == ConnectionState.Closed)
				return;
			transaction.Rollback();
			conn.Close();
		}

		public void Dispose()
		{
			transaction.Rollback();
			conn.Close();
		}

		public string CreateRegex(int pos, char letter)
		{
			string regex = "'";

			for (int i = 0; i < pos; i++) { regex += "_"; }
			regex += letter;
			for (int i = pos + 1; i < length; i++) { regex += "_"; }
			
			return regex + "'";
		}

		public void UpdateLetters(List<(char, short)> feedback)
		{
			if (null == feedback)
				feedback = new List<(char, short)>();
			for (short i = 0; i < feedback.Count; i++)
			{
				(char letter, short status) pos = feedback[i];
				switch (pos.status)
				{
					case 0:
						bannedLetters.Add(pos.letter);
						break;
					case 1:
						bannedPos.Add((pos.letter, i));
						if (false == reqLetters.Contains(pos.letter))
							reqLetters.Add(pos.letter);
						break;
					case 2:
						if (false == reqPos.Contains((pos.letter, i)))
							reqPos.Add((pos.letter, i));
						break;
				}
			}
		}

		public List<char> GetAllowed()
		{
			List<char> alphabet = new List<char>();
			foreach (char letter in fullAlphabet)
			{
				if (false == bannedLetters.Contains(letter))
					alphabet.Add(letter);
			}
			return alphabet;
		}

		//Remove words without letters in required positions
		public int RemReqPos()
		{
			int sum = 0;
			foreach ((char letter, short pos) in reqPos)
			{
				cmd.CommandText = "DELETE FROM english.dbo.words WHERE words NOT LIKE " + CreateRegex(pos, letter) + ";";
				sum += cmd.ExecuteNonQuery();
			}
			return sum;
		}

		//Remove words without required letters, no position reqs
		public int RemReqNPos()
		{
			int sum = 0;
			foreach (char letter in reqLetters)
			{
				cmd.CommandText = "DELETE FROM english.dbo.words WHERE words NOT LIKE '%" + letter + "%';";
				sum += cmd.ExecuteNonQuery();
			}
			return sum;
		}

		//Remove words with banned letters
		public int RemBanned()
		{
			int sum = 0;
			foreach (char letter in bannedLetters)
			{
				cmd.CommandText = "DELETE FROM english.dbo.words WHERE words LIKE '%" + letter + "%';";
				sum += cmd.ExecuteNonQuery();
			}
			return sum;
		}

		//Remove words with letters in invalid positions
		public int RemInvalPos()
		{
			int sum = 0;
			foreach((char letter, short pos) in bannedPos)
			{
				cmd.CommandText = "DELETE FROM english.dbo.words WHERE words LIKE " + CreateRegex(pos, letter) + ";";
				sum += cmd.ExecuteNonQuery();
			}
			return sum;
		}

		//Compute
		public List<List<(char, int)>> ComputePos(List<char> alphabet)
		{
			List<List<(char, int)>> results = new List<List<(char, int)>>();
			for (int i = 0; i < length; i++)
			{
				List<(char, int)> column = new List<(char, int)>();
				foreach (char letter in alphabet)
				{
					cmd.CommandText = "SELECT count(*) FROM english.dbo.words WHERE words like " + CreateRegex(i, letter) + ";";
					SqlDataReader reader = cmd.ExecuteReader();
					reader.Read();
					column.Add((letter, reader.GetInt32(0)));
					reader.Close();
				}
				column.Sort((x, y) => y.Item2.CompareTo(x.Item2));
				results.Add(column);
			}

			return results;
		}

		public List<(char, int)> ComputeInc(List<char> alphabet)
		{
			List<(char, int)> results = new List<(char, int)>();
			foreach (char letter in alphabet)
			{
				cmd.CommandText = "SELECT count(*) FROM english.dbo.words WHERE words like '%" + letter + "%';";
				SqlDataReader reader = cmd.ExecuteReader();
				reader.Read();
				results.Add((letter, reader.GetInt32(0)));
				reader.Close();
			}

			return results;
		}

		//0 - char not in word	|	1 - char in word, not in position	|	2 - char in word, in correct position
		public (List<List<(char, int)>>, List<(char, int)>) Run(List<(char, short)> feedback)
		{
			startConnection();

			//Create list of allowed letters
			UpdateLetters(feedback);
			List<char> alphabet = GetAllowed();
			

			//Remove invalid words from dataset
			if (feedback != null)
			{
				//Remove words without letters in required positions
				RemReqPos();

				//Remove words without required letters, no position reqs
				RemReqNPos();

				//Remove words with banned letters
				RemBanned();

				//Remove words with letters in invalid positions
				RemInvalPos();
			}

			//Compute
			(List<List<(char, int)>>, List<(char, int)>) results = (ComputePos(alphabet), ComputeInc(alphabet));
			endConnection();
			return results;
		}


	}
}
