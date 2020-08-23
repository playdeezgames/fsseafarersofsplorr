namespace Splorr.Seafarers.Models

type IslandStatisticIdentifier =
    | CareenDistance = 1

type IslandStatisticTemplate = StatisticTemplate

type Island =
    {
        Jobs           : Job list
        CareenDistance : float //TODO: make into islandstatisticidentifier
    }

