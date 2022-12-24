#pragma once
#include "Resource.hpp"
#include <array>
#include <vector>

class FactoryBlueprint;

struct State
{
    State(uint32_t startTime);
    State(const State& state);
    void addResources(const std::array<uint32_t, 3> resources);
    uint32_t optimisticGeodeEstimate(const FactoryBlueprint& factory) const;
    State withRobot(const FactoryBlueprint& factory, Resource robotType) const;
    std::vector<Resource> craftableRobots(const FactoryBlueprint& factory) const;
    bool canCraftRobot(Resource robotType, const FactoryBlueprint& factory) const;
    
    std::array<uint32_t, 4> items;
    std::array<uint32_t, 3> robots;
    uint32_t timeRemaining;
    uint8_t didntBuy;
private:
    std::array<uint32_t, 3> optimisticResources(const FactoryBlueprint& factory) const;
};

struct SearchState
{
    const FactoryBlueprint& factory;
    uint32_t mostGeodes;
    uint64_t seenNodes;
};
