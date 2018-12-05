using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Common.Day04 {
	class GuardRecord {
		public int ID { get; }
		public List<Shift> Shifts { get; }
		public int TotalTimeSlept { get; }
		public Tuple<int, int> MostFrequentMinute { get; }

		public GuardRecord(int id, List<Shift> shifts) {
			ID = id;
			Shifts = shifts;
			TotalTimeSlept = Shifts.Sum(x => x.TotalMinutesSlept);
			int[] minuteFrequency = new int[60];
			Shifts.AsParallel().ForAll(shift => {
				shift.Sleeps.ForEach(sleep => {
					for (int minute = sleep.Start.Minute; minute < sleep.End.Minute; ++minute) {
						Interlocked.Increment(ref minuteFrequency[minute]);
					}
				});
			});
			MostFrequentMinute = new Tuple<int, int>(minuteFrequency.ToList().IndexOf(minuteFrequency.Max()), minuteFrequency.Max());
		}
	}
}
