#include "Blueprint.hpp"
#include "State.hpp"
#include "Resource.hpp"
#include <iostream>
#include <fstream>
#include <chrono>
#include <vector>

void search(SearchState& searchState, State& state)
{
    searchState.seenNodes++;

    // If it is theoretically impossible to beat the current record,
    // it would be a fruitless endeavour to try
    if (state.optimisticGeodeEstimate(searchState.factory) <= searchState.mostGeodes)
    {
        return;
    }
    // With one minute remaining,
    // even crafting a geode robot would not grant more geodes
    else if (state.timeRemaining == 1)
    {
        searchState.mostGeodes = std::max(state.items[Geode], searchState.mostGeodes);
        return;
    }

    std::vector<Resource> craftable = state.craftableRobots(searchState.factory);

    // It is always optimal to craft a geode robot
    if (std::count(craftable.begin(), craftable.end(), Geode) > 0)
    {
        State newState = state.withRobot(searchState.factory, Geode);
        newState.addResources(state.robots);
        return search(searchState, newState);
    }

    for (Resource resource : craftable)
    {
        State newState = state.withRobot(searchState.factory, resource);
        newState.addResources(state.robots);
        search(searchState, newState);
    }

    // If we already skipped buying all three robots we don't have to look further
    // No point in saving resources with 2 or fewer minutes remaining
    if ((state.didntBuy & 0b111) != 0b111 && state.timeRemaining > 2)
    {
        State newState(state);
        newState.addResources(state.robots);
        for (Resource resource : craftable)
        {
            newState.didntBuy |= (1 << resource);
        }

        search(searchState, newState);
    }
}

uint32_t geodesInBlueprint(const FactoryBlueprint& blueprint, uint32_t time)
{
    auto start = std::chrono::high_resolution_clock::now();

    State initialState(time);

    SearchState searchState =
    {
        blueprint, 0, 0
    };

    search(searchState, initialState);

    auto stop = std::chrono::high_resolution_clock::now();
    auto duration = std::chrono::duration_cast<std::chrono::milliseconds>(stop - start);

    std::cout << "The max amount of geodes in blueprint " << blueprint.id
        << " is " << searchState.mostGeodes
        << " (" << searchState.seenNodes << " nodes in "
        << duration.count() << "ms)" << std::endl;
    
    return searchState.mostGeodes;
}

std::vector<FactoryBlueprint> parseBlueprints()
{
    std::vector<FactoryBlueprint> blueprints;

    std::ifstream file("blueprints.txt");
    for (std::string line; std::getline(file, line); )
    {
        blueprints.push_back(parseBlueprint(line));
    }

    file.close();
    return blueprints;
}

int main()
{
    std::vector<FactoryBlueprint> blueprints = parseBlueprints();
    
    uint32_t totalQuality = 0;
    for (FactoryBlueprint& blueprint : blueprints)
    {
        uint32_t maxGeodes = geodesInBlueprint(blueprint, 24);
        totalQuality += maxGeodes * blueprint.id;
    }

    std::cout << "The sum of the quality of the blueprints after 24 minutes is " << totalQuality << std::endl << std::endl;
    
    uint32_t totalGeodes = 1;
    for (auto it = blueprints.begin(); it != blueprints.begin() + 3; std::advance(it, 1))
    {
        totalGeodes *= geodesInBlueprint(*it, 32);
    }

    std::cout << "The total number of geodes is of the first three blueprints after 32 minutes is " << totalGeodes << std::endl;

    return 0;
}
