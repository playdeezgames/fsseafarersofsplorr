namespace Splorr.Seafarers.Models

type Island =
    {
        Name           : string
        AvatarVisits   : Map<string, AvatarVisit>
        Jobs           : Job list
        Markets        : Map<uint64, Market>
        Items          : Set<uint64>
        CareenDistance : float
    }

