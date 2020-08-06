namespace Splorr.Seafarers.Models

type Avatar =
    {
        Messages     : string list//TODO: associate directly to avatarId
        Position     : Location//TODO: make this a Vessel statistics
        Heading      : float//TODO: make this a Vessel statistic - min 0, max 2pi
        Money        : float//TODO: make this a Shipmate statistic - min 0, max 1.7976931348623157e308
        Reputation   : float//TODO: make this a Shipmate statistic - min -1.7976931348623157e308, max 1.7976931348623157e308
        Job          : Job option //TODO: associate directly to avatarId
        Inventory    : Map<uint64, uint32> //TODO: gets its own table
        Shipmates    : Shipmate array //TODO: gets its own set of tables associate to avatarId
        Metrics      : Map<Metric, uint> //TODO: gets its own table associate to avatarId
    }