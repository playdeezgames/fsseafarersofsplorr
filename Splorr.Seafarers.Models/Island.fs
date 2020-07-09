namespace Splorr.Seafarers.Models

type Island =
    {
        Name       : string
        VisitCount : uint32 option
        LastVisit  : float option
        Jobs       : Job list
        Markets    : Map<Commodity, Market>
        Items      : Set<Item>
    }

