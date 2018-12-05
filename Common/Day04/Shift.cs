using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Day04 {
	class Shift {
		public int ID { get; }
		public List<Sleep> Sleeps { get; }
		public int TotalMinutesSlept { get; }

		public Shift(int id, List<Sleep> sleeps) {
			ID = id;
			Sleeps = sleeps;
			TotalMinutesSlept = Sleeps.Sum(x => x.Minutes);
		}
	}
}
