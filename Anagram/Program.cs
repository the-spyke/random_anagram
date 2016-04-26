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
				// Format is "<sentence> <word> <entry count (int)>"
				// Entry count is used only for test result output, so may be 0.
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
				var source = tests[testIndex].ToLowerInvariant();

				source = source.Replace("\r", string.Empty);
				source = source.Replace("\t", string.Empty);
				source = source.Replace("\n", string.Empty);

				var parts = source.Split(' ');

				var sentence = parts[0];
				var word = parts[1];
				var shouldBe = Convert.ToInt32(parts[2]);

				// We're using only lowercase latin letters, the 'a' has code 97, so we can transform letters
				// to the array's index.
				const int firstCharOffset = 97;

				var stopWatch = new Stopwatch();

				stopWatch.Start();

				// Arrays to count letters in the word.
				var remainingLettersOriginal = new int[26];

				foreach (var letter in word)
				{
					var letterIndex = letter - firstCharOffset;

					remainingLettersOriginal[letterIndex] += 1;
				}

				var remainingLetters = new int[26];
				var remainingLettersCount = word.Length;
				var windowWidth = 0;

				Buffer.BlockCopy(remainingLettersOriginal, 0, remainingLetters, 0, remainingLetters.Length);

				var entries = new List<int>();

				var position = 0;
				var isSliding = false;

				while (position < sentence.Length)
				{
					var letterIndexCurrent = sentence[position] - firstCharOffset;
					var windowPosition = position - windowWidth;
					var letterIndexAtWindowPosition = sentence[windowPosition] - firstCharOffset;

					// If there're still remaining letters in anagram and current one is suitable.
					if (remainingLetters[letterIndexCurrent] > 0)
					{
						remainingLetters[letterIndexCurrent] -= 1;

						// We've found an anagram. Try to slide a window.
						if (remainingLettersCount == 1)
						{
							entries.Add(windowPosition);

							isSliding = true;

							remainingLetters[letterIndexAtWindowPosition] += 1;
						}
						else
						{
							remainingLettersCount -= 1;
							windowWidth += 1;
						}
					}
					// Current letter is missing in remaining letters and we can't slide the window further. So, we need to reset.
					else if (letterIndexCurrent != letterIndexAtWindowPosition)
					{
						// Sliding by one letter was wrong. Reset and not forget to include that wrong letter in new window.
						// position - 2 + 1 = position - 1. The new windows will start from that letter. 
						if (isSliding)
						{
							isSliding = false;
							position -= 2;
						}

						Buffer.BlockCopy(remainingLettersOriginal, 0, remainingLetters, 0, remainingLetters.Length);

						remainingLettersCount = word.Length;
						windowWidth = 0;
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
	}
}
