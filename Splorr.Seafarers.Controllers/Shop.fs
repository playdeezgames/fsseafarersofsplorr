namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Shop = 
    let private RunWithIsland (source:CommandSource) (sink:MessageSink) (location:Location) (island:Island) (world: World) : Gamestate option =
        "" |> sink
        world.Messages
        |> Utility.DumpMessages sink
        let world =
            world
            |> World.ClearMessages
        island.Name |> sprintf "You are at the shop on the island of '%s'." |> sink
        match source() with
        | Some (Command.Buy (quantity, itemName))->
            (Shop, location, world |> World.BuyItems location quantity itemName) 
            |> Gamestate.Docked
            |> Some            

        | Some (Command.Sell (quantity, itemName))->
            (Shop, location, world |> World.SellItems location quantity itemName) 
            |> Gamestate.Docked
            |> Some            

        | Some Command.Dock ->
            (Dock, location, world) 
            |> Gamestate.Docked
            |> Some

        | Some Command.Items ->
            (ItemList, location, world) 
            |> Gamestate.Docked
            |> Some

        | Some Command.Status ->
            (Shop, location, world) 
            |> Gamestate.Docked
            |> Gamestate.Status
            |> Some

        | Some Command.Quit ->
            (Shop, location, world) 
            |> Gamestate.Docked
            |> Gamestate.ConfirmQuit
            |> Some

        | Some Command.Inventory ->
            (Shop, location, world) 
            |> Gamestate.Docked
            |> Gamestate.Inventory
            |> Some

        | Some Command.Help ->
            (Shop, location, world) 
            |> Gamestate.Docked
            |> Gamestate.Help
            |> Some

        | _ -> 
            (Shop, location, world |> World.AddMessages ["Maybe try 'help'?"]) 
            |> Gamestate.Docked
            |> Some

    let Run (source:CommandSource) (sink:MessageSink) (location:Location) (world: World) : Gamestate option =
        match world.Islands |> Map.tryFind location with
        | Some island ->
            RunWithIsland source sink location island world
        | None ->
            world
            |> Gamestate.AtSea
            |> Some
