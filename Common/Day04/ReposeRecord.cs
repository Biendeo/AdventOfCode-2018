using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.Day04 {
	public class ReposeRecord {
		private List<GuardRecord> guardRecords;

		public ReposeRecord() {
			// This one requires reading in the input line by line. Every line starts with a
			// DateTime (in the form of YYYY-MM-DD hh:mm), and the rest can be determined simply
			// by matching the key words of the message.
			var records = new List<Record>();
			string input = Resources.Input_Day04;
			using (var reader = new StringReader(input)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					var record = new Record(line);
					records.Add(record);
				}
			}

			// The records are not necessarily in order so they need to be sorted.
			records.Sort((a, b) => {
				return a.Time.CompareTo(b.Time);
			});

			// Now we convert them into shifts so the info is easy to access.
			var shifts = new Dictionary<int, List<Shift>>();
			int currentID = -1;
			var currentSleeps = new List<Sleep>();
			var lastSleep = DateTime.Now;
			for (int i = 0; i < records.Count; ++i) {
				if (records[i].Type == RecordType.BeginShift) {
					currentID = records[i].ID;
					if (!(shifts.ContainsKey(currentID))) {
						shifts.Add(currentID, new List<Shift>());
					}
					currentSleeps = new List<Sleep>();
				} else if (records[i].Type == RecordType.FallAsleep) {
					lastSleep = records[i].Time;
				} else if (records[i].Type == RecordType.WakeUp) {
					currentSleeps.Add(new Sleep(lastSleep, records[i].Time));
				}

				if (i == records.Count - 1 || records[i + 1].Type == RecordType.BeginShift) {
					shifts[currentID].Add(new Shift(currentID, currentSleeps));
				}
			}

			// And then store them all in one list for easy access and sorting later.
			guardRecords = new List<GuardRecord>();
			foreach (var p in shifts) {
				guardRecords.Add(new GuardRecord(p.Key, p.Value));
			}
		}

		public int Part1() {
			// Part 1 requires finding which guard sleeps the most over all of their shifts, and
			// then returning the product of their number and the minute they slept through the
			// most.

			// My implementation is a bit inefficient but elegant using LINQ. Parallelization is not
			// done on the sort, and this total time slept could be cached when the object is
			// constructed since they're immutable.
			guardRecords.Sort((a, b) => {
				return a.TotalTimeSlept.CompareTo(b.TotalTimeSlept);
			});
			return guardRecords[guardRecords.Count - 1].MostFrequentMinute.Item1 * guardRecords[guardRecords.Count - 1].ID;
		}

		public int Part2() {
			// Part 2 requires finding the most slept through minute by a single guard. This has a
			// similar solution at first to part 1, but you need to note what their sleepiest minute
			// is as well. Fortunately, you don't need a second pass, so this is easier to solve.

			// Paarallelization is the same as before, perform the sort in parallel and cache the
			// values that are used here.
			guardRecords.Sort((a, b) => {
				return a.MostFrequentMinute.Item2.CompareTo(b.MostFrequentMinute.Item2);
			});
			return guardRecords[guardRecords.Count - 1].MostFrequentMinute.Item1 * guardRecords[guardRecords.Count - 1].ID;
		}
	}
}
