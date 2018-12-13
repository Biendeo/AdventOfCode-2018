using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common.Day13 {
	public class MineCartMadness {
		private TrackTile[,] track;
		private List<Cart> carts;
		private int activeCarts => carts.Sum(cart => cart.Crashed ? 0 : 1);
		private int width;
		private int height;
		private Tuple<int, int> firstCrash;

		public MineCartMadness() {
			// The input is a map of the track, with the carts on top of the track. It's easier to
			// load the input as a 2D array, and then convert it to the internal data type.
			var inputAsList = new List<string>();
			carts = new List<Cart>();
			string input = Resources.Input_Day13;
			using (var reader = new StringReader(input)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					inputAsList.Add(line);
				}
			}
			width = inputAsList[0].Length;
			height = inputAsList.Count;
			track = new TrackTile[width, height];
			for (int y = 0; y < height; ++y) {
				for (int x = 0; x < width; ++x) {
					switch (inputAsList[y][x]) {
						case ' ':
							track[x, y] = TrackTile.None;
							break;
						case '|':
							track[x, y] = TrackTile.Vertical;
							break;
						case '-':
							track[x, y] = TrackTile.Horizontal;
							break;
						case '/':
							track[x, y] = TrackTile.ForwardCurve;
							break;
						case '\\':
							track[x, y] = TrackTile.BackwardCurve;
							break;
						case '+':
							track[x, y] = TrackTile.Intersection;
							break;
						case '^':
							track[x, y] = TrackTile.Vertical;
							carts.Add(new Cart(x, y, Direction.Up));
							break;
						case '<':
							track[x, y] = TrackTile.Horizontal;
							carts.Add(new Cart(x, y, Direction.Left));
							break;
						case 'v':
							track[x, y] = TrackTile.Vertical;
							carts.Add(new Cart(x, y, Direction.Down));
							break;
						case '>':
							track[x, y] = TrackTile.Horizontal;
							carts.Add(new Cart(x, y, Direction.Right));
							break;
					}
				}
			}
		}

		private void Print() {
			for (int y = 0; y < height; ++y) {
				for (int x = 0; x < width; ++x) {
					var cart = carts.Find(c => c.X == x && c.Y == y);
					if (cart == null || cart.Crashed) {
						switch (track[x, y]) {
							case TrackTile.None:
								Console.Write(' ');
								break;
							case TrackTile.Horizontal:
								Console.Write('-');
								break;
							case TrackTile.Vertical:
								Console.Write('|');
								break;
							case TrackTile.ForwardCurve:
								Console.Write('/');
								break;
							case TrackTile.BackwardCurve:
								Console.Write('\\');
								break;
							case TrackTile.Intersection:
								Console.Write('+');
								break;
						}
					} else {
						switch (cart.Direction) {
							case Direction.Up:
								Console.Write('^');
								break;
							case Direction.Left:
								Console.Write('<');
								break;
							case Direction.Down:
								Console.Write('v');
								break;
							case Direction.Right:
								Console.Write('>');
								break;
						}
					}
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}

		private void RunUntilOneCartLeft() {
			// To run a tick simulation, we need to sort all the carts by row first and then column.
			// Then, for each one, figure out where it moves forward (this requires a bunch of case
			// checking since the turns work independently of the intersections.

			// When a cart moves, check whether it collides with any carts, and if they do, mark
			// both as crashed (this preserves the list so that the iterators don't break). This
			// continues until there is one cart left.

			// This cannot be parallelized easily unless you can detect whether all the carts are
			// at least 3 or more spaces away from each other. Still, most of the operations are
			// constant in time, and my input only had 17 carts which wouldn't get that much of a
			// benefit. You must handle each cart in sequence when they are close, and you must
			// handle each tick in sequence.
			for (int tick = 0; activeCarts > 1; ++tick) {
				carts.Sort((a, b) => {
					return (a.Y.CompareTo(b.Y)) * 100000 + a.X.CompareTo(b.X);
				});
				foreach (var cart in carts) {
					if (!cart.Crashed) {
						switch (cart.Direction) {
							case Direction.Up:
								--cart.Y;
								break;
							case Direction.Left:
								--cart.X;
								break;
							case Direction.Down:
								++cart.Y;
								break;
							case Direction.Right:
								++cart.X;
								break;
						}

						switch (track[cart.X, cart.Y]) {
							case TrackTile.ForwardCurve:
								switch (cart.Direction) {
									case Direction.Up:
										cart.Direction = Direction.Right;
										break;
									case Direction.Left:
										cart.Direction = Direction.Down;
										break;
									case Direction.Down:
										cart.Direction = Direction.Left;
										break;
									case Direction.Right:
										cart.Direction = Direction.Up;
										break;
								}
								break;
							case TrackTile.BackwardCurve:
								switch (cart.Direction) {
									case Direction.Up:
										cart.Direction = Direction.Left;
										break;
									case Direction.Left:
										cart.Direction = Direction.Up;
										break;
									case Direction.Down:
										cart.Direction = Direction.Right;
										break;
									case Direction.Right:
										cart.Direction = Direction.Down;
										break;
								}
								break;
							case TrackTile.Intersection:
								switch (cart.TurnDirection) {
									case TurnDirection.Left:
										switch (cart.Direction) {
											case Direction.Up:
												cart.Direction = Direction.Left;
												break;
											case Direction.Left:
												cart.Direction = Direction.Down;
												break;
											case Direction.Down:
												cart.Direction = Direction.Right;
												break;
											case Direction.Right:
												cart.Direction = Direction.Up;
												break;
										}
										cart.TurnDirection = TurnDirection.Forward;
										break;
									case TurnDirection.Forward:
										cart.TurnDirection = TurnDirection.Right;
										break;
									case TurnDirection.Right:
										switch (cart.Direction) {
											case Direction.Up:
												cart.Direction = Direction.Right;
												break;
											case Direction.Left:
												cart.Direction = Direction.Up;
												break;
											case Direction.Down:
												cart.Direction = Direction.Left;
												break;
											case Direction.Right:
												cart.Direction = Direction.Down;
												break;
										}
										cart.TurnDirection = TurnDirection.Left;
										break;
								}
								break;
						}

						foreach (var otherCart in carts) {
							if (!otherCart.Crashed && otherCart != cart && otherCart.X == cart.X && otherCart.Y == cart.Y) {
								if (firstCrash == null) {
									firstCrash = new Tuple<int, int>(cart.X, cart.Y);
								}
								cart.Crashed = true;
								otherCart.Crashed = true;
							}
						}
					}
				}
			}
		}

		public Tuple<int, int> Part1() {
			// For part 1, the first crash must be noted and then returned. Since it doesn't take
			// too long to run part 2 as well, this just runs the same function, but just notes
			// the solution when it shows up.
			RunUntilOneCartLeft();
			return firstCrash;
		}

		public Tuple<int, int> Part2() {
			// For part 2, run until there is one cart left, and then return its position.
			RunUntilOneCartLeft();
			var cart = carts.Find(x => !x.Crashed);
			return new Tuple<int, int>(cart.X, cart.Y);
		}
	}
}
