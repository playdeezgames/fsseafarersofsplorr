namespace Splorr.Seafarers.Models

type AvatarVisit =
    {
        VisitCount : uint32
        LastVisit  : float option
    }

type Island =
    {
        Name         : string
        AvatarVisits : Map<string, AvatarVisit>
        Jobs         : Job list
        Markets      : Map<Commodity, Market>
        Items        : Set<Item>
    }

