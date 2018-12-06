using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Common.Day06 {
	public class ChronalCoordinates {
		private List<Tuple<int, int>> inputCoords;
		private readonly int width;
		private readonly int height;

		public ChronalCoordinates() {
			// The input is provided as a list of integerial coordinates, each simply two numbers
			// separated by a comma. This can be read in simply by performing a regular expression
			// match on the pattern. We'll do the width/height pushing method from day 3 again to
			// know ahead of time the dimensions of the working area.
			width = 0;
			height = 0;
			var regex = new Regex("([0-9]+), ([0-9]+)");
			inputCoords = new List<Tuple<int, int>>();
			string input = Resources.Input_Day06;
			using (var reader = new StringReader(input)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					var matches = regex.Match(line);
					var newCoord = new Tuple<int, int>(int.Parse(matches.Groups[1].Value), int.Parse(matches.Groups[2].Value));
					inputCoords.Add(newCoord);
					// The + 1 is because we want to also include the tile that this is on.
					width = Math.Max(width, newCoord.Item1 + 1);
					height = Math.Max(height, newCoord.Item2 + 1);
				}
			}
		}

		private static int ManhattenDistance(Tuple<int, int> a, Tuple<int, int> b) {
			return Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);
		}

		public int Part1() {
			// For part 1, find the coordinate that has the greatest number of tiles close to it
			// that isn't infinite. A region is infinite if it is on the border of the given area,
			// since you can guarantee every coord in a straight line distance from the closest
			// point will never change.

			int[] closestTiles = new int[inputCoords.Count];
			// My input was 355x354, so this is not too taxing, but obviously it may not scale the
			// best.
			int[,] closest = new int[width, height];
			// Is there a smoother way of assigning everything to -1?
			for (int z = 0; z < width * height; ++z) {
				closest[z % width, z / width] = -1;
			}

			// Iterate through every possible tile, and find which coord is the closest. If it's a
			// tie, don't count it.
			Enumerable.Range(0, height * width).AsParallel().ForAll(z => {
				int x = z % width;
				int y = z / width;
				var coord = new Tuple<int, int>(x, y);

				int closestDistance = int.MaxValue;
				int closestIndex = -1;
				int numClosest = 0;

				foreach (int i in Enumerable.Range(0, inputCoords.Count)) {
					int distance = ManhattenDistance(inputCoords[i], coord);
					if (distance < closestDistance) {
						closestIndex = i;
						closestDistance = distance;
						numClosest = 1;
					} else if (distance == closestDistance) {
						++numClosest;
					}
				}

				if (numClosest == 1) {
					Interlocked.Increment(ref closestTiles[closestIndex]);
					closest[x, y] = closestIndex;
				}
			});

			// Now go over the border and note those tiles as infinitely stretching out.
			// Since we want the largest area, let's set these to int.MinValue so we don't have to
			// deal with them.
			Enumerable.Range(0, height * width).AsParallel().ForAll(z => {
				int x = z % width;
				int y = z / width;
				var coord = new Tuple<int, int>(x, y);

				if ((x == 0 || x == width - 1 || y == 0 || y == height - 1) && closest[x, y] != -1) {
					closestTiles[closest[x, y]] = int.MinValue;
				}
			});

			return closestTiles.Max();
		}

		public int Part2() {
			// For part 2, simply find the number of safe coords, which is when the sum of the
			// distances between it and the input coords is less than 10000. This is really simple,
			// just iterate over every tile, sum the distances, and then see if it's less than the
			// value. Parallelization is very simple on top of this, and you just operate a counter.
			const int MAXIMUM_DISTANCE = 10000;
			int safeCoords = 0;

			Enumerable.Range(0, height * width).AsParallel().ForAll(z => {
				int x = z % width;
				int y = z / width;
				var coord = new Tuple<int, int>(x, y);

				int distance = 0;

				foreach (var c in inputCoords) {
					distance += ManhattenDistance(coord, c);
				}

				if (distance < MAXIMUM_DISTANCE) {
					Interlocked.Increment(ref safeCoords);
				}
			});

			return safeCoords;
		}
	}
}
