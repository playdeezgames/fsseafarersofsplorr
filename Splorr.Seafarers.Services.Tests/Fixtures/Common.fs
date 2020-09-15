namespace Fixtures.Common

open Splorr.Seafarers.Models
open System
open NUnit.Framework

module internal Dummy =
    let internal AvatarId = "avatar"
    let internal BogusAvatarId = "bogus"
    let internal Random = Random()

module internal Stub =
    let internal AdverbSource()          : string list = [ "woefully" ]
    let internal AdjectiveSource()       : string list = [ "tatty" ]
    let internal ObjectNameSource()      : string list = [ "thing" ]
    let internal PersonNameSource()      : string list = [ "george" ]
    let internal PersonAdjectiveSource() : string list = [ "ugly" ]
    let internal ProfessionSource()      : string list = [ "poopsmith" ]
    let internal TermSources = 
        (AdverbSource, AdjectiveSource, ObjectNameSource, PersonNameSource, PersonAdjectiveSource, ProfessionSource)
    let internal NameSource() = []
    let internal ShipmateRationItemSource (_) (_) = [1UL]
    let internal WorldSingleStatisticSource (identifier:WorldStatisticIdentifier) : Statistic =
        match identifier with 
        | WorldStatisticIdentifier.JobReward ->
            {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
        | _ ->
            raise (System.NotImplementedException "Stub.WorldSingleStatisticSource")
    let internal CommoditySource() = 
        Map.empty
        |> Map.add 
            1UL 
            {
                CommodityName=""
                BasePrice=1.0
                PurchaseFactor=1.0
                SaleFactor=1.0
                Discount=0.5
            }
    let internal ItemSource () = 
        Map.empty
        |> Map.add 
            1UL 
            {
                ItemName="item under test"
                Commodities= Map.empty |> Map.add 1UL 1.0
                Occurrence=1.0
                Tonnage = 1.0
            }
    let internal VesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=10.0; CurrentValue=10.0; MaximumValue=10.0} |> Some
        | VesselStatisticIdentifier.FoulRate ->
            {MinimumValue = 0.001; CurrentValue=0.001; MaximumValue=0.001} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue = 0.0; CurrentValue=1.0; MaximumValue=1.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue = 0.0; CurrentValue=0.0; MaximumValue=6.3} |> Some
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue = 0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue = 0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
        | _ ->
            None
    

module internal Fake =
    let internal AvatarMessageSink (_) (_) = 
        raise (NotImplementedException "Fake.AvatarMessageSink")
    let internal ShipmateSingleStatisticSource (_) (_) (_) = 
        raise (NotImplementedException "Fake.ShipmateSingleStatisticSource")
        None
    let internal ShipmateSingleStatisticSink (_) (_) (_) =
        raise (NotImplementedException "Fake.ShipmateSingleStatisticSink")
    let internal AvatarShipmateSource (_) =
        raise (NotImplementedException "Fake.AvatarShipmateSource")
        []
    let internal AvatarSingleMetricSink (_) (actual:Metric * uint64) =
        raise (System.NotImplementedException (sprintf "Fake.AvatarSingleMetricSink - %s %u" ((actual|>fst).ToString()) (actual |> snd)))
    let internal AvatarSingleMetricSource (_) (_) = 
        raise (NotImplementedException "Fake.AvatarSingleMetricSource")
        0UL
    let internal AvatarIslandSingleMetricSink (_) (_) (_) (_) =
        raise (NotImplementedException "Fake.AvatarIslandSingleMetricSink")
    let internal IslandSingleMarketSink (_) (_) =
        raise (NotImplementedException "Fake.IslandSingleMarketSink")
    let internal IslandItemSink (_) (_) =
        raise (NotImplementedException "Fake.IslandItemSink")
    let internal IslandMarketSink (_) (_) =
        raise (NotImplementedException "Fake.IslandMarketSink")
    let internal IslandSingleMarketSource (_) (_) = 
        raise (NotImplementedException "Fake.IslandSingleMarketSource")
        None
    let internal IslandMarketSource (_) = 
        raise (NotImplementedException "Fake.IslandMarketSource")
        Map.empty
    let internal IslandItemSource (_) = 
        raise (NotImplementedException "Fake.IslandItemSource")
        Set.empty
    let internal VesselSingleStatisticSink (_) (_) = 
        raise (NotImplementedException "Fake.VesselSingleStatisticSink")

module internal Mock =
    let internal AvatarMessageSink (expected:string) (_) (actual:string) = 
        Assert.AreEqual(expected, actual)
    let internal AvatarSingleMetricSink (expected:(Metric * uint64) list) (_) (actual:Metric * uint64) =
        let found = 
            expected
            |> List.tryPick
                (fun e -> 
                    if e = actual then
                        Some ()
                    else
                        None)
        if found.IsNone then
            Assert.Fail(sprintf "assertAvatarSingleMetricSink %s %u" ((actual |> fst).ToString()) (actual |> snd))
    let internal AvatarMessagesSink (messages:string list) (_) (message) =
        match messages |> List.tryFind (fun x->x=message) with
        | Some _ ->
            ()
        | None ->
            Assert.Fail(message |> sprintf "Received an invalid message - `%s`.")
    
    
    

    

    
