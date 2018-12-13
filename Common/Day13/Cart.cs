using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Day13 {
	class Cart {
		public int X;
		public int Y;
		public Direction Direction;
		public TurnDirection TurnDirection;
		public bool Crashed;

		public Cart(int x, int y, Direction direction) {
			X = x;
			Y = y;
			Direction = direction;
			TurnDirection = TurnDirection.Left;
			Crashed = false;
		}
	}
}
