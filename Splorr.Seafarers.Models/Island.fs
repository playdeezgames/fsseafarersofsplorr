namespace Splorr.Seafarers.Models

type Island =
    {
        Name           : string
        AvatarVisits   : Map<string, AvatarVisit>
        Jobs           : Job list
        CareenDistance : float //TODO: make into islandstatisticidentifier
    }

