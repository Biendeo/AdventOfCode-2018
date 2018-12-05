using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Day02 {
	/// <summary>
	/// The main class for the day 2 challenge.
	/// </summary>
	public class InventoryManagementSystem {
		private List<string> values;

		public InventoryManagementSystem() {
			// To initialize the object, simply load the input in as a list of strings. No other
			// input is necessary. The input is a list of 250 26-character strings of letters.
			values = new List<string>();
			string input = Resources.Input_Day2;
			using (var reader = new StringReader(input)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					values.Add(line);
				}
			}
		}

		public int Part1() {
			// For this part, we need to note how many of the lines contain exactly two identical
			// letters, and how many contain exactly three identical letters (some strings may have
			// both), and then return the product of those two values.

			// This can be parallelized easily by splitting the input evenly between available
			// worker threads and atomically writing to these two integers.
			int appearsTwice = 0;
			int appearsThrice = 0;

			values.AsParallel().ForAll(s => {
				// For each string, I've reduced this down to an O(n) operation since every
				// character needs to be scanned. Run through the list, and increment that value in
				// an array by one. Then iterate over the array and check to see if at least one
				// element has a 2 or 3 as a value. Those values must be atomically increment, which
				// fortunately is handled by Interlocked.Increment().

				int[] seenValues = new int[256];
				for (int j = 0; j < s.Length; ++j) {
					++seenValues[s[j]];
				}
				bool seenTwice = false;
				bool seenThrice = false;
				for (int j = 'a'; j <= 'z'; ++j) {
					if (seenValues[j] == 2 && !seenTwice) {
						Interlocked.Increment(ref appearsTwice);
						seenTwice = true;
					} else if (seenValues[j] == 3 && !seenThrice) {
						Interlocked.Increment(ref appearsThrice);
						seenThrice = true;
					}
				}
			});

			return appearsTwice * appearsThrice;
		}

		public string Part2() {
			// For this part, we need to find two strings that differ by only one character, and
			// then return that string but without the differing character.
			
			// This can be easily split among different threads, as long as the one that finds a
			// solution can terminate the other threads. This can be done by starting both each
			// thread and each internal loop iteration with a check on a boolean. Fortunately,
			// when a result is found, that thread needs to spend a little bit of time computing
			// the result, so the other threads can finish up.

			// One potential optimization can be only checking the strings forward in the list, and
			// not backwards. This can halve the amount of time overall, but cannot easily be done
			// with a foreach loop unfortunately.

			// One solution I tried that attempted to solve this involved creating all the pairs
			// of the strings before doing the parallel loop. This doesn't scale well though,
			// resulting in massive memory usage and long CPU times before the loop is even reached.
			// This solution was removed immediately.
			
			bool foundResult = false;
			string result = "";

			values.AsParallel().ForAll(s => {
				if (!foundResult) {
					foreach (string otherS in values) {
						if (!foundResult) {
							int differences = 0;
							for (int i = 0; i < s.Length; ++i) {
								if (s[i] != otherS[i]) {
									++differences;
									if (differences >= 2) {
										break;
									}
								}
							}
							if (differences == 1) {
								foundResult = true; // Relying on C# having atomic writes here.
								for (int i = 0; i < s.Length; ++i) {
									if (s[i] == otherS[i]) {
										result += otherS[i];
									}
								}
								break;
							}
						}
					}
				}
			});

			return result;
		}
	}
}
