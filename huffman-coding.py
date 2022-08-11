def sort_freqs(data):
	data.sort()
	occurences = {}
	val = data[0]
	i = 1
	count = 1
	sorted_freqs = []
	while(i < len(data)):
		if data[i] == val:
			count += 1
		else:
			occurences[val] = count
			sorted_freqs = sort_val_freq(val, occurences, sorted_freqs)
			count = 1
			val = data[i]
		i += 1
	occurences[val] = count
	sorted_freqs = sort_val_freq(val, occurences, sorted_freqs)
	return occurences, sorted_freqs

def sort_val_freq(val, occurences, sorted_freqs):
	if len(sorted_freqs) == 0:
		return [val]
	for i, item in enumerate(sorted_freqs):
		if occurences[val] < occurences[item]:
			sorted_freqs.insert(i, val)
	return sorted_freqs

if __name__ == '__main__':
	data = [1, 5, 7, 4, 9, 0, 8, 7, 2, 5, 4, 6, 8, 1, 4, 5, 9, 6, 7, 4, 0]
	occurences, sorted_freqs = sort_freqs(data)
	print(occurences)
	print(sorted_freqs)
	