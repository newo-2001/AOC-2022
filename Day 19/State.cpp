#include "State.hpp"
#include "Blueprint.hpp"
#include "Resource.hpp"
#include <algorithm>

State::State(uint32_t startTime)
    : items(std::array<uint32_t, 4> { 0 }),
    robots(std::array<uint32_t, 3> { 1, 0, 0 }),
    timeRemaining(startTime),
    didntBuy(0)
{
}

State::State(const State& state)
    : items(state.items),
    robots(state.robots),
    timeRemaining(state.timeRemaining - 1),
    didntBuy(0)
{
}

inline uint32_t gaussSum(int32_t n)
{
    return n < 0 ? 0 : n * (n + 1) / 2;
}

bool State::canCraftRobot(Resource robotType, const FactoryBlueprint& factory) const
{
    const RobotBlueprint& recipe = factory.recipes[robotType];
    for (uint32_t resource = Ore; resource <= Obsidian; resource++)
    {
        if (items[resource] < recipe.costs[resource]) return false;
    }
    return true;
}

// An optimistic projection of the total resources that could be collected
// if one of every robot could be made for free every minute
std::array<uint32_t, 3> State::optimisticResources(const FactoryBlueprint& factory) const
{
    std::array<uint32_t, 3> resources = {0};

    // For this to be profitable we need to save atleast
    // one minute to craft a geode robot
    // and one minute for it to crack a geode
    uint32_t time = timeRemaining - 2;

    for (uint32_t resource = Ore; resource <= Obsidian; resource++)
    {
        // If we can't make the robot right now
        // that will cost the robot one minute of productivity
        // It will also take atleast one minute to craft
        uint32_t soonestCraft = time - !canCraftRobot((Resource) resource, factory) - 1;

        resources[resource] = robots[resource] * time + gaussSum(soonestCraft) + items[resource];
    }

    return resources;
}

uint32_t State::optimisticGeodeEstimate(const FactoryBlueprint& factory) const
{
    std::array<uint32_t, 3> projection = optimisticResources(factory);
    const RobotBlueprint& recipe = factory.recipes[Geode];

    // The optimistic maximum amount of geode robots
    // we can make with our optimistic maximum resources
    projection[Ore] /= recipe.costs[Ore];
    projection[Obsidian] /= recipe.costs[Obsidian];
    int32_t maxRobots = std::min(projection[Ore], projection[Obsidian]);

    // If we don't have any clay or obsidian robots,
    // We must first spend atleast one minute crafting these
    // Including one more minute for the geode robot itself
    int32_t earliestGeodeRobot = timeRemaining - 1 - (robots[Clay] == 0) - (robots[Obsidian] == 0);

    // If we can't craft one now we will have to wait atleast an extra minute
    if (!canCraftRobot(Geode, factory)) earliestGeodeRobot--;
    
    //           crafting -> _________________   _____ <- remaining
    // e.g. maxRobots = 5 -> 1 + 2 + 3 + 4 + 5 + 5 + 5
    uint32_t crafting = gaussSum(std::min(earliestGeodeRobot, maxRobots));
    uint32_t remaining = std::max(earliestGeodeRobot - maxRobots, 0) * maxRobots;
   
    return items[Geode] + crafting + remaining;
}

std::vector<Resource> State::craftableRobots(const FactoryBlueprint& factory) const
{
    std::vector<Resource> craftable;

    for (uint32_t robotType = Ore; robotType <= Geode; robotType++)
    {
        // If the robot wasn't bought before even though it was possible,
        // it will always be suboptimal to buy it later
        if ((didntBuy >> robotType) & 1) continue;
        
        if (robotType != Geode)
        {
            uint32_t maxCost = factory.maxCosts[robotType];
            
            // There is no point in making more robots if we already
            // have enough resources to craft the most expensive robot
            // during all the remaining minutes except the last
            if (items[robotType] >= (timeRemaining - 1) * maxCost)
            {
                continue;
            }

            // There is no point in having more robots of a type than
            // resources you can consume of that type per minute
            if (robots[robotType] >= maxCost)
            {
                continue;
            }
        }

        const RobotBlueprint& recipe = factory.recipes[robotType];
        bool affordable = true;

        for (uint32_t resource = Ore; resource <= Obsidian; resource++)
        {
            // If we don't have enough of one of the
            // required resources we can't craft that robot
            if (recipe.costs[resource] > items[resource])
            {
                affordable = false;
                break;
            }
        }

        if (affordable) craftable.push_back((Resource) robotType);
    }

    return craftable;
}

State State::withRobot(const FactoryBlueprint& factory, Resource robotType) const
{
    State newState(*this);

    // We don't store the amount of geode robots
    // because the total produced geodes
    // can immediately be added to the inventory after crafting
    if (robotType == Geode)
    {
        // Time is already decremented in copy-constructor
        newState.items[Geode] += newState.timeRemaining;
    }
    else
    {
        newState.robots[robotType]++;
    }

    const RobotBlueprint& recipe = factory.recipes[robotType];
    for (uint32_t resource = Ore; resource <= Obsidian; resource++)
    {
        newState.items[resource] -= recipe.costs[resource];
    }

    return newState;
}

void State::addResources(const std::array<uint32_t, 3> resources)
{
    for (uint32_t resource = Ore; resource <= Obsidian; resource++)
    {
        items[resource] += resources[resource];
    }
}
