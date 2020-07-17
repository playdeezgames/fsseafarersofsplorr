namespace Splorr.Seafarers.Models

type Metric =
    | Moved         = 1
    | Ate           = 2
    | VisitedIsland = 3
    | CompletedJob  = 4
    | AbandonedJob  = 5
    | AcceptedJob   = 6
    | CleanedHull   = 7

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
        Inventory    : Map<uint64, uint32>
        Satiety      : Statistic
        Health       : Statistic
        Turn         : Statistic
        RationItem   : uint64
        Metrics      : Map<Metric, uint>
        Vessel       : Vessel
    }