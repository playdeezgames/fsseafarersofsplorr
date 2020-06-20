module DmsTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers

[<Test>]
let ``ToFloat.It returns 0 when given zeros.`` () =
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
let ``ToFloat.It returns half pi when given ninety degrees.`` () =
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
let ``ToFloat.It returns pi when given one hundred eighty degrees.`` () =
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
let ``ToFloat.It returns three halves pi when given two hundred seventy degrees.`` () =
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
let ``ToDms.It return all zero when given zero.`` () =
    let radians: float = 0.0
    let actual = radians |> Dms.ToDms
    Assert.AreEqual(0, actual.Degrees)
    Assert.AreEqual(0, actual.Minutes)
    Assert.AreEqual(0.0,actual.Seconds)

[<Test>]
let ``ToDms.It returns ninety degrees when given half pi.`` () =
    let radians: float = System.Math.PI/2.0
    let actual = radians |> Dms.ToDms
    Assert.AreEqual(90, actual.Degrees)
    Assert.AreEqual(0, actual.Minutes)
    Assert.AreEqual(0.0,actual.Seconds)

[<Test>]
let ``ToDms.It returns one hundred eighty degrees when given pi.`` () =
    let radians: float = -System.Math.PI
    let actual = radians |> Dms.ToDms
    Assert.AreEqual(180, actual.Degrees)
    Assert.AreEqual(0, actual.Minutes)
    Assert.AreEqual(0.0,actual.Seconds)

[<Test>]
let ``ToDms.It return two hundred seventy degrees when give three halves pi.`` () =
    let radians: float = -System.Math.PI/2.0
    let actual = radians |> Dms.ToDms
    Assert.AreEqual(270, actual.Degrees)
    Assert.AreEqual(0, actual.Minutes)
    Assert.AreEqual(0.0,actual.Seconds)

[<Test>]
let ``ToString.It formats degrees minutes and seconds.``()=
    let expected = "1\u00b02'3.000000\""
    let actual = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
        |> Dms.ToString
    Assert.AreEqual(expected, actual)
