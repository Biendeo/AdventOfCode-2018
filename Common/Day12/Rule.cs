using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Day12 {
	class Rule {
		public string Pattern { get; }
		public bool WillHavePlant { get; }

		public Rule(string input) {
			Pattern = input.Substring(0, 5);
			WillHavePlant = input[9] == '#';
		}
	}
}
