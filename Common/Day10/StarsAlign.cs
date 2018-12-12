using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Day10 {
	public class StarsAlign {
		private List<Star> stars;

		public StarsAlign() {
			// The input is simply a string that encodes four integers, this can be parsed just by
			// extracting the substrings.
			stars = new List<Star>();

			string input = Resources.Input_Day10;
			using (var reader = new StringReader(input)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					var star = new Star(line);
					stars.Add(star);
				}
			}
		}

		private int RunStars(bool printMessage) {
			// To complete this puzzle, simply iterate through every star and move it by its
			// velocity until all the stars are close enough to show the message. The message should
			// occupy at most a 70x10 size, so simply detecting whether all the stars are that close
			// should be good enough. Once they are, if requested (like part 1), print the message.
			// The time is returned for part 2.

			// When printing the stars, I found it the easiest to make a 2D array of bools and
			// noting which ones are occupied by stars. Then print based on that array, which should
			// help boost performance.

			// There's some small parallelized parts such as moving every star between time steps
			// and writing to the bool array. However, you need a synchronized operation between
			// time steps in order to determine the spread of the stars, so I'm not too sure how
			// this could be improved without running multiple simulations in parallel, but that
			// would require much higher memory requirements.
			var currentStars = new List<Star>(stars);
			bool starsReadable = false;
			bool starsMovedAway = false;

			int time = 0;
			const int MAX_WIDTH = 70;
			const int MAX_HEIGHT = 10;

			for (time = 0; time < 60000 && !starsMovedAway; ++time) {
				int width = currentStars.Max(star => star.PosX) - currentStars.Min(star => star.PosX);
				int height = currentStars.Max(star => star.PosY) - currentStars.Min(star => star.PosY);

				if (width > MAX_WIDTH || height > MAX_HEIGHT) {
					if (starsReadable) {
						starsMovedAway = true;
					}
				} else {
					starsReadable = true;
					int minimumX = currentStars.Min(star => star.PosX);
					int minimumY = currentStars.Min(star => star.PosY);
					bool[,] spottedStars = new bool[MAX_WIDTH, MAX_HEIGHT];
					currentStars.AsParallel().ForAll(star => {
						spottedStars[star.PosX - minimumX, star.PosY - minimumY] = true;
					});
					if (printMessage) {
						for (int y = 0; y < MAX_HEIGHT; ++y) {
							for (int x = 0; x < MAX_WIDTH; ++x) {
								Console.Write(spottedStars[x, y] ? '#' : '.');
							}
							Console.WriteLine();
						}
					}
				}

				currentStars.AsParallel().ForAll(star => star.NextStep());
			}
			return time - 2;
		}

		public string Part1() {
			// For part 1, simply run the simulation, and print the result.
			RunStars(true);
			return "";
		}

		public int Part2() {
			// For part 2, simply return the time that the message was printed.
			return RunStars(false);
		}
	}
}
