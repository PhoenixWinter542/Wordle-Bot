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
		public void Meaty()
		{
			Analyzer analyzer = new Analyzer(5);
			List<List<(char, int)>> results = analyzer.Run(null);
			Search search = new Search(results, new List<(char, int)> { });
			Assert.AreEqual("sanes", search.Run());

			List<(char, short)> feedback = new List<(char, short)> { ('s', 0), ('a', 1), ('n', 0), ('e', 1), ('s', 0) };
			results = analyzer.Run(feedback);
			search = new Search(results, new List<(char, int)> { });
			Assert.AreEqual("", search.Run());
		}
	}
}
