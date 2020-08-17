module CommonTestFixtures

open Splorr.Seafarers.Controllers
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System
open NUnit.Framework

let internal connectionString = 
    "Data Source=:memory:;Version=3;New=True;"

let internal random = 
    Random()

let internal sinkStub 
        (_ : Message) 
        : unit = 
    ()

let internal toSource 
        (command:Command option) 
        : unit -> Command option= 
    fun () -> command

let internal createConnection () : SQLiteConnection =
    new SQLiteConnection(connectionString)

let internal avatarId : string = ""

let internal statisticDescriptors =
    [
        {
            StatisticId = ShipmateStatisticIdentifier.Satiety
            StatisticName="satiety"
            MinimumValue=0.0
            CurrentValue=100.0
            MaximumValue=100.0
        }
        {
            StatisticId = ShipmateStatisticIdentifier.Health
            StatisticName="health"
            MinimumValue=0.0
            CurrentValue=100.0
            MaximumValue=100.0
        }
        {
            StatisticId = ShipmateStatisticIdentifier.Turn
            StatisticName="turn"
            MinimumValue=0.0
            CurrentValue=0.0
            MaximumValue=50000.0
        }
        {
            StatisticId = ShipmateStatisticIdentifier.Money
            StatisticName="money"
            MinimumValue=0.0
            CurrentValue=0.0
            MaximumValue=1000000000.0
        }
    ]
let internal shipmateStatisticTemplateSourceStub () =
    statisticDescriptors
    |> List.map (fun x -> (x.StatisticId, x))
    |> Map.ofList
let internal vesselStatisticTemplateSourceStub () 
        : Map<VesselStatisticIdentifier, VesselStatisticTemplate>= 
    Map.empty

let internal vesselStatisticSinkStub (_) (_) = 
    ()

let internal vesselSingleStatisticSourceStub (_) (identifier: VesselStatisticIdentifier) = 
    match identifier with 
    | VesselStatisticIdentifier.FoulRate ->
        {MinimumValue=0.001; MaximumValue=0.001; CurrentValue=0.001} |> Some
    | VesselStatisticIdentifier.Tonnage ->
        {MinimumValue=100.0; MaximumValue=100.0; CurrentValue=100.0} |> Some
    | VesselStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0} |> Some
    | VesselStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0} |> Some
    | VesselStatisticIdentifier.ViewDistance ->
        {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
    | VesselStatisticIdentifier.DockDistance ->
        {MinimumValue=1.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
    | VesselStatisticIdentifier.Speed ->
        {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
    | VesselStatisticIdentifier.Heading ->
        {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
    | _ ->
        None

let internal vesselSingleStatisticSinkStub (_) (_) = ()

let internal avatarMessageSourceStub (_) = []
let internal avatarMessageSinkStub (_) (_) = ()
let internal avatarMessagePurgerStub (_) = ()

let avatarExpectedMessagesSink (expected:string list) (_) (actual:string) : unit =
    match expected |> List.tryFind (fun x -> x = actual) with
    | Some _ ->
        Assert.Pass ("Valid message received.")
    | _ ->
        Assert.Fail ("Invalid Message Received")


let internal adverbSource()          : string list = [ "woefully" ]
let internal adjectiveSource()       : string list = [ "tatty" ]
let internal objectNameSource()      : string list = [ "thing" ]
let internal personNameSource()      : string list = [ "george" ]
let internal personAdjectiveSource() : string list = [ "ugly" ]
let internal professionSource()      : string list = [ "poopsmith" ]
let internal termSources = 
    (adverbSource, adjectiveSource, objectNameSource, personNameSource, personAdjectiveSource, professionSource)

let internal termNameSource() = []

let internal shipmateRationItemSourceStub (_) (_) = [ 1UL ]
let internal shipmateRationItemSinkStub (_) (_) (_) = ()

let internal rationItemSourceStub () = [ 1UL ]

let internal worldSingleStatisticSourceStub (identifier: WorldStatisticIdentifier) : Statistic =
    match identifier with 
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "worldSingleStatisticSourceStub")

let internal shipmateSingleStatisticSinkStub (_) (_) (_) =
    ()

let internal shipmateSingleStatisticSourceStub (_) (_) (identifier:ShipmateStatisticIdentifier) =
    match identifier with
    | ShipmateStatisticIdentifier.Satiety
    | ShipmateStatisticIdentifier.Health ->
        Statistic.Create (0.0, 100.0) 100.0 |> Some
    | ShipmateStatisticIdentifier.Turn ->
        Statistic.Create (0.0, 50000.0) 0.0 |> Some
    | ShipmateStatisticIdentifier.Money ->
        Statistic.Create (0.0, 1000000.0) 0.0 |> Some
    | ShipmateStatisticIdentifier.Reputation ->
        Statistic.Create (-1000.0, 1000.0) 0.0 |> Some
    | _ -> 
        raise (System.NotImplementedException (identifier.ToString() |> sprintf "shipmateSingleStatisticSourceStub %s"))
        None
    

let internal avatarShipmateSourceStub (_) =
    [Primary]

let internal avatarInventorySourceStub (_) =
    Map.empty
let internal avatarInventorySinkStub (_) (_) =
    ()