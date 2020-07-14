module VesselTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let vessel =
    {
        Tonnage = 100.0
        Fouling = {MinimumValue=0.0; MaximumValue=0.5; CurrentValue=0.0}
        FoulRate = 0.01
    }

[<Test>]
let ``Create.It creates a vessel.`` () =
    let inputTonnage = 1.0
    let expected =
        {Tonnage = inputTonnage; Fouling = {MinimumValue = 0.0; MaximumValue = 0.5; CurrentValue=0.0}; FoulRate=0.001}
    let actual =
        inputTonnage
        |> Vessel.Create
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Befoul.It increases how fouled the vessel's hull is by the vessel's foul rate.`` () =
    let input = vessel
    let expected =
        {input with
            Fouling = {input.Fouling with CurrentValue=input.FoulRate}}
    let actual =
        input
        |> Vessel.Befoul
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Befoul.It will not increase the vessel's fouling when the vessel is already at maximum fouling.`` () =
    let input = {vessel with Fouling = {vessel.Fouling with CurrentValue = vessel.Fouling.MaximumValue}}
    let expected = input
    let actual =
        input
        |> Vessel.Befoul
    Assert.AreEqual(expected, actual)
