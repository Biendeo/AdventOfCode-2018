using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Day4 {
	class Sleep {
		public DateTime Start { get; }
		public DateTime End { get; }
		public int Minutes { get; }

		public Sleep(DateTime start, DateTime end) {
			Start = start;
			End = end;
			Minutes = (End - Start).Minutes;
		}
	}
}
