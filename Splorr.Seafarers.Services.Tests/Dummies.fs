module Dummies

open System
open Splorr.Seafarers.Models

let ValidAvatarId = Guid.NewGuid().ToString()
let ValidJobIndex = 1u
let ValidIslandLocation = (1.0,2.0)
let ValidItemName = "valid item name"
let ValidIslandName = "valid island name"
let ValidJob : Job = 
    {
        FlavorText = ""
        Destination = ValidIslandLocation
        Reward = 1.0
    }
let ValidIslandList = [ ValidIslandLocation ]

let InvalidIslandLocation = (-1.0, -2.0)
