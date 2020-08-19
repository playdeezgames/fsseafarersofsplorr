namespace Splorr.Seafarers.Models

type World =
    {
        AvatarId : string
        Islands  : Map<Location, Island>
    }
    