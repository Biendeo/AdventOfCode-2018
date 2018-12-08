using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Day08 {
	class Node {
		public List<Node> Children;
		public List<int> Metadata;
		public Node Parent { get; }

		public Node() {
			Children = new List<Node>();
			Metadata = new List<int>();
		}

		public Node(Node parent) : this() {
			Parent = parent;
			parent?.Children.Add(this);
		}
		
		public int Sum() {
			return Children.Sum(x => x.Sum()) + Metadata.Sum();
		}

		public int Value() {
			if (Children.Count == 0) {
				return Metadata.Sum();
			} else {
				int value = 0;
				foreach (int x in Metadata) {
					if (x - 1 < Children.Count && x - 1 >= 0) {
						value += Children[x - 1].Value();
					}
				}
				return value;
			}
		}
	}
}
