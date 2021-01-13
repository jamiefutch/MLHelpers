using System;
using System.Collections.Generic;

using MLHelpers.Text;

namespace StopWordsTest
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			string[] sw = new[] {"the", "a", "and"};

			StopWordsRemover swr = new StopWordsRemover();

			var w = swr.RemoveStopWords("the dog ate a bone");


		}
	}
}
