namespace Splorr.Seafarers.Models

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
    }