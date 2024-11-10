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
		List<(char, int)> letterInc = new List<(char, int)> { ('a', 1000), ('s', 500), ('b', 700), ('t', 8), ('r', 568), ('y', 752), ('h', 123), ('w', 72) };


		[TestMethod]
		public void NodeCompareTest()
		{
			Node first = new Node();
			Node second = new Node();

			//first > second	1
			first.heur = 1990;
			second.heur = 180;
			first.offset = new List<short> { 1, 2, 3, 4, 5 };
			second.offset = new List<short> { 0, 9, 0, 1, 8 };

			Assert.AreEqual(1, first.CompareTo(second));

			//first == second	0
			first.heur = 180;
			second.heur = 180;
			first.offset = new List<short> { 1, 2, 3, 4, 5 };
			second.offset = new List<short> { 1, 2, 3, 4, 5 };

			Assert.AreEqual(0, first.CompareTo(second));

			//first < second	-1
			first.heur = 180;
			second.heur = 1990;
			first.offset = new List<short> { 1, 2, 3, 4, 5 };
			second.offset = new List<short> { 0, 9, 0, 1, 8 };

			Assert.AreEqual(-1, first.CompareTo(second));
		}

		[TestMethod]
		public void ConstructorTest()
		{
			Search search = new Search((letterPos, letterInc));
		}

		[TestMethod]
		public void HeurPosTest()
		{
			Search search = new Search((letterPos, letterInc));
			Node test = new Node();
			test.offset = new List<short> { 1, 0, 0, 0, 0 };
			Assert.AreEqual(1430, search.HeurPos(test));
		}

		[TestMethod]
		public void HeurIncTest()
		{

		}

		[TestMethod]
		public void HeurNormalizedTest()
		{

		}

		[TestMethod]
		public void HeurWordsIncLetterTest()
		{
			Search search = new Search((letterPos, letterInc));
			Node test = new Node();
			test.offset = new List<short> { 0, 0, 0, 0, 0 };
			Assert.AreEqual(3827, search.HeurWordsIncLetter(test));
		}

		[TestMethod]
		public void GetWordTest()
		{
			Node test = new Node();
			test.offset = new List<short> { 1, 1, 0, 0, 1 };
			Search search = new Search((letterPos, letterInc));
			Assert.AreEqual("straw", search.GetWord(test));
		}

		[TestMethod]
		public void IsWordTest()
		{
			Node test = new Node();
			test.offset = new List<short> { 1, 1, 0, 0, 1 };
			Search search = new Search((letterPos, letterInc));
			Assert.IsTrue(search.IsWord(test));
			test.offset = new List<short> { 1, 0, 0, 1, 1 };
			Assert.IsFalse(search.IsWord(test));
		}

		[TestMethod]
		public void ExpandTest()
		{
			Node test = new Node();
			test.offset = new List<short> { 0, 1, 0, 0, 1 };
			Search search = new Search((letterPos, letterInc), 0);
			search.frontier.Clear();
			search.closed.Clear();

			search.Expand(test);
			List<Node> result = search.frontier.ToList();
			//Sorts from least to greatest
			Assert.IsTrue(Enumerable.SequenceEqual(new List<short> { 0, 1, 0, 1, 1 }, result[0].offset));
			Assert.IsTrue(Enumerable.SequenceEqual(new List<short> { 0, 2, 0, 0, 1 }, result[1].offset));
			Assert.IsTrue(Enumerable.SequenceEqual(new List<short> { 1, 1, 0, 0, 1 }, result[2].offset));

			search = new Search((letterPos, letterInc));
			search.frontier.Clear();
			search.closed.Clear();

			search.Expand(test);
			result = search.frontier.ToList();
			Assert.IsTrue(Enumerable.SequenceEqual(new List<short> { 0, 2, 0, 0, 1 }, result[0].offset));
			Assert.IsTrue(Enumerable.SequenceEqual(new List<short> { 0, 1, 0, 1, 1 }, result[1].offset));
			Assert.IsTrue(Enumerable.SequenceEqual(new List<short> { 1, 1, 0, 0, 1 }, result[2].offset));
		}

		[TestMethod]
		public void RunTest()
		{
			Search search = new Search((letterPos, letterInc), 0);
			Assert.AreEqual("arrah", search.Run());
		}
	}
}
