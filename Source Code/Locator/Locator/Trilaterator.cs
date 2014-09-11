using System;
using System.Collections.Generic;

namespace Locator {

	class Trilaterator {
		private const int MAX_NODES = 3;

		private List<Node> Nodes;
		private Node Tag;
		private int Index;

		public Trilaterator() {
			Nodes = new List<Node>();
			Tag = new Node();
			Index = 0;

			for (int i = 0; i < MAX_NODES; i++) {
				Nodes.Add(new Node());
			}
		}

		public void AddNode(double nodeX, double nodeY) {
			if (Index >= MAX_NODES) {
				Console.WriteLine("[Warning] Tried to add more than 3 nodes. Overwriting a previous one.");
				Index %= MAX_NODES;
			}
			
			Node node = Nodes[Index];
			node.X = nodeX;
			node.Y = nodeY;

			Nodes[Index++] = node;
		}

		/**
		 * The only reason we store the position of the tag is to obtain mock
		 * signal strengths for it. The trilateration itself only needs the
		 * positions of the nodes and the signal strengths to each node.
		 */
		public void MoveTag(double newX, double newY) {
			Tag.X = newX;
			Tag.Y = newY;
		}

		public double[] Locate() {
			double str1 = GetSignalStrength(Nodes[0]);
			double str2 = GetSignalStrength(Nodes[1]);
			double str3 = GetSignalStrength(Nodes[2]);

			return Run(str1, str2, str3);
		}

		public void Print() {
			string output = "";

			for (int i = 0; i < MAX_NODES; i++) {
				output += "Node " + (i + 1) + " = (" + Nodes[i].X + ", " + Nodes[i].Y + ")\n";
			}

			output += "Tag = (" + Tag.X + ", " + Tag.Y + ")";

			Console.WriteLine(output);
		}

		/**
		 * This is where the magic happens. Trilateration in 2 (very long) lines!
		 * See here: http://stackoverflow.com/questions/17889765/triangulation-algorithm-on-grid-with-signal-strength-c-sharp
		 */
		private double[] Run(double str1, double str2, double str3) {
			var px = ((str1 * str1) - (str2 * str2) + (Nodes[1].X * Nodes[1].X)) / (2.0 * Nodes[1].X);
			var py = ((str1 * str1) - (str3 * str3) + (Nodes[2].X * Nodes[2].X) + (Nodes[2].Y * Nodes[2].Y)) / (2.0 * Nodes[2].Y) - (Nodes[2].X / Nodes[2].X) * px;

			return new double[] { px, py };
		}

		private double GetSignalStrength(Node node) {
			double xd = node.X - Tag.X;
			double yd = node.Y - Tag.Y;

			var signalStrength = Math.Sqrt((xd * xd) + (yd * yd));

			return -signalStrength;
		}
	}

	struct Node {
		public double X, Y;
	}
}
