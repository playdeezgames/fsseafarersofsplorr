module VesselTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let vessel =
    {
        Tonnage = 100.0
        Fouling = 
            Map.empty
            |> Map.add Port {MinimumValue=0.0; MaximumValue=0.25; CurrentValue=0.0}
            |> Map.add Starboard {MinimumValue=0.0; MaximumValue=0.25; CurrentValue=0.0}
        FoulRate = 0.01
    }

[<Test>]
let ``Create.It creates a vessel.`` () =
    let inputTonnage = 1.0
    let expected =
        {
            Tonnage = inputTonnage
            Fouling = 
                Map.empty
                |> Map.add Port {MinimumValue = 0.0; MaximumValue = 0.25; CurrentValue=0.0}
                |> Map.add Starboard {MinimumValue = 0.0; MaximumValue = 0.25; CurrentValue=0.0}
            FoulRate=0.001
        }
    let actual =
        inputTonnage
        |> Vessel.Create
    Assert.AreEqual(expected, actual)

[<Test>]
let ``TransformFouling.It transforms fouling on the port side when the port side is specified.`` () =
    let input = vessel
    let inputSide = Port
    let expected =
        {input with
            Fouling =
                input.Fouling
                |> Map.add inputSide {input.Fouling.[inputSide] with CurrentValue = 0.25}}
    let actual =
        input
        |> Vessel.TransformFouling inputSide (Statistic.ChangeCurrentBy 0.25)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``TransformFouling.It transforms fouling on the starboard side when the starboard side is specified.`` () =
    let input = vessel
    let inputSide = Starboard
    let expected =
        {input with
            Fouling =
                input.Fouling
                |> Map.add inputSide {input.Fouling.[inputSide] with CurrentValue = 0.25}}
    let actual =
        input
        |> Vessel.TransformFouling inputSide (Statistic.ChangeCurrentBy 0.25)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Befoul.It increases how fouled the vessel's hull is by the vessel's foul rate.`` () =
    let input = vessel
    let expected =
        input
        |> Vessel.TransformFouling Port (Statistic.ChangeCurrentBy (input.FoulRate/2.0))
        |> Vessel.TransformFouling Starboard (Statistic.ChangeCurrentBy (input.FoulRate/2.0))
    let actual =
        input
        |> Vessel.Befoul
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Befoul.It will not increase the vessel's fouling when the vessel is already at maximum fouling.`` () =
    let input = 
        vessel
        |> Vessel.TransformFouling Port (fun x -> {x with CurrentValue = x.MaximumValue})
        |> Vessel.TransformFouling Starboard (fun x -> {x with CurrentValue = x.MaximumValue})
    let expected = input
    let actual =
        input
        |> Vessel.Befoul
    Assert.AreEqual(expected, actual)
