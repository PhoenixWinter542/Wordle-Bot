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
	public class SearchTests
	{
		List<List<(char, int)>> letterPos = new List<List<(char, int)>> { 
		new List<(char, int)> { ('a', 10),	('s', 4) },
		new List<(char, int)> { ('b', 200), ('t', 143), ('r', 73) },
		new List<(char, int)> { ('r', 4) },
		new List<(char, int)> { ('a', 422), ('y', 52) },
		new List<(char, int)> { ('h', 800),	('w', 33) }
		};
		List<(char, int)> letterInc = new List<(char, int)> { };

		[TestMethod]
		public void ConstructorTest()
		{
			Search search = new Search(letterPos, letterInc);
		}

		[TestMethod]
		public void HeurBasicTest()
		{
			Search search = new Search(letterPos, letterInc);
			Node test = new Node();
			test.offset = new List<short> { 1, 0, 0, 0, 0 };
			search.HeurBasic(ref test);
			Assert.AreEqual(1430, test.heur);
		}

		[TestMethod]
		public void GetWordTest()
		{
			Node test = new Node();
			test.offset = new List<short> { 1, 1, 0, 0, 1 };
			Search search = new Search(letterPos, letterInc);
			Assert.AreEqual("straw", search.GetWord(test));
		}

		[TestMethod]
		public void IsWordTest()
		{
			Node test = new Node();
			test.offset = new List<short> { 1, 1, 0, 0, 1 };
			Search search = new Search(letterPos, letterInc);
			Assert.IsTrue(search.IsWord(test));
			test.offset = new List<short> { 1, 0, 0, 1, 1 };
			Assert.IsFalse(search.IsWord(test));
		}

		[TestMethod]
		public void ExpandTest()
		{
			Node test = new Node();
			test.offset = new List<short> { 0, 1, 0, 0, 1 };
			Search search = new Search(letterPos, letterInc);
			search.frontier.Clear();
			search.closed.Clear();

			search.Expand(test);
			List<Node> result = search.frontier.ToList<Node>();
			Assert.IsTrue(Enumerable.SequenceEqual(new List<short> { 1, 1, 0, 0, 1 }, result[0].offset));
			Assert.IsTrue(Enumerable.SequenceEqual(new List<short> { 0, 2, 0, 0, 1 }, result[1].offset));
			Assert.IsTrue(Enumerable.SequenceEqual(new List<short> { 0, 1, 0, 1, 1 }, result[2].offset));
		}

		[TestMethod]
		public void RunTest()
		{
			Search search = new Search(letterPos, letterInc);
			Assert.AreEqual("arrah", search.Run());
		}
	}
}
