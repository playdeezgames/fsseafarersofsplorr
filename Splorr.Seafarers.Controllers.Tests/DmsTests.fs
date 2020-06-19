module Splorr.Seafarers.Controllers.Tests.Dms

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers

[<Test>]
let ``ToFloat function.Zeros returns 0 radians.`` () =
    let dms: Dms = 
        {
            Degrees = 0
            Minutes = 0
            Seconds = 0.0
        }
    let actual =
        dms
        |> Dms.ToFloat
    Assert.AreEqual(0.0, actual)
    
[<Test>]
let ``ToFloat function.90 degrees returns pi/2 radians.`` () =
    let dms: Dms = 
        {
            Degrees = 90
            Minutes = 0
            Seconds = 0.0
        }
    let actual =
        dms
        |> Dms.ToFloat
    Assert.AreEqual(System.Math.PI/2.0, actual)


[<Test>]
let ``ToFloat function.180 degrees returns pi radians.`` () =
    let dms: Dms = 
        {
            Degrees = 180
            Minutes = 0
            Seconds = 0.0
        }
    let actual =
        dms
        |> Dms.ToFloat
    Assert.AreEqual(System.Math.PI, actual)


[<Test>]
let ``ToFloat function.270 degrees returns 3pi/2 radians.`` () =
    let dms: Dms = 
        {
            Degrees = 270
            Minutes = 0
            Seconds = 0.0
        }
    let actual =
        dms
        |> Dms.ToFloat
    Assert.AreEqual(3.0 * System.Math.PI/2.0, actual)
    
[<Test>]
let ``ToDms function.Zero maps to all zeros.`` () =
    let radians: float = 0.0
    let actual = radians |> Dms.ToDms
    Assert.AreEqual(0, actual.Degrees)
    Assert.AreEqual(0, actual.Minutes)
    Assert.AreEqual(0.0,actual.Seconds)

[<Test>]
let ``ToDms function.PI/2 to 90 degrees.`` () =
    let radians: float = System.Math.PI/2.0
    let actual = radians |> Dms.ToDms
    Assert.AreEqual(90, actual.Degrees)
    Assert.AreEqual(0, actual.Minutes)
    Assert.AreEqual(0.0,actual.Seconds)

[<Test>]
let ``ToDms function.PI to 180 degrees.`` () =
    let radians: float = -System.Math.PI
    let actual = radians |> Dms.ToDms
    Assert.AreEqual(180, actual.Degrees)
    Assert.AreEqual(0, actual.Minutes)
    Assert.AreEqual(0.0,actual.Seconds)

[<Test>]
let ``ToDms function.-PI/2 to 270 degrees.`` () =
    let radians: float = -System.Math.PI/2.0
    let actual = radians |> Dms.ToDms
    Assert.AreEqual(270, actual.Degrees)
    Assert.AreEqual(0, actual.Minutes)
    Assert.AreEqual(0.0,actual.Seconds)

[<Test>]
let ``ToString function.Zeros yields zeros with punctuation marks.``()=
    let expected = "1\u00b02'3.000000\""
    let actual = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
        |> Dms.ToString
    Assert.AreEqual(expected, actual)
