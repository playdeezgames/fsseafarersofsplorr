module IslandTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

[<Test>]
let ``Create.It returns a new island.`` () =
    let actual = Island.Create()
    Assert.AreEqual("", actual.Name)

[<Test>]
let ``SetName.It sets the name of a given island to a given name.`` () =
    let name = "Uno"
    let actual = Island.Create() |> Island.SetName name
    Assert.AreEqual(name, actual.Name)

