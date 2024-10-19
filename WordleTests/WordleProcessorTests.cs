using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word_Analyzer;

namespace WordleTests
{
	[TestClass]
	public class WordleProcessorTests
	{
		string ConnectionString = "server=DESKTOP-SV6S892;trusted_connection=Yes";

		[TestMethod]
		public void ProcessTest()
		{

		}

		[TestMethod]
		public void InverseWordleTest()
		{
			string answer = WordleProcessor.GetRandWord(ConnectionString);
			List<string> results = WordleProcessor.InverseWordle("saree", answer);
			string output = "Answer: " + answer + "\nGuesses: ";
			bool first = true;
			foreach(string word in results)
			{
				if (first)
					first = false;
				else
					output += ", ";
				output += word;
			}
			Console.WriteLine(output);
		}

		[TestMethod]
		public void GetRandWordTest()
		{
			Assert.IsNotNull(WordleProcessor.GetRandWord(ConnectionString));
		}
		
		[TestMethod]
		public void GetWordAtTest()
		{
			Assert.IsNotNull(WordleProcessor.InverseWordle("saree", WordleProcessor.GetWordAt(0, 3, ConnectionString)));
			Assert.IsNotNull(WordleProcessor.InverseWordle("saree", WordleProcessor.GetWordAt(1, 3, ConnectionString)));
			Assert.IsNotNull(WordleProcessor.InverseWordle("saree", WordleProcessor.GetWordAt(2, 3, ConnectionString)));
			Assert.IsNotNull(WordleProcessor.InverseWordle("saree", WordleProcessor.GetWordAt(3, 3, ConnectionString)));
		}

		[TestMethod]
		public void AverageTries()	//5.15 at 100 runs
		{
			double average = 0;
			int runs = 100;
			for(int i = 0; i < runs; i++)
			{
				List<string> results = WordleProcessor.InverseWordle("cares", WordleProcessor.GetRandWord(ConnectionString));
				average += (double)results.Count / runs;
			}
			Console.WriteLine("Average Tries: " + average);
		}

		[TestMethod]
		public void AverageSaintTries()	//4.94 at 100 runs
		{
			double average = 0;
			int runs = 10;
			for (int i = 0; i < runs; i++)
			{
				List<string> results = WordleProcessor.InverseWordle("saint", WordleProcessor.GetRandWord(ConnectionString));
				average += (double)results.Count / runs;
			}
			Console.WriteLine("Average Tries: " + average);
		}

		[TestMethod]
		public void OnePercentTest()
		{
			double average = 0;
			int runs = 159;
			for (int i = 0; i < runs; i++)
			{
				List<string> results = WordleProcessor.InverseWordle("cares", WordleProcessor.GetWordAt(i, runs, ConnectionString));
				average += (double)results.Count / runs;
			}
			Console.WriteLine("Average Tries: " + average);
		}

		[TestMethod]
		public void TenPercentTest()
		{
			double average = 0;
			int runs = 1592;
			for (int i = 0; i < runs; i++)
			{
				List<string> results = WordleProcessor.InverseWordle("cares", WordleProcessor.GetWordAt(i, runs, ConnectionString));
				average += (double)results.Count / runs;
			}
			Console.WriteLine("Average Tries: " + average);
		}

		[TestMethod]
		public void RunAllTest() //4.9451
		{
			double average = 0;
			int runs = 15920;
			for (int i = 0; i < runs; i++)
			{
				List<string> results = WordleProcessor.InverseWordle("cares", WordleProcessor.GetWordAt(i, runs, ConnectionString));
				average += (double)results.Count / runs;
			}
			Console.WriteLine("Average Tries: " + average);
		}
	}
}
