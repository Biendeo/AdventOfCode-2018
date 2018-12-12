using Common.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Day09 {
	/// <summary>
	/// The main class for the day 9 challenge.
	/// </summary>
	public class MarbleMania {
		private int numPlayers;
		private int lastMarblePoints;

		public MarbleMania() {
			// The input is simply two integers encoded in a string, so that can be easily grabbed
			// out.
			string input = Resources.Input_Day09;
			var splitString = input.Split(' ');
			numPlayers = int.Parse(splitString[0]);
			lastMarblePoints = int.Parse(splitString[6]);
		}

		private static long PlayToWinningScore(int numPlayers, int lastMarblePoints) {
			// To determine the result, I simple just play the game, starting with the 0 marble.
			// Iterating through each marble until the last one, if the marble is not divisible by
			// 23, place it in the current position and move the current position two clockwise
			// (i.e. two to the right in a circular buffer). Otherwise, get the marble 9 behind and
			// remove it, then add the current marble and that one together and add that to the
			// current player's score, then move the current position to two marbles after that.
			// The winner is the player with the highest score tracked over that.

			// For part 1, this implementation works very quickly, but for part 2 it struggles
			// really hard because the insert operation is O(n^2) (since for every marble, you would
			// need to move up to every marble when inserting). This could be improved with a linked
			// list, but that would require a bit of overhead with tracking the current index, since
			// random access is not feasible, and I'm not sure if that would actually give a worthy
			// speed improvement.

			// Parallelization could help but I'm not sure where to put it. The biggest time cost
			// is the insertion method, which couldn't be parallelized except with a lock, which
			// means it won't help at all. A different container may allow parallelization to occur
			// since you only need to care about reaching back to the beginning of the list when
			// wrapped around, or when looking back with a 23 marble, both of which would require a
			// fair amount of additional checking.

			// This solution is a very naive implementation of the rules. It takes about an hour on
			// my computer to actually run part 2, which is not ideal.
			var marbles = new List<int>(lastMarblePoints) { 0 };
			var score = new List<long>(numPlayers);
			for (int i = 0; i < numPlayers; ++i) {
				score.Add(0);
			}

			int currentPosition = 1;
			int currentPlayer = -1;

			for (int currentMarble = 1; currentMarble <= lastMarblePoints; ++currentMarble) {
				currentPlayer = (currentPlayer + 1) % numPlayers;

				if (currentMarble % 23 == 0) {
					int scoreIncrement = 0;
					scoreIncrement += currentMarble;
					int removedMarbleIndex = (currentPosition - 9) % marbles.Count;
					if (removedMarbleIndex < 0) {
						removedMarbleIndex += marbles.Count;
					}
					scoreIncrement += marbles[removedMarbleIndex];
					marbles.RemoveAt(removedMarbleIndex);
					currentPosition = (removedMarbleIndex + 2) % marbles.Count;
					if (currentPosition == 0) {
						currentPosition = marbles.Count;
					}
					score[currentPlayer] += scoreIncrement;
				} else {
					marbles.Insert(currentPosition, currentMarble);
					currentPosition = (currentPosition + 2) % marbles.Count;
					if (currentPosition == 0) {
						currentPosition = marbles.Count;
					}
				}
			}

			return score.Max();
		}

		public long Part1() {
			// For part 1, simply play the game to the end and return the score of the winning
			// player.
			return PlayToWinningScore(numPlayers, lastMarblePoints);
		}

		public long Part2() {
			// For part 2, simply play the game as before except the highest marble is 100 times
			// higher than the input. This fortunately is the exact same as part 1, just with a
			// longer game, but it does show the problems with the implementation of the game.
			return PlayToWinningScore(numPlayers, lastMarblePoints * 100);
		}
	}
}
