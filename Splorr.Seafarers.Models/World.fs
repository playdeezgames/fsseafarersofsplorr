namespace Splorr.Seafarers.Models

type World =
    {
        AvatarId : string
        Avatars  : Map<string,Avatar>
        Islands  : Map<Location, Island>
    }
    