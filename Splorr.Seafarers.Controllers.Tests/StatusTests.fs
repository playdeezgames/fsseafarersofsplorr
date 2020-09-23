module StatusTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open Splorr.Seafarers.Services

let private previousGameState =
    None
    |> Gamestate.MainMenu

type TestStatusRunContext(avatarJobSource, islandSingleNameSource, shipmateSingleStatisticSource, vesselSingleStatisticSource) =
    interface StatusRunContext with
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = previousGameState
    let expected = input |> Some
    let context = 
        TestStatusRunContext
            (avatarJobSourceStub, 
            islandSingleNameSourceStub, 
            shipmateSingleStatisticSourceStub, 
            vesselSingleStatisticSourceStub) 
            :> StatusRunContext
    let actual =
        input
        |> Status.Run 
            context
            //avatarJobSourceStub
            //islandSingleNameSourceStub
            //shipmateSingleStatisticSourceStub
            //vesselSingleStatisticSourceStub
            sinkDummy
    Assert.AreEqual(expected, actual)

