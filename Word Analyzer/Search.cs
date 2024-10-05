using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Word_Analyzer
{
	public struct Node
	{
		public List<int> offset;	//Saves the word based on positions in letterPos
	}

	internal class Search
	{
		List<List<(char, long)>> letterPos;	//Tracks the number of words containing each letter for each position
		List<(char, long)> letterInc;	//Tracks the number of words containing each letter



	}
}
