namespace Huffman {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class Node {
        public int n {get; set;} = -1;
        public int frequency {get; set;} = 0;
        public List<Node> children {get; set;} = new List<Node>();

        public override string ToString()
        {
            string result = $"Node(n:{this.n}, frequency:{this.frequency}";
            if(this.children.Count > 0) {
                result += $", children:[{this.children[0]}, {this.children[1]}]";
            }
            return result + ")";
        }
    }

    public class HuffmanTree {
        List<Node> nodes = new List<Node>();

        public HuffmanTree(int[] data) {
            var freqs = new Dictionary<int, int>();
            foreach(int val in data) {
                if(!freqs.Keys.Contains(val)) {
                    freqs[val] = 1;
                    continue;
                }
                freqs[val]++;
            }
            foreach(int val in freqs.Keys) {
                nodes.Add(new Node{ n = val, frequency = freqs[val] });
            }

            Node tree = BuildTree();

            Console.WriteLine(tree);
        }

        private Node BuildTree() {
            var newNodes = nodes;
            while(newNodes.Count > 1) {
                newNodes = newNodes.OrderBy(node => node.frequency).ToList();
                Node node1 = newNodes[0];
                newNodes.RemoveAt(0);
                Node node2 = newNodes[0];
                newNodes.RemoveAt(0);
                newNodes.Add(new Node{ n = -1, frequency = node1.frequency + node2.frequency, children = new List<Node> { node1, node2 } });
            }
            return newNodes[0];
        }
    }
}