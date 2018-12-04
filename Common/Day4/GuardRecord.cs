using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.Day4 {
	class GuardRecord {
		public int ID { get; }
		public List<Shift> Shifts { get; }
		public int TotalTimeSlept {
			get {
				return Shifts.Sum(x => x.TotalMinutesSlept);
			}
		}
		public Tuple<int, int> MostFrequentMinute {
			get {
				int[] minuteFrequency = new int[60];
				Shifts.AsParallel().ForAll(shift => {
					shift.Sleeps.ForEach(sleep => {
						for (int minute = sleep.Start.Minute; minute < sleep.End.Minute; ++minute) {
							Interlocked.Increment(ref minuteFrequency[minute]);
						}
					});
				});
				return new Tuple<int, int>(minuteFrequency.ToList().IndexOf(minuteFrequency.Max()), minuteFrequency.Max());
			}
		}

		public GuardRecord(int id, List<Shift> shifts) {
			ID = id;
			Shifts = shifts;
		}
	}
}
