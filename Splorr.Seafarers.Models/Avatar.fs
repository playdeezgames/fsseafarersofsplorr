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
        Inventory    : Map<uint64, uint32>
        Shipmates    : Shipmate array
        Metrics      : Map<Metric, uint>
        Vessel       : Vessel
    }