namespace Huffman {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Node {
        public int n {get; set;} = -1; // Value of node, -1 if frequency only (no value)
        public int frequency {get; set;} = 0; // Frequency of appearance of value in data set
        public List<Node> children {get; set;} = new List<Node>(); // Child nodes which make it a tree

        public override string ToString()
        {
            // Formats Node when printing for easy debugging
            string result = $"Node(n:{this.n}, frequency:{this.frequency}";
            if(this.children.Count > 0) {
                result += $", children:[{this.children[0]}, {this.children[1]}]";
            }
            return result + ")";
        }
    }

    public class HuffmanTree {
        List<Node> nodes = new List<Node>(); // Original nodes created from data

        public HuffmanTree(int[] data) {
            var freqs = new Dictionary<int, int>(); // Frequency dict for values -- Key: data value, Value: frequency of appearance
            foreach(int val in data) {
                if(!freqs.Keys.Contains(val)) {
                    freqs[val] = 1; // Add value to dict if not already in
                    continue;
                }
                freqs[val]++;
            }
            foreach(int val in freqs.Keys) {
                nodes.Add(new Node{ n = val, frequency = freqs[val] }); // Create node for each value node from data
            }

            Node tree = BuildTree();

            Console.WriteLine(tree);
        }

        private Node BuildTree() {
            /*  Creates tree based on value frequency
                using Nodes and assigning children
                Returns: Node, children create tree
            */
            var newNodes = this.nodes; // copy this.nodes in case we need to access original nodes
            while(newNodes.Count > 1) { // loop until one node remains (to return)
                newNodes = newNodes.OrderBy(node => node.frequency).ToList(); // Sorts nodes based on frequency
                Node node1 = newNodes[0]; // pop 0
                newNodes.RemoveAt(0);
                Node node2 = newNodes[0]; // pop 0
                newNodes.RemoveAt(0);
                newNodes.Add(new Node{ n = -1, frequency = node1.frequency + node2.frequency, children = new List<Node> { node1, node2 } }); // node1/node2 replaced by new node using sum of frequencies + them as children
            }
            return newNodes[0];
        }
    }
}