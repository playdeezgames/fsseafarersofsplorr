﻿namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open System.Drawing

module Chart = 
    let private plotLocation (scale:int) (location:Location) : int * int =
        ((location |> fst |> int) * scale - scale/2, (location |> snd |> int) * scale-scale/2)

    let private outputChart (worldSize:Location) (chartName: string) (world:World) : unit =
        try
            let avatar = world.Avatars.[world.AvatarId]
            let scale = 10
            let x, y = plotLocation scale worldSize
            use bmp = new Bitmap(x, y)
            let g = Graphics.FromImage(bmp)
            g.Clear(Color.DarkBlue)
            use seenIslandBrush = new SolidBrush(Color.Green)
            use knownIslandBrush = new SolidBrush(Color.LightGreen)
            use avatarBrush = new SolidBrush(Color.Bisque)
            use textBrush:Brush = new SolidBrush(Color.White) :> Brush
            use font = new Font("Arial", 10.0f)
            let legend: Map<uint,string> = 
                world.Islands
                |> Map.filter
                    (fun _ island ->
                        island.AvatarVisits.ContainsKey world.AvatarId)
                |> Map.fold
                    (fun leg location island -> 
                        let x, y = plotLocation scale location
                        let addToLegend, brush =
                            match island.AvatarVisits.[world.AvatarId].VisitCount with
                            | None -> false, seenIslandBrush
                            | _ -> true, knownIslandBrush
                        g.FillEllipse(brush,x,y,scale,scale)
                        if addToLegend then
                            let index = (leg.Count + 1) |> uint
                            g.DrawString(index|>sprintf "%u", font, textBrush, (x |> float32), (y |> float32) - 20.0f)
                            leg
                            |> Map.add index island.Name
                        else
                            leg) Map.empty
            g.FillEllipse(avatarBrush, (avatar.Position |> fst |> int) * scale, (avatar.Position |> snd |> int) * scale, scale, scale)
            bmp.Save(chartName |> sprintf "%s.png", Imaging.ImageFormat.Png)
            let legendText =
                legend
                |> Map.toList
                |> List.map 
                    (fun (index,name) ->
                        sprintf "%u - %s" index name)
                |> List.toArray
            System.IO.File.WriteAllLines(chartName |> sprintf "%s.txt", legendText)
        with
        | _ -> ()
            //try, catch, eat... ci build fails because of the gdi stuff not working on wherever it is being built
            //TODO: if this should happen on a user's machine, we should puke something out to a file or other indicator

    let Run (worldSize:Location) (sink:MessageSink) (chartName:string) (world:World) : Gamestate option =
        let chartName = if chartName |> System.String.IsNullOrWhiteSpace then System.Guid.NewGuid().ToString() else chartName
        [
            "" |> Line
            (chartName, chartName) ||> sprintf "Writing chart to '%s.png' and '%s.txt'" |> Line
        ]
        |> List.iter sink
        outputChart worldSize chartName world
        world
        |> Gamestate.AtSea
        |> Some

