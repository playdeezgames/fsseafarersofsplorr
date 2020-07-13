namespace Splorr.Seafarers.Models

type Metric =
    | Moved = 1

type Avatar =
    {
        Messages     : string list
        Position     : Location
        Heading      : float
        Speed        : float
        ViewDistance : float
        DockDistance : float
        Money        : float
        Reputation   : float
        Job          : Job option
        Inventory    : Map<uint, uint32>
        Satiety      : Statistic
        Health       : Statistic
        Turn         : Statistic
        RationItem   : uint
        Metrics      : Map<Metric, uint>
        Vessel       : Vessel
    }