using Common.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Day08 {
	/// <summary>
	/// The main class for the day 8 challenge.
	/// </summary>
	public class MemoryManeuver {
		private Node headNode;

		private static void InitializeNodes(List<int> inputNumbers, ref int currentIndex, Node currentNode) {
			int numChildren = inputNumbers[currentIndex++];
			int numMetadata = inputNumbers[currentIndex++];
			for (int i = 0; i < numChildren; ++i) {
				var newNode = new Node(currentNode);
				InitializeNodes(inputNumbers, ref currentIndex, newNode);
			}
			for (int i = 0; i < numMetadata; ++i) {
				currentNode.Metadata.Add(inputNumbers[currentIndex++]);
			}
		}

		public MemoryManeuver() {
			// To initialize the object, the input is simply a list of space separated integers,
			// which can easily be done with a mapping.
			string input = Resources.Input_Day08;
			var inputNumbers = new List<int>(input.Split(' ').ToList().Select(x => int.Parse(x)));

			headNode = new Node();

			// Parsing the data is easy with a referenced variable and a recursive function.
			int index = 0;
			InitializeNodes(inputNumbers, ref index, headNode);
		}

		public int Part1() {
			// For part 1, simply sum each node's metadata elements, which can be done simply with
			// recursive calls.

			// For parallelization, it's not that easy with the current implementation (and
			// considering how natural the recursive solution is, it's probably not necessary).
			return headNode.Sum();
		}
		
		public int Part2() {
			// For part 2, instead, if a node has children, its metadata refers to the index of the
			// child, and is recursively calling that function instead. 

			// Again, same boat for parallelization. Performance could be improved by caching these
			// values though, as it is very likely to repeat calculating some child nodes.
			return headNode.Value();
		}
	}
}
