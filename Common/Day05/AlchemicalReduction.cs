using Common.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Day05 {
	public class AlchemicalReduction {
		private string startingPolymer;

		public AlchemicalReduction() {
			// The input is simply a string, so this can be read in directly.
			startingPolymer = Resources.Input_Day05.Trim();
		}

		private static bool DoUnitsMatch(char a, char b) {
			if (a >= 'a' && a <= 'z') {
				return a - 'a' + 'A' == b;
			} else if (a >= 'A' && a <= 'Z') {
				return a - 'A' + 'a' == b;
			} else {
				return false;
			}
		}

		private string CollapsePolymer(string polymer) {
			// To collapse the polymer, we perform collapse operations until no more operations are
			// able to be done (i.e. iterating over the list and finding no matches). C#'s remove
			// function seems to work fine, although it may be better to use a different operation
			// for large data.

			// It is not easy to parallelize on its own though.
			string currentPolymer = polymer;

			bool noRemovals = false;
			do {
				noRemovals = true;
				for (int i = 0; i < currentPolymer.Length - 1; ++i) {
					if (DoUnitsMatch(currentPolymer[i], currentPolymer[i + 1])) {
						currentPolymer = currentPolymer.Remove(i, 2);
						noRemovals = false;
					}
				}
			} while (!noRemovals);

			return currentPolymer;
		}

		public int Part1() {
			// For part 1, we simply need to collapse the original string and return how long it is.
			// Since part 2 also needs to run this function, the implementation has been abstracted
			// to just that function.
			return CollapsePolymer(startingPolymer).Length;
		}

		public int Part2() {
			// For part 2, for each letter, remove all instances of it from the string, and then
			// collapse it and see if it is the shortest string. The shortest length is returned.

			// This can be parallelized easily simply by completing each letter simultaneously. The
			// final value can simply be locked.
			int shortestPolymer = int.MaxValue;
			Enumerable.Range(0, 26).AsParallel().ForAll(a => {
				string currentPolymer = startingPolymer;
				for (int i = 0; i < currentPolymer.Length; ++i) {
					if (currentPolymer[i] == a + 'a') {
						currentPolymer = currentPolymer.Remove(i, 1);
						--i;
					} else if (currentPolymer[i] == a + 'A') {
						currentPolymer = currentPolymer.Remove(i, 1);
						--i;
					}
				}
				int length = CollapsePolymer(currentPolymer).Length;
				// Could this lock be neater?
				lock (this) {
					if (length < shortestPolymer) {
						shortestPolymer = length;
					}
				}
			});
			return shortestPolymer;
		}
	}
}
