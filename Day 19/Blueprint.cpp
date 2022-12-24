#include "Blueprint.hpp"
#include <sstream>

FactoryBlueprint::FactoryBlueprint(uint32_t id, std::array<RobotBlueprint, 4> blueprints)
    : id(id), recipes(blueprints), maxCosts({0})
{
    for (RobotBlueprint& recipe : blueprints)
    {
        for (uint32_t resource = Ore; resource <= Obsidian; resource++)
        {
            maxCosts[resource] = std::max(recipe.costs[resource], maxCosts[resource]);
        }
    }
}

RobotBlueprint::RobotBlueprint(uint32_t ore, uint32_t clay, uint32_t obsidian)
    : costs(std::array<uint32_t, 3> { ore, clay, obsidian })
{
}

RobotBlueprint::RobotBlueprint()
    : costs(std::array<uint32_t, 3> { 0 })
{
}

std::stringstream& skip(std::stringstream& stream, std::string& str, uint32_t words)
{
    for (uint32_t i = 0; i < words; i++)
    {
        stream >> str;
    }
    return stream;
}

FactoryBlueprint parseBlueprint(std::string line)
{
    std::array<RobotBlueprint, 4> recipes;

    uint32_t id, oreCost, otherCost;
    std::stringstream stream(line);
    std::string word;

    stream >> word >> id >> word;
    skip(stream, word, 4) >> oreCost;
    recipes[0] = RobotBlueprint(oreCost, 0, 0);
    skip(stream, word, 5) >> oreCost;
    recipes[1] = RobotBlueprint(oreCost, 0, 0);
    skip(stream, word, 5) >> oreCost >> word >> word >> otherCost;
    recipes[2] = RobotBlueprint(oreCost, otherCost, 0);
    skip(stream, word, 5) >> oreCost >> word >> word >> otherCost;
    recipes[3] = RobotBlueprint(oreCost, 0, otherCost);

    return FactoryBlueprint(id, recipes);
}
