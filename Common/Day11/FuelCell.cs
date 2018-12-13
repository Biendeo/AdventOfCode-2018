using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Day11 {
	class FuelCell {
		public int X { get; }
		public int Y { get; }
		public int GridSerial { get; }
		public int RackID { get { return X + 10; } }
		public int PowerLevel {
			get {
				return (((RackID * Y) + GridSerial) * RackID) % 1000 / 100 - 5;
			}
		}

		public FuelCell(int x, int y, int gridSerial) {
			X = x;
			Y = y;
			GridSerial = gridSerial;
		}
	}
}
