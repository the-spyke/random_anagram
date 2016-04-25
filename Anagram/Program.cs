using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anagram
{
	class Program
	{
		static void Main(string[] args)
		{
			var tests = new string[]
			{
				/*00*/ "a abc 0",
				/*01*/ "abc abc 1",
				/*02*/ "abcabdcba abc 4",
				/*03*/ "aaaaabaaaa aaa 5",
				/*04*/ "aaaaabaaa aaab 4",
				/*05*/ "abcabcdabc abcd 4",
				/*06*/ "abcabcdcba abcd 2",
				/*07*/ "abcabcdbac abcd 3",
				/*08*/ "ababababababababaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbababababababababaabababababbbbbbababababaaaabababbaab ab 59",
				/*09*/ @"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa 
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
						aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa 1"
			};

			for (int testIndex = 0; testIndex < tests.Length; testIndex++)
			{
				var source = tests[testIndex];

				source = source.Replace("\r", string.Empty);
				source = source.Replace("\t", string.Empty);
				source = source.Replace("\n", string.Empty);

				var parts = source.Split(' ');

				var sentence = parts[0];
				var word = parts[1];
				var shouldBe = Convert.ToInt32(parts[2]);

				const int firstCharOffset = 97;

				var stopWatch = new Stopwatch();

				stopWatch.Start();

				var currentLetters = new int[26];
				var removedLetters = new int[26];

				foreach (var letter in word)
				{
					var i = letter - firstCharOffset;

					currentLetters[i] += 1;
				}

				var currentLettersCounter = word.Length;
				var removedLettersCounter = 0;

				var entries = new List<int>();

				var position = 0;
				var nextLetterIndex = -1;

				while (position < sentence.Length)
				{
					var letterIndex = sentence[position] - firstCharOffset;

					if (currentLetters[letterIndex] > 0)
					{
						currentLetters[letterIndex] -= 1;
						removedLetters[letterIndex] += 1;

						currentLettersCounter -= 1;
						removedLettersCounter += 1;

						if (currentLettersCounter == 0)
						{
							var entryPosition = position - word.Length + 1;

							entries.Add(entryPosition);

							nextLetterIndex = sentence[entryPosition] - firstCharOffset;

							currentLetters[nextLetterIndex] += 1;
							removedLetters[nextLetterIndex] -= 1;

							currentLettersCounter += 1;
							removedLettersCounter -= 1;
						}
					}
					else if (sentence[position - removedLettersCounter] - firstCharOffset != letterIndex)
					{
						if (nextLetterIndex >= 0)
						{
							currentLetters[nextLetterIndex] -= 1;
							removedLetters[nextLetterIndex] += 1;

							currentLettersCounter -= 1;
							removedLettersCounter += 1;

							nextLetterIndex = -1;

							Swap(ref currentLetters, ref removedLetters);
							Swap(ref currentLettersCounter, ref removedLettersCounter);

							position -= 2;
						}
						else
						{
							for (int i = 0; i < currentLetters.Length; i++)
							{
								currentLetters[i] += removedLetters[i];
								removedLetters[i] = 0;
							}

							currentLettersCounter = word.Length;
							removedLettersCounter = 0;
						}
					}

					position += 1;
				}

				stopWatch.Stop();

				Console.WriteLine("Test #" + testIndex);
				Console.WriteLine("Sentence: " + sentence);
				Console.WriteLine("Word: " + word);
				Console.WriteLine("Entries count: " + entries.Count);
				Console.Write("Entries positions:");

				if (entries.Count > 0)
				{
					foreach (var entry in entries)
					{
						Console.Write(" " + entry);
					}
				}

				Console.WriteLine();
				Console.WriteLine("Time: " + stopWatch.ElapsedMilliseconds + "ms");
				Console.WriteLine("Result: " + (shouldBe == entries.Count ? "OK" : "ERROR"));
				Console.WriteLine();
			}

			Console.ReadKey();
		}

		static void Swap(ref int left, ref int right)
		{
			var leftCopy = left;

			left = right;
			right = leftCopy;
		}

		static void Swap(ref int[] left, ref int[] right)
		{
			var leftCopy = left;

			left = right;
			right = leftCopy;
		}
	}
}
