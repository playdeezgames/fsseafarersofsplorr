namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System

module Chart = 
    let private plotLocation 
            (scale    : int) 
            (location : Location) 
            : int * int =
        ((location |> snd |> int) * scale - scale/2, (-(location |> fst |> int)) * scale + scale/2)

    let private outputChart 
            (context : ServiceContext)
            (worldSize                      : Location) 
            (messageSink                    : MessageSink) 
            (chartName                      : string) 
            (avatarId                       : string) 
            : unit =
        try
            use writer = System.IO.File.CreateText(sprintf "%s.html" chartName)
            writer.WriteLine("<html>")
            writer.WriteLine("<body>")
            writer.WriteLine("<table>")
            writer.WriteLine("<tbody>")
            writer.WriteLine("<tr>")
            writer.WriteLine("<td>")
            let scale = 10
            let width, height = ((worldSize |> fst |> int) * scale, (worldSize |> snd |> int) * scale)
            writer.WriteLine(sprintf "<svg width=\"%d\" height=\"%d\">" width height)
            writer.WriteLine(sprintf "<rect width=\"%d\" height=\"%d\" style=\"fill:#00008B;\"/>" width height)
            let legend: Map<uint,string> = 
                context
                |> Island.GetList
                |> List.fold
                    (fun leg location -> 
                        let x, y = plotLocation scale location
                        let seen = 
                            Avatar.GetIslandMetric context location AvatarIslandMetricIdentifier.Seen  avatarId
                            |> Option.map (fun x -> x > 0UL) 
                            |> Option.defaultValue false
                        let addToLegend =
                            match Avatar.GetIslandMetric context location AvatarIslandMetricIdentifier.VisitCount avatarId with
                            | None -> false
                            | _ -> true
                        match seen, addToLegend with
                        | true, true ->
                            writer.WriteLine(sprintf "<ellipse cx=\"%d\" cy=\"%d\" rx=\"%d\" ry=\"%d\" style=\"fill:#00FF00;\"/>" x (height+y) (scale/2) (scale/2))
                        | true, false ->
                            writer.WriteLine(sprintf "<ellipse cx=\"%d\" cy=\"%d\" rx=\"%d\" ry=\"%d\" style=\"fill:#008000;\"/>" x (height+y) (scale/2) (scale/2))
                        | _ ->
                            ()
                        if addToLegend then
                            let index = (leg.Count + 1) |> uint
                            let yOffset = if (-y)>height/2 then 20 else (-10)
                            writer.WriteLine(sprintf "<text x=\"%d\" y=\"%d\" fill=\"#ffffff\">%u</text>" x (y+height+yOffset) index)
                            leg
                            |> Map.add index (Island.GetName context location |> Option.get)
                        else
                            leg) Map.empty
            let avatarPosition = 
                avatarId
                |> Avatar.GetPosition 
                    context
                |> Option.get
                |> plotLocation scale
            writer.WriteLine(sprintf "<ellipse cx=\"%d\" cy=\"%d\" rx=\"%d\" ry=\"%d\" style=\"fill:#c0c000;\"/>" (avatarPosition |> fst) (height+(avatarPosition |> snd)) 3 3)
            writer.WriteLine("</svg>")
            writer.WriteLine("</td>")
            writer.WriteLine("<td valign=\"top\">")
            writer.WriteLine("<ul>")
            legend
            |> Map.toList
            |> List.iter
                (fun (index,name) ->
                    (index, name)
                    ||> sprintf "<li>%u - %s</li>" 
                    |> writer.WriteLine)
            writer.WriteLine("</ul>")
            writer.WriteLine("</td>")
            writer.WriteLine("</tr>")
            writer.WriteLine("</tbody>")
            writer.WriteLine("</table>")
            writer.WriteLine("</body>")
            writer.WriteLine("</html>")
            writer.Close()
        with
        | ex ->
            [
                (Hue.Error, ex.ToString() |> sprintf "An error occurred when attempting to export the chart: '%s'" |> Line) |> Hued
            ]            
            |> List.iter messageSink

    let private UpdateDisplay 
        (messageSink : MessageSink) 
        (chartName   : string)
        : unit =
        [
            "" |> Line
            chartName |> sprintf "Writing chart to '%s.html'" |> Line
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
            (context : ServiceContext)
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
            context
            (World.GetStatistic context WorldStatisticIdentifier.PositionX |> Statistic.GetMaximumValue, 
                World.GetStatistic context WorldStatisticIdentifier.PositionY |> Statistic.GetMaximumValue) 
            messageSink 
            chartName 
            avatarId
        avatarId
        |> Gamestate.InPlay
        |> Some

