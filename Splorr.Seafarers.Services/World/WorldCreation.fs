namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module WorldCreation =
    type IslandSingleNameSink = Location -> string option -> unit
    type IslandFeatureGeneratorSource = unit -> Map<IslandFeatureIdentifier, IslandFeatureGenerator>
    type IslandSingleFeatureSink = Location -> IslandFeatureIdentifier -> unit


    let private GenerateIslandName
            (context: CommonContext)
            : string =
        let consonants = [ "h"; "k"; "l"; "m"; "p" ]
        let vowels = [ "a"; "e"; "i"; "o"; "u" ]
        let vowel = Utility.PickFromListRandomly context [true; false]
        let nameLength =
            Map.empty
            |> Map.add 3 1.0
            |> Map.add 4 3.0
            |> Map.add 5 6.0
            |> Map.add 6 7.0
            |> Map.add 7 6.0
            |> Map.add 8 3.0
            |> Map.add 9 1.0
            |> Utility.GenerateFromWeightedValues context
            |> Option.get
        [1..(nameLength)]
        |> List.map 
            (fun i -> i % 2 = (if vowel then 1 else 0))
        |> List.map
            (fun v -> 
                if v then
                    Utility.PickFromListRandomly context vowels
                else
                    Utility.PickFromListRandomly context consonants)
        |> List.reduce (+)


    let rec private GenerateIslandNames  //TODO: move to world generator?
            (context : CommonContext) 
            (nameCount:int) 
            (names: Set<string>) 
            : List<string> =
        if names.Count>=nameCount then
            names
            |> Set.toList
            |> Utility.SortListRandomly context
            |> List.take nameCount
        else
            names
            |> Set.add (GenerateIslandName context)
            |> GenerateIslandNames context nameCount

    type SetIslandNameContext =
        abstract member islandSingleNameSink : IslandSingleNameSink ref
    let private SetIslandName
            (context : CommonContext)
            (location : Location)
            (name : string option)
            : unit =
        (context :?> SetIslandNameContext).islandSingleNameSink.Value location name

    type GenerateIslandFeatureContext =
        abstract member islandFeatureGeneratorSource : IslandFeatureGeneratorSource ref
    let private GenerateIslandFeature
            (context: CommonContext)
            : Map<IslandFeatureIdentifier, IslandFeatureGenerator> =
        (context :?> GenerateIslandFeatureContext).islandFeatureGeneratorSource.Value()

    type SetIslandFeatureContext =
        abstract member islandSingleFeatureSink      : IslandSingleFeatureSink ref
    let private SetIslandFeature
            (context : CommonContext)
            (location : Location)
            (identifier : IslandFeatureIdentifier)
            : unit =
        (context :?> SetIslandFeatureContext).islandSingleFeatureSink.Value location identifier

    let private NameIslands
            (context: CommonContext)
            : unit =
        let locations = 
            Island.GetList context
        GenerateIslandNames 
            context 
            (locations.Length) 
            (Utility.GetTermList context "island name" |> Set.ofList)
        |> Utility.SortListRandomly context
        |> List.zip (locations)
        |> List.iter
            (fun (l,n) -> 
                SetIslandName context l (Some n))

    let private PopulateIslands
            (context : CommonContext)
            : unit =
        let generators = GenerateIslandFeature context
        Island.GetList context
        |> List.iter
            (fun location -> 
                generators
                |> Map.iter
                    (fun identifier generator ->
                        if IslandFeature.Create context generator then
                            SetIslandFeature context location identifier))

    let rec private GenerateIslands  //TODO: move to world generator?
            (context                : CommonContext)
            (worldSize              : Location) 
            (minimumIslandDistance  : float)
            (maximumGenerationTries : uint32, 
             currentTry             : uint32) 
            : unit =
        if currentTry>=maximumGenerationTries then
            NameIslands 
                context
            PopulateIslands
                context
        else
            let locations = Island.GetList context
            let candidateLocation = 
                (Utility.GenerateFromRange 
                    context 
                    (0.0, (worldSize |> fst)), 
                        Utility.GenerateFromRange 
                            context 
                            (0.0,(worldSize |> snd)))
            if locations |> List.exists(fun k ->(Location.DistanceTo candidateLocation k) < minimumIslandDistance) then
                GenerateIslands
                    context 
                    worldSize 
                    minimumIslandDistance 
                    (maximumGenerationTries, currentTry+1u) 
            else
                Island.Create
                    context
                    candidateLocation
                GenerateIslands 
                    context 
                    worldSize 
                    minimumIslandDistance 
                    (maximumGenerationTries, 0u) 

    let internal UpdateCharts 
            (context : CommonContext)
            (avatarId : string) 
            : unit =
        let viewDistance = 
            Vessel.GetStatistic context avatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition = 
            avatarId 
            |> Vessel.GetPosition 
                context
            |> Option.get
        Island.GetList context
        |> List.filter
            (fun location -> 
                ((avatarPosition |> Location.DistanceTo location)<=viewDistance))
        |> List.iter
            (fun location ->
                AvatarIslandMetric.Put context avatarId location AvatarIslandMetricIdentifier.Seen 1UL)

    let internal Create 
            (context  : CommonContext)
            (avatarId : string)
            : unit =
        let maximumGenerationRetries =
            WorldStatisticIdentifier.IslandGenerationRetries
            |> WorldStatistic.GetStatistic context
            |> Statistic.GetCurrentValue
            |> uint
        let minimumIslandDistance = 
            WorldStatisticIdentifier.IslandDistance
            |> WorldStatistic.GetStatistic context 
            |> Statistic.GetCurrentValue
        let worldSize =
            (WorldStatisticIdentifier.PositionX
            |> WorldStatistic.GetStatistic context 
            |> Statistic.GetMaximumValue,
                WorldStatisticIdentifier.PositionY
                |> WorldStatistic.GetStatistic context 
                |> Statistic.GetMaximumValue)
        Avatar.Create 
            context
            avatarId
        GenerateIslands 
            context 
            worldSize 
            minimumIslandDistance
            (maximumGenerationRetries, 0u)
        avatarId
        |> UpdateCharts 
            context

