namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type ItemListRunWithIslandContext =
    inherit OperatingContext
    inherit AvatarGetPrimaryStatisticContext

type ItemListRunContext =
    inherit ItemListRunWithIslandContext
    inherit DockedRunBoilerplateContext

module ItemList = 
    let private RunWithIsland 
            (context            : ItemListRunWithIslandContext)
            (itemSource         : unit -> Map<uint64, ItemDescriptor>) 
            (islandItemSource   : Location -> Set<uint64>) 
            (messageSink        : MessageSink) 
            (location           : Location) 
            (avatarId           : string) 
            : Gamestate option =
        [
            "" |> Line
            (Hue.Heading, "Item" |> sprintf "%-20s" |> Text) |> Hued
            (Hue.Sublabel, " | " |> Text) |> Hued
            (Hue.Heading, "Selling" |> sprintf "%8s" |> Text) |> Hued
            (Hue.Sublabel, " | " |> Text) |> Hued
            (Hue.Heading, "Offering" |> sprintf "%8s" |> Line) |> Hued
            (Hue.Sublabel, "---------------------+----------+----------" |> Line) |> Hued
        ]
        |> List.iter messageSink
        let items = itemSource()
        location
        |> islandItemSource
        |> Set.iter (fun item -> 
            let descriptor = items.[item]
            let sellPrice: float = (item, location) ||> Item.DetermineSalePrice context
            let buyPrice: float = (item, location) ||> Item.DeterminePurchasePrice context
            [
                (Hue.Value, descriptor.ItemName |> sprintf "%-20s" |> Text) |> Hued
                (Hue.Sublabel, " | " |> Text) |> Hued
                (Hue.Value, sellPrice |> sprintf "%8.3f" |> Text) |> Hued
                (Hue.Sublabel, " | " |> Text) |> Hued
                (Hue.Value, buyPrice |> sprintf "%8.3f" |> Line) |> Hued
            ]
            |> List.iter messageSink
            ())
        [
            (Hue.Label, "Money: " |> Text) |> Hued
            (Hue.Value, avatarId |> Avatar.GetMoney context |> sprintf "%f" |> Line) |> Hued
        ]
        |> List.iter messageSink
        avatarId
        |> Gamestate.InPlay
        |> Some

    let Run 
            (context : ItemListRunContext)
            (avatarMessageSource           : AvatarMessageSource)
            (islandItemSource              : IslandItemSource)
            (islandSource                  : IslandSource)
            (itemSource                    : ItemSource) 
            (messageSink                   : MessageSink) =
        RunWithIsland 
            context
            itemSource 
            islandItemSource 
            messageSink
        |> Docked.RunBoilerplate 
            context
            avatarMessageSource
            islandSource
    

