#include <fstream>
#include <iostream>
#include <string>
#include <set>

int countUnique(const char* str, int length)
{
	std::set<char> span;
	for (int i = 0; i < length; i++)
		span.insert(str[i]);
	
	return span.size();
}

int firstMarkerOfLength(const char* str, int length)
{
	const char* pos = str;
	while (countUnique(pos++, length) < length);
	return pos - str + length - 1;
}

int main()
{
	std::ifstream file("signal.txt");
	std::string signal;
	getline(file, signal);
	
	// Part 1
	int startOfPacket = firstMarkerOfLength(signal.c_str(), 4);
	std::cout << "The start of the packet is at " << startOfPacket << std::endl;
	
	// Part 2
	int startOfMessage = firstMarkerOfLength(signal.c_str(), 14);
	std::cout << "The start of the message is at " << startOfMessage << std::endl;
}