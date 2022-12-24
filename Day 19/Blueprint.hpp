#pragma once
#include "Resource.hpp"
#include <array>
#include <string>

struct State;

struct RobotBlueprint {
    RobotBlueprint();
    RobotBlueprint(uint32_t ore, uint32_t clay, uint32_t obsidian);
    std::array<uint32_t, 3> costs;
};

class FactoryBlueprint {
public:
    FactoryBlueprint(uint32_t id, std::array<RobotBlueprint, 4> blueprints);
    
    uint32_t id;
    std::array<RobotBlueprint, 4> recipes;
    std::array<uint32_t, 3> maxCosts = {0};
};

FactoryBlueprint parseBlueprint(std::string line);
