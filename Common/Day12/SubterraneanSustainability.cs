using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Day12 {
	public class SubterraneanSustainability {
		private string initialState;
		private List<Rule> rules;

		public SubterraneanSustainability() {
			// The input is given as a string that includes the initial state, and a bunch of rules.
			// Thse are handled separately.
			rules = new List<Rule>();
			string input = Resources.Input_Day12;
			using (var reader = new StringReader(input)) {
				string line = reader.ReadLine();
				initialState = line.Substring(15);
				line = reader.ReadLine();
				while ((line = reader.ReadLine()) != null) {
					var rule = new Rule(line);
					// Because of the implementation below, you can ignore rules that result in no
					// plant, since we'll default to no plant existing.
					if (rule.WillHavePlant) {
						rules.Add(new Rule(line));
					}
				}
			}
		}

		private long SumAtGeneration(long endGeneration) {
			// To work out this problem, a set of longs are used to store the indices of all the
			// plants. This makes it easy handle how the plants move before 0 at the beginning and
			// how they gradually slide further in the positive direction over time. Earlier array
			// based implementations were not very feasible over time.

			// Each generation, a new set is created to handle the next generation, and each
			// position two before the first plant to two after the last plant are handled to figure
			// out if there should be a plant in the new spot.

			// This cannot be parallelized between generations, but you can do a small bit of
			// concurrency when iterating over each index. However, I need to use a long version of
			// Enumerable.Range, which doesn't exist, so currently the solution is single-threaded.
			var plants = new HashSet<long>();
			for (int i = 0; i < initialState.Length; ++i) {
				if (initialState[i] == '#') {
					plants.Add(i);
				}
			}

			long convergeReminder = 0;

			for (long generation = 0; generation < endGeneration; ++generation) {
				if (generation == 10000) {
					// The plants would converge and be moving to the right by some rate, so I'll
					// try and track it here.
					convergeReminder = plants.Max();
				} else if (generation == 20000) {
					// Now let's detect how far all the plants moved.
					long nextConvergeReminder = plants.Max();
					long plantsMoveRatePerTenThousand = nextConvergeReminder - convergeReminder;
					var finalPlants = new HashSet<long>();
					long plantIncrement = (endGeneration - generation) / 10000 * plantsMoveRatePerTenThousand;
					foreach (int plant in plants) {
						finalPlants.Add(plant + plantIncrement);
					}
					plants = finalPlants;
					break;
				}
				var newGeneration = new HashSet<long>();
				for (long i = plants.Min() - 2; i < plants.Max() + 4; ++i) {
					foreach (var rule in rules) {
						bool doesThisRuleMatch = true;
						for (int x = 0; x < 5; ++x) {
							if (plants.Contains(i + x - 2) != (rule.Pattern[x] == '#')) {
								doesThisRuleMatch = false;
								break;
							}
						}
						if (doesThisRuleMatch) {
							lock (newGeneration) {
								newGeneration.Add(i);
							}
							break;
						}
					}
				}
				plants = newGeneration;
			}

			long sum = 0;
			foreach (long i in plants) {
				sum += i;
			}

			return sum;
		}

		public long Part1() {
			// For this part, simply return the sum of the plants after generation 20. This can
			// easily be simulated.
			return SumAtGeneration(20);
		}

		public long Part2() {
			// For part 2, generation 50 billion must be computed. Fortunately, the pattern
			// converges and moves to the right at a constant rate after a while, so that is
			// detected by generation 20000, and is computed from there.
			return SumAtGeneration(50000000000);
		}
	}
}
