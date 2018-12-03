using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Day3 {
	class Claim {
		public int ID { get; }
		public int LeftMargin { get; }
		public int TopMargin { get; }
		public int Width { get; }
		public int Height { get; }

		public Claim(string inputLine) {
			var regex = new Regex("#([0-9]+) @ ([0-9]+),([0-9]+): ([0-9]+)x([0-9]+)");
			var match = regex.Match(inputLine);
			ID = int.Parse(match.Groups[1].Value);
			LeftMargin = int.Parse(match.Groups[2].Value);
			TopMargin = int.Parse(match.Groups[3].Value);
			Width = int.Parse(match.Groups[4].Value);
			Height = int.Parse(match.Groups[5].Value);
		}
	}
}
