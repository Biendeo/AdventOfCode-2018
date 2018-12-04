using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Day4 {
	class Shift {
		public int ID { get; }
		public List<Sleep> Sleeps { get; }
		public int TotalMinutesSlept {
			get {
				return Sleeps.Sum(x => x.Minutes);
			}
		}

		public Shift(int id, List<Sleep> sleeps) {
			ID = id;
			Sleeps = sleeps;
		}
	}
}
