using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.Day3 {
	/// <summary>
	/// The main class for the day 3 challenge.
	/// </summary>
	public class NMHYSI {
		private List<Claim> claims;
		private int width = 0;
		private int height = 0;

		public NMHYSI() {
			// This one requires a bit more work than the previous question. Each line stores a
			// claim's ID, left and top margins, and their width and heights. These are handled by
			// a regex match on each string.

			// As a bonus check, we can determine the total width and height of the fabric needed
			// by using a max operation on the furthest X and Y of the fabric we see (i.e. a claim's
			// left margin plus its width, and its top margin plus its height). By the end of
			// scanning the input, we know all the input, and how much fabric is then needed.
			claims = new List<Claim>();
			string input = Resources.Input_Day3;
			using (var reader = new StringReader(input)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					var claim = new Claim(line);
					width = Math.Max(width, claim.LeftMargin + claim.Width);
					height = Math.Max(height, claim.TopMargin + claim.Height);
					claims.Add(claim);
				}
			}
		}

		private int[,] FabricClaimFrequency() {
			// Both parts require to know how many times a single grid of fabric has been claimed.
			// This can be done easily with a parallel loop on every claim's squares, with an
			// atomic increment operator. The resulting array is returned. This isn't too bad
			// memory-wise, the problem only states the fabric is at least 1000x1000 units, which
			// means this will occupy at least 4MB of memory.

			// For my input, this ended up being exactly 1000x1000.
			int[,] fabric = new int[width, height];

			claims.AsParallel().ForAll(claim => {
				for (int x = claim.LeftMargin; x < claim.LeftMargin + claim.Width; ++x) {
					for (int y = claim.TopMargin; y < claim.TopMargin + claim.Height; ++y) {
						Interlocked.Increment(ref fabric[x, y]);
					}
				}
			});

			return fabric;
		}

		public int Part1() {
			// For this part, we need to figure out how many grids of fabric are being requested
			// by two or more claims. This can easily be done by returning a 2D array of the fabric
			// with each grid showing the number of claims on that grid, and then iterating over the
			// list and determining how many of these are 2 or greater.
			int[,] fabric = FabricClaimFrequency();

			int totalOverlaps = 0;
			// This could be done parallel but I'm not sure how to implement that in C# using a
			// simple array. It would also contains a lot of quick thread operations and locking.
			foreach (int a in fabric) {
				if (a >= 2) {
					++totalOverlaps;
				}
			}

			return totalOverlaps;
		}

		public int Part2() {
			// For this part, we need to figure out which claim is completely unhindered by any
			// other claim. This can be solved by finding the claim such that, using the same
			// function as Part 1, find which claim's grids are completely filled with 1.
			// This can easily be done with a parallel loop on each claim, going through all their
			// tiles and determining whether each one is 1. If any of them are not 1, the loops
			// should exit. If they all are 1, then note that ID and note that a solution has been
			// found so that future tasks don't need to search.
			int[,] fabric = FabricClaimFrequency();

			bool resultFound = false;
			int foundId = 0;
			claims.AsParallel().ForAll(claim => {
				if (!resultFound) {
					bool currentlyValid = true;
					for (int x = claim.LeftMargin; x < claim.LeftMargin + claim.Width && currentlyValid; ++x) {
						for (int y = claim.TopMargin; y < claim.TopMargin + claim.Height && currentlyValid; ++y) {
							if (fabric[x, y] != 1) {
								currentlyValid = false;
							}
						}
					}
					if (currentlyValid) {
						resultFound = true;
						foundId = claim.ID;
					}
				}
			});

			return foundId;
		}
	}
}
