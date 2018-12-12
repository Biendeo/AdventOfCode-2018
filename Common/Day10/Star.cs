using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Day10 {
	class Star {
		public int PosX { get; private set; }
		public int PosY { get; private set; }
		public int VelX { get; }
		public int VelY { get; }

		public Star(string inputLine) {
			// The line is simply the numbers encoded at certain character positions.
			PosX = int.Parse(inputLine.Substring(10, 6));
			PosY = int.Parse(inputLine.Substring(18, 6));
			VelX = int.Parse(inputLine.Substring(36, 2));
			VelY = int.Parse(inputLine.Substring(40, 2));
		}

		public void NextStep() {
			PosX += VelX;
			PosY += VelY;
		}
	}
}
