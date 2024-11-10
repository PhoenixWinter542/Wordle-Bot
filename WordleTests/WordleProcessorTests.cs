using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
			List<string> results = WordleProcessor.InverseWordle("saree", answer, 1);
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
		public void HyphaTest()
		{
			string answer = "hypha";
			List<string> results = WordleProcessor.InverseWordle("cares", answer, 2);
			string output = "Answer: " + answer + "\nGuesses: ";
			bool first = true;
			foreach (string word in results)
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
			Assert.IsNotNull(WordleProcessor.InverseWordle("saree", WordleProcessor.GetWordAt(0, 3, ConnectionString), 1));
			Assert.IsNotNull(WordleProcessor.InverseWordle("saree", WordleProcessor.GetWordAt(1, 3, ConnectionString), 1));
			Assert.IsNotNull(WordleProcessor.InverseWordle("saree", WordleProcessor.GetWordAt(2, 3, ConnectionString), 1));
			Assert.IsNotNull(WordleProcessor.InverseWordle("saree", WordleProcessor.GetWordAt(3, 3, ConnectionString), 1));
		}

		[TestMethod]
		public void AverageRandTriesHeurDefault()	//5.15 at 100 runs
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 100;
			for(int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetRandWord(ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 1);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void AverageRandSaintTriesHeurDefault()	//4.94 at 100 runs
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 10;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetRandWord(ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 1);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void OnePercentTestHeurPos()
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 159;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("sanes", answer, 0);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void OnePercentTestHeurDefault()
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 159;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 1);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void OnePercentTestHeurInc()
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 159;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 2);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void OnePercentTestHeurNormalized()
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 159;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 3);
				Debug.WriteLine("finished: " + i + " " + answer + " in " + (DateTime.Now - start).TotalSeconds + " seconds");
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " s");
		}

		[TestMethod]
		public void OnePercent2to1()
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 159;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 4);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void OnePercent1to2()
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 159;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 5);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void OnePercent1to4()
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 159;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 6);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void OnePercent1to8()
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 159;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 7);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void TenPercentTestHeurDefault()
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 1592;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 1);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}

		[TestMethod]
		public void RunAllTestHeurDefault() //4.9451	may have been bugged at the time
		{
			double average = 0;
			double averageDuration = 0;
			int runs = 15920;
			for (int i = 0; i < runs; i++)
			{
				string answer = WordleProcessor.GetWordAt(i, runs, ConnectionString);
				var start = DateTime.Now;
				Debug.WriteLine("\nStarted: " + i + " " + answer);
				List<string> results = WordleProcessor.InverseWordle("cares", answer, 1);
				average += (double)results.Count / runs;
				var duration = (DateTime.Now - start).TotalSeconds;
				averageDuration += duration / runs;
				Debug.WriteLine("finished: " + i + " " + answer + " in " + duration + " seconds");
			}
			Console.WriteLine("Average Tries: " + average + "\nAverage Duration: " + averageDuration + " seconds");
		}
	}
}
