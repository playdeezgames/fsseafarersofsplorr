namespace Splorr.Seafarers.Models

type World =
    {
        Turn: uint32
        Messages: string list
        Avatar: Avatar
        Islands: Map<Location, Island>
    }