using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Day01 {
	/// <summary>
	/// The main class for the Day 1 challenge.
	/// </summary>
	public class ChronalCalibration {
		private List<int> values;

		public ChronalCalibration() {
			// To initialize the object, we'll simply read in each line and store it as an int.
			values = new List<int>();
			string input = Resources.Input_Day1;
			using (var reader = new StringReader(input)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					// Every line should be in the form of +1 or -1, so every value past that first
					// character should be the actual integer value.
					int val = int.Parse(line.Substring(1));
					if (line[0] == '-') {
						// If the number starts with a -, we want the opposite of this.
						val = -val;
					} else if (line[0] == '+') {
						// If the number starts with a +, do nothing.
					} else {
						// This is just a backup case if there's a newline or another incompatible
						// line.
						continue;
					}
					// Then, add this value to the list.
					values.Add(val);
				}
			}
		}

		public int Part1() {
			// This part simply wants the end result of all the values. This could be sped up
			// by pooling the summing operation, but the LINQ sum is probably efficient enough,
			// and the input doesn't exceed 1000 values.
			return values.Sum();
		}

		public int Part2() {
			// This part requires looping through the input multiple times to find the first time
			// the current frequency repeats. This cannot be easily multi-threaded, so the
			// implementation is just left simple with a set.
			int currentFrequency = 0;
			var seenFreqs = new HashSet<int> {
				currentFrequency
			};

			while (true) {
				foreach (int val in values) {
					currentFrequency += val;
					if (seenFreqs.Contains(currentFrequency)) {
						return currentFrequency;
					} else {
						seenFreqs.Add(currentFrequency);
					}
				}
			}
		}
	}
}
