using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Day4 {
	enum RecordType {
		BeginShift,
		FallAsleep,
		WakeUp
	}

	class Record {
		public DateTime Time { get; }
		public int ID { get; }
		public RecordType Type { get; }

		public Record(string inputLine) {
			// All the input has a date at the beginning so let's find that. I've just used the
			// substrings here since they're easy to match.
			Time = new DateTime(int.Parse(inputLine.Substring(1, 4)), int.Parse(inputLine.Substring(6, 2)), int.Parse(inputLine.Substring(9, 2)), int.Parse(inputLine.Substring(12, 2)), int.Parse(inputLine.Substring(15, 2)), 0);

			// Then we need the type of record. We can simply detect whether a substring is in
			// the line.
			if (inputLine.Contains("begins shift")) {
				Type = RecordType.BeginShift;
				// We also need the ID of the guard.
				ID = int.Parse(inputLine.Substring(26).Split(' ')[0]);
				Console.WriteLine($"Guard #{ID} seen.");
			} else if (inputLine.Contains("falls asleep")) {
				Type = RecordType.FallAsleep;
			} else if (inputLine.Contains("wakes up")) {
				Type = RecordType.WakeUp;
			}
		}
	}
}
