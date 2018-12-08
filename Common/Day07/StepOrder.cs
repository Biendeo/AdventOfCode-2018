using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Day07 {
	class StepOrder {
		public char Before { get; }
		public char After { get; }
		public StepOrder(string line) {
			// The line is the same syntax every time, and the letter is one character, so this
			// can be a really simple index grab.
			Before = line[5];
			After = line[36];
		}

		public StepOrder(char before, char after) {
			Before = before;
			After = after;
		}
	}
}
