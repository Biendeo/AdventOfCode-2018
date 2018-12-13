using Common.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Day11 {
	public class ChronalCharge {
		private readonly int gridSerialNumber;
		private readonly FuelCell[,] fuelCells;

		public ChronalCharge() {
			// The input is simply an int.
			gridSerialNumber = int.Parse(Resources.Input_Day11);

			// The fuel cell grid should be generated now as well.
			fuelCells = new FuelCell[300, 300];
			for (int y = 0; y < 300; ++y) {
				for (int x = 0; x < 300; ++x) {
					fuelCells[x, y] = new FuelCell(x + 1, y + 1, gridSerialNumber);
				}
			}
		}

		private Tuple<int, int, int> GreatestPower(int minSize, int maxSize) {
			// To find the greatest power, for each window size available (part 1 only views one
			// size), scan each top-left possible, then compute the size of the window.
			
			// This is parallelized per grid window size, which means that part 1 is not
			// parallelized. The lock is only used at the end to minimize starvation.

			// This could be improved by somehow improving the system, as this scales in O(n^5) when
			// the grid is scaled. I'm not too sure how to improve upon it. Part 2 takes 30 minutes
			// on my hardware, but the parallelization helps a lot.
			FuelCell largestTopLeftCell = null;
			int largestSize = -1;
			int largestPower = int.MinValue;

			Enumerable.Range(minSize, maxSize - minSize + 1).AsParallel().ForAll(size => {
				Console.WriteLine(size);
				FuelCell sizeLargestTopLeftCell = null;
				int sizeLargestSize = -1;
				int sizeLargestPower = int.MinValue;
				for (int y = 0; y < 300 - size + 1; ++y) {
					for (int x = 0; x < 300 - size + 1; ++x) {
						int totalPower = 0;
						for (int a = 0; a < size; ++a) {
							for (int b = 0; b < size; ++b) {
								totalPower += fuelCells[x + a, y + b].PowerLevel;
							}
						}
						if (totalPower > sizeLargestPower) {
							sizeLargestPower = totalPower;
							sizeLargestTopLeftCell = fuelCells[x, y];
							sizeLargestSize = size;
						}
					}
				}

				lock (this) {
					if (sizeLargestPower > largestPower) {
						largestPower = sizeLargestPower;
						largestSize = sizeLargestSize;
						largestTopLeftCell = sizeLargestTopLeftCell;
					}
				}
			});

			return new Tuple<int, int, int>(largestTopLeftCell.X, largestTopLeftCell.Y, largestSize);
		}

		public Tuple<int, int> Part1() {
			// For part 1, simply scan the grid with a 3x3 window, and return the top-left co-ord
			// of the greatest power seen.
			var result = GreatestPower(3, 3);
			return new Tuple<int, int>(result.Item1, result.Item2);
		}

		public Tuple<int, int, int> Part2() {
			// For part 2, the window can be any size, so it's between 1 and 300 large.
			return GreatestPower(1, 300);
		}
	}
}
