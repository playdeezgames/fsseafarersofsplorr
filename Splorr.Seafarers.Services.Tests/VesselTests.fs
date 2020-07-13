module VesselTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

[<Test>]
let ``Create.It creates a vessel.`` () =
    let inputTonnage = 1.0
    let expected =
        {Tonnage = inputTonnage}
    let actual =
        inputTonnage
        |> Vessel.Create
    Assert.AreEqual(expected, actual)
