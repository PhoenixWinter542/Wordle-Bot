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
	public class MainTests
	{
		[TestMethod]
		public void MeatyBasic()
		{
			Analyzer analyzer = new Analyzer(5);
			(List<List<(char, int)>>, List<(char, int)>) results = analyzer.Run(null);
			Search search = new Search(results, 0);
			Assert.AreEqual("sanes", search.Run());

			List<(char, short)> feedback = new List<(char, short)> { ('s', 0), ('a', 1), ('n', 0), ('e', 1), ('s', 0) };
			results = analyzer.Run(feedback);
			search = new Search(results, 0);
			Assert.AreEqual("areae", search.Run());

			feedback = new List<(char, short)> { ('a', 1), ('r', 0), ('e', 1), ('a', 1), ('e', 1) };
			results = analyzer.Run(feedback);
			search = new Search(results, 0);
			Assert.AreEqual("beata", search.Run());

			feedback = new List<(char, short)> { ('b', 0), ('e', 2), ('a', 2), ('t', 2), ('a', 1) };
			results = analyzer.Run(feedback);
			search = new Search(results, 0);
			Assert.AreEqual("meath", search.Run());

			feedback = new List<(char, short)> { ('m', 2), ('e', 2), ('a', 2), ('t', 2), ('h', 0) };
			results = analyzer.Run(feedback);
			search = new Search(results, 0);
			Assert.AreEqual("meaty", search.Run());
		}
		[TestMethod]
		public void MeatyWordsIncLetter()
		{
			Analyzer analyzer = new Analyzer(5);
			(List<List<(char, int)>>, List<(char, int)>) results = analyzer.Run(null);
			Search search = new Search(results);
			Assert.AreEqual("saree", search.Run());

			List<(char, short)> feedback = new List<(char, short)> { ('s', 0), ('a', 1), ('r', 0), ('e', 1), ('e', 1) };
			results = analyzer.Run(feedback);
			search = new Search(results);
			Assert.AreEqual("leany", search.Run());

			feedback = new List<(char, short)> { ('l', 0), ('e', 2), ('a', 2), ('n', 0), ('y', 2) };
			results = analyzer.Run(feedback);
			search = new Search(results);
			Assert.AreEqual("beaky", search.Run());

			feedback = new List<(char, short)> { ('b', 0), ('e', 2), ('a', 2), ('k', 0), ('y', 2) };
			results = analyzer.Run(feedback);
			search = new Search(results);
			Assert.AreEqual("peaty", search.Run());

			feedback = new List<(char, short)> { ('p', 0), ('e', 2), ('a', 2), ('t', 2), ('y', 2) };
			results = analyzer.Run(feedback);
			search = new Search(results);
			Assert.AreEqual("meaty", search.Run());
		}
	}
}
