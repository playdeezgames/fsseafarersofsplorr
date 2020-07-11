namespace Splorr.Seafarers.Models

type World =
    {
        Avatars     : Map<string,Avatar>
        Islands     : Map<Location, Island>

        RewardRange : float * float
        Commodities : Map<uint, CommodityDescriptor>
        Items       : Map<uint, ItemDescriptor>
    }