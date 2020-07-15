namespace Splorr.Seafarers.Models

type AvatarVisit =
    {
        VisitCount : uint32
        LastVisit  : float option
    }

type Island =
    {
        Name           : string
        AvatarVisits   : Map<string, AvatarVisit>
        Jobs           : Job list
        Markets        : Map<uint64, Market>
        Items          : Set<uint64>
        CareenDistance : float
    }

