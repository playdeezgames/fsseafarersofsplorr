namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System.Drawing
open System

module Chart = 
    let private plotLocation 
            (scale    : int) 
            (location : Location) 
            : int * int =
        ((location |> snd |> int) * scale - scale/2, (-(location |> fst |> int)) * scale - scale/2)

    let private outputChart 
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (islandSingleNameSource         : IslandSingleNameSource)
            (islandSource                   : IslandSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (worldSize                      : Location) 
            (messageSink                    : MessageSink) 
            (chartName                      : string) 
            (avatarId                       : string) 
            : unit =
        try
            let scale = 10
            let x, y = plotLocation scale worldSize
            use bmp = new Bitmap(x, -y)
            let g = Graphics.FromImage(bmp)
            g.TranslateTransform(0.0f, (-y) |> float32)
            g.Clear(Color.DarkBlue)
            use seenIslandBrush = new SolidBrush(Color.Green)
            use knownIslandBrush = new SolidBrush(Color.LightGreen)
            use avatarBrush = new SolidBrush(Color.Bisque)
            use textBrush:Brush = new SolidBrush(Color.White) :> Brush
            use font = new Font("Arial", 10.0f)
            let legend: Map<uint,string> = 
                islandSource()
                |> List.fold
                    (fun leg location -> 
                        let x, y = plotLocation scale location
                        let addToLegend, brush =
                            match avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount with
                            | None -> false, seenIslandBrush
                            | _ -> true, knownIslandBrush
                        g.FillEllipse(brush,x,y,scale,scale)
                        if addToLegend then
                            let index = (leg.Count + 1) |> uint
                            let yOffset = if (-y)>bmp.Height/2 then 10.0f else (-20.0f)
                            g.DrawString(index|>sprintf "%u", font, textBrush, (x |> float32), (y |> float32) + yOffset)
                            leg
                            |> Map.add index (islandSingleNameSource location |> Option.get)
                        else
                            leg) Map.empty
            let avatarPosition = 
                avatarId
                |> Avatar.GetPosition vesselSingleStatisticSource 
                |> Option.get
                |> plotLocation scale
            g.FillEllipse(avatarBrush, avatarPosition |> fst , avatarPosition |> snd, scale, scale)
            bmp.Save(chartName |> sprintf "%s.png", Imaging.ImageFormat.Png)
            let legendText =
                legend
                |> Map.toList
                |> List.map 
                    (fun (index,name) ->
                        sprintf "%u - %s" index name)
                |> List.toArray
            IO.File.WriteAllLines(chartName |> sprintf "%s.txt", legendText)
        with
        | ex ->
            [
                (Hue.Error, ex.ToString() |> sprintf "An error occurred when attempting to export the chart: '%s'" |> Line) |> Hued
            ]            
            |> List.iter messageSink
            //try, catch, eat... ci build fails because of the gdi stuff not working on wherever it is being built

    let private UpdateDisplay 
        (messageSink : MessageSink) 
        (chartName   : string)
        : unit =
        [
            "" |> Line
            (chartName, chartName) ||> sprintf "Writing chart to '%s.png' and '%s.txt'" |> Line
        ]
        |> List.iter messageSink

    let private GetDefaultedChartName 
            (chartName : string) 
            : string =
        if chartName |> String.IsNullOrWhiteSpace then 
            Guid.NewGuid().ToString() 
        else 
            chartName

    let Run 
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (islandSingleNameSource         : IslandSingleNameSource)
            (islandSource                   : IslandSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (worldSingleStatisticSource     : WorldSingleStatisticSource) 
            (messageSink                    : MessageSink) 
            (chartName                      : string) 
            (avatarId                       : string) 
            : Gamestate option =
        let chartName = 
            chartName 
            |> GetDefaultedChartName
        UpdateDisplay 
            messageSink 
            chartName
        outputChart 
            avatarIslandSingleMetricSource
            islandSingleNameSource
            islandSource
            vesselSingleStatisticSource
            (worldSingleStatisticSource WorldStatisticIdentifier.PositionX |> Statistic.GetMaximumValue, 
                worldSingleStatisticSource WorldStatisticIdentifier.PositionY |> Statistic.GetMaximumValue) 
            messageSink 
            chartName 
            avatarId
        avatarId
        |> Gamestate.AtSea
        |> Some

