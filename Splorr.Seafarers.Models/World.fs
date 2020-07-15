﻿namespace Splorr.Seafarers.Models

type World =
    {
        Avatars     : Map<string,Avatar>
        Islands     : Map<Location, Island>

        RewardRange : float * float
    }
    