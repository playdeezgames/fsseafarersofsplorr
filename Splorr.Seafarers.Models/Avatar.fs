namespace Splorr.Seafarers.Models

type Avatar =
    {
        Position: Location
        Heading: float
        Speed: float
        ViewDistance: float
        DockDistance: float
        Money: float
        Reputation: float
        Job: Job option
        Inventory: Map<Item, uint32>
        Satiety: Statistic
        Health: Statistic
    }