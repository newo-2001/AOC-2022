#include <fstream>
#include <iostream>
#include <string>
#include <list>
#include <vector>

struct Move {
    int count;
    int from;
    int to;
};

class Crates
{
public:
    Crates(const std::vector<std::list<char>>& crates)
    {
        m_crates = std::vector<std::list<char>>();
        m_crates.reserve(crates.size());

        for (auto& stack : crates)
            m_crates.push_back(std::list<char>(stack));
    }

    void move(const Move& move)
    {
        for (size_t i = 0; i < move.count; i++)
        {
            m_crates[move.to].push_front(m_crates[move.from].front());
            m_crates[move.from].pop_front();
        }
    }

    void moveTogether(const Move& move)
    {
        auto& from = m_crates[move.from];
        auto& to = m_crates[move.to];
        auto end = from.begin();
        std::advance(end, move.count);

        to.splice(to.begin(), from, from.begin(), end);
    }

    void printTop() const
    {
        std::string topCrates;
        for (auto& stack : m_crates)
        {
            if (stack.empty()) topCrates.push_back(' ');
            else topCrates.push_back(stack.front());
        }

        std::cout << "The top crates are: " << topCrates << std::endl;
    }
private:
    std::vector<std::list<char>> m_crates;
};

Move parseMove(std::string& line)
{
    Move move {};
    size_t pos = 0;
    size_t i = 0;
    std::string token;
    while ((pos = line.find(' ')) != std::string::npos)
    {
        token = line.substr(0, pos);
        switch (i++)
        {
            case 1:
                move.count = stoi(token);
                break;
            case 3:
                move.from = stoi(token) - 1;
                break;
        }
        line.erase(0, pos + 1);
    }
    move.to = stoi(line) - 1;

    return move;
}

void parseData(std::vector<std::list<char>>& crates, std::vector<Move>& moves)
{
    bool parsingMoves = false;

    std::ifstream file("crates.txt");
    std::string token;
    while (!file.eof())
    {
        getline(file, token);
        if (parsingMoves)
        {
            moves.push_back(parseMove(token));
            continue;
        }
        
        if (token.find('[') == std::string::npos)
        {
            parsingMoves = true;
            getline(file, token);
            continue;
        }
    
        size_t pos = 0;
        size_t offset = 0;
        while ((pos = token.find('[')) != std::string::npos)
        {
            offset += pos + 1;
            size_t index = offset / 4;
            for (size_t i = crates.size(); i <= index; i++)
                crates.push_back(std::list<char>());
            crates[index].push_back(token[pos+1]);
            token.erase(0, pos + 1);
        }
    }
}

int main()
{
    std::vector<std::list<char>> parsedCrates;
    std::vector<Move> moves;
    parseData(parsedCrates, moves);
    
    // Part 1
    Crates crates(parsedCrates);
    for (Move& move : moves)
        crates.move(move);
    crates.printTop();

    // Part 2
    crates = Crates(parsedCrates);
    for (Move& move : moves)
        crates.moveTogether(move);
    crates.printTop();
}