import numpy as np

class Node:
	def __init__(self, n, frequency=0, children=[]):
		self.n = n
		self.frequency = frequency
		self.children = children
	
	def __eq__(self, other):
		return self.frequency == other.frequency
	
	def __lt__(self, other):
		return self.frequency < other.frequency

	def __gt__(self, other):
		return self.frequency > other.frequency

	def __add__(self, other):
		return Node(-1, self.frequency + other.frequency, [self, other])

	def __repr__(self):
		result = f'Node(n:{self.n}, freqency:{self.frequency}'
		if len(self.children) > 0:
			result += f', children:{self.children}'
		result += ')'
		return result

class Huffman:
	def __init__(self, data):
		self.nodes = []
		freqs = { 0 : 0, 1 : 0, 2 : 0, 3 : 0}
		for val in data:
			freqs[val] += 1
		for val in freqs.keys():
			self.nodes.append(Node(val, freqs[val]))

		print(self.nodes)
		self.build_tree()
		print(self.nodes)

	def build_tree(self):
		while len(self.nodes) > 1:
			self.nodes.sort()
			node1 = self.nodes.pop(0)
			node2 = self.nodes.pop(0)
			self.nodes.append(node1 + node2)

if __name__ == '__main__':
	h = Huffman(np.random.randint(0, 4, 16))