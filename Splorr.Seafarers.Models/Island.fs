namespace Splorr.Seafarers.Models

type Island =
    {
        Name           : string
        AvatarVisits   : Map<string, AvatarVisit>
        Jobs           : Job list
        Markets        : Map<uint64, Market>
        CareenDistance : float
    }

