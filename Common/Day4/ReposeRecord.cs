using Common.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Common.Day4 {
	public class ReposeRecord {
		private List<Record> records;
		private HashSet<int> guardIDs;
		private DateTime FirstTime { get { return records[0].Time; } }
		private DateTime LastTime { get { return records[records.Count - 1].Time; } }

		public ReposeRecord() {
			// This one requires reading in the input line by line. Every line starts with a
			// DateTime (in the form of YYYY-MM-DD hh:mm), and the rest can be determined simply
			// by matching the key words of the message.
			records = new List<Record>();
			guardIDs = new HashSet<int>();
			string input = Resources.Input_Day4;
			using (var reader = new StringReader(input)) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					var record = new Record(line);
					records.Add(record);
					if (record.Type == RecordType.BeginShift) {
						guardIDs.Add(record.ID);
					}
				}
			}

			// The records are not necessarily in order so they need to be sorted.
			records.Sort((a, b) => {
				return a.Time.CompareTo(b.Time);
			});
		}

		public int Part1() {
			// Part 1 requires finding which guard sleeps the most over all of their shifts, and
			// then returning the product of their number and the minute they slept through the
			// most. We can log which minutes each guard sleeps in, and count their overall sleep
			// counts as we scan through the input. Then, we know which guard slept the most, so we
			// just find which minute they slept the most through.

			// Based on the way that I've got the data, it's not easy to parallelize this because
			// you need to know who started the last shift. After that, the solution would be easier
			// using locks. This will require some work in the constructor to present the data
			// better (maybe parsing the records and storing them as a list of shifts (in order),
			// which contain an ID and a list of sleeps, which have a start and end minute.
			var totalSleepTimes = new Dictionary<int, int>();
			var guardSleeps = new Dictionary<int, int[]>();
			foreach (int id in guardIDs) {
				totalSleepTimes.Add(id, 0);
				guardSleeps.Add(id, new int[60]);
			}

			int sleepiestGuard = -1;
			int frequency = 0;

			int currentGuard = -1;
			DateTime lastSleep = DateTime.Now;
			foreach (var record in records) {
				if (record.Type == RecordType.BeginShift) {
					currentGuard = record.ID;
				} else if (record.Type == RecordType.FallAsleep) {
					lastSleep = record.Time;
				} else if (record.Type == RecordType.WakeUp) {
					for (int minute = lastSleep.Minute; minute < record.Time.Minute; ++minute) {
						++guardSleeps[currentGuard][minute];
						++totalSleepTimes[currentGuard];
						if (totalSleepTimes[currentGuard] > frequency) {
							frequency = totalSleepTimes[currentGuard];
							sleepiestGuard = currentGuard;
						}
					}
				}
			}

			int sleepiestMinute = -1;
			frequency = 0;
			for (int minute = 0; minute < 60; ++minute) {
				if (guardSleeps[sleepiestGuard][minute] > frequency) {
					sleepiestMinute = minute;
					frequency = guardSleeps[sleepiestGuard][minute];
				}
			}

			return sleepiestMinute * sleepiestGuard;
		}

		public int Part2() {
			// Part 2 requires finding the the most slept through minute by a single guard. This
			// has a similar solution at first to part 1, but you need to note what their sleepiest
			// minute is as well. Fortunately, you don't need a second pass, so this is easier to
			// solve.

			// Parallelizing suffers from the same issue as part 1, the data is not easy to handle
			// with multiple threads.
			var guardSleeps = new Dictionary<int, int[]>();
			foreach (int id in guardIDs) {
				guardSleeps.Add(id, new int[60]);
			}

			int sleepiestGuard = -1;
			int sleepiestMinute = -1;
			int frequency = 0;

			int currentGuard = -1;
			DateTime lastSleep = DateTime.Now;
			foreach (var record in records) {
				if (record.Type == RecordType.BeginShift) {
					currentGuard = record.ID;
				} else if (record.Type == RecordType.FallAsleep) {
					lastSleep = record.Time;
				} else if (record.Type == RecordType.WakeUp) {
					for (int minute = lastSleep.Minute; minute < record.Time.Minute; ++minute) {
						++guardSleeps[currentGuard][minute];
						if (guardSleeps[currentGuard][minute] > frequency) {
							frequency = guardSleeps[currentGuard][minute];
							sleepiestGuard = currentGuard;
							sleepiestMinute = minute;
						}
					}
				}
			}

			return sleepiestMinute * sleepiestGuard;
		}
	}
}
