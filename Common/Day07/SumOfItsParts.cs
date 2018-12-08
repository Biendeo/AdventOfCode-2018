using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.Day07 {
	/// <summary>
	/// The main class for the day 7 challenge.
	/// </summary>
	public class SumOfItsParts {
		private List<StepOrder> stepOrderings;
		private HashSet<char> steps;

		public SumOfItsParts() {
			// To initialize the object, load the input as a list of orderings. The orderings can
			// be easily read by grabbing specific characters from the string.

			// Each step is also noted in a set so that it's easy to know whether some letters don't
			// exist.
			stepOrderings = new List<StepOrder>();
			steps = new HashSet<char>();
			string input = Resources.Input_Day07;
			using (var reader = new StringReader(input)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					var ordering = new StepOrder(line);
					stepOrderings.Add(ordering);
					steps.Add(ordering.Before);
					steps.Add(ordering.After);
				}
			}
		}

		public int TimeForChar(char c) {
			return c - 'A' + 1 + 60;
		}

		public string Part1() {
			// For part 1, figure out the correct ordering of the steps, with a tie-breaker being
			// alphabetical order.

			// This isn't very easy to solve with parallelization; my solution was to iterate over
			// the characters, and find the first one alphabetically that satisfies every ordering
			// (i.e. every ordering where this letter appears after another one, those first letters
			// have already been added to the list). The trouble is that you require a strict
			// ordering when iterating through the letters, which cannot easily be parallelized, and
			// between generations, the available letters keeps changing which means you cannot
			// predict the steps in advance. The solution below is a single-threaded solution.
			string currentOrder = "";
			var remainingSteps = new SortedSet<char>(steps);
			while (remainingSteps.Count > 0) {
				foreach (char letter in remainingSteps) {
					bool requirementsMet = true;
					foreach (var order in stepOrderings) {
						if (letter == order.After && remainingSteps.Contains(order.Before)) {
							requirementsMet = false;
							break;
						}
					}
					if (requirementsMet) {
						currentOrder += letter;
						remainingSteps.Remove(letter);
						break;
					}
				}
			}

			return currentOrder;
		}

		private class Worker {
			public char Letter;
			public int TimeRemaining;

			public Worker(char letter, int timeRemaining) {
				Letter = letter;
				TimeRemaining = timeRemaining;
			}
		};

		public int Part2() {
			// For part 2, if each letter c takes c + 61 - 'A' time units to complete, and 5 workers
			// are simultaneously working on any available tasks, how many time units does it take
			// to complete the whole task?

			// This is similar to the previous part when determining which letters are available,
			// but the difference is what to do with that list. While there's letters left, let each
			// worker take a tick off their work, then determine any available letters, and then
			// distribute that to the workers. Again, it's very difficult to parallelize this, as
			// each of these must be completed in sequence. You can perform the worker operations
			// easily in parallel with a lock on the collections used, but by that point you don't
			// save any time when dealing with the threads. The solution below is again a
			// single-threaded solution.
			var workers = new List<Worker>() {
				new Worker('?', 0),
				new Worker('?', 0),
				new Worker('?', 0),
				new Worker('?', 0),
				new Worker('?', 0)
			};

			int currentTimeStep = 0;

			var remainingSteps = new SortedSet<char>(steps);
			var completedSteps = new HashSet<char>();
			while (completedSteps.Count < steps.Count) {
				foreach (var worker in workers) {
					if (worker.Letter != '?') {
						--worker.TimeRemaining;
						if (worker.TimeRemaining == 0) {
							completedSteps.Add(worker.Letter);
							worker.Letter = '?';
						}
					}
				}

				var availableSteps = new List<char>();
				foreach (char letter in remainingSteps) {
					bool requirementsMet = true;
					foreach (var order in stepOrderings) {
						if (letter == order.After && !completedSteps.Contains(order.Before)) {
							requirementsMet = false;
							break;
						}
					}
					if (requirementsMet) {
						availableSteps.Add(letter);
					}
				}


				foreach (var worker in workers) {
					if (worker.Letter == '?' && availableSteps.Count > 0) {
						worker.Letter = availableSteps[0];
						remainingSteps.Remove(worker.Letter);
						availableSteps.Remove(worker.Letter);
						worker.TimeRemaining = TimeForChar(worker.Letter);
					}
				}

				++currentTimeStep;
			}

			return currentTimeStep - 1;
		}
	}
}
