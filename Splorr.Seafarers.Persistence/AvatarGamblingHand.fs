namespace Splorr.Seafarers.Persistence

open System
open Tarot
open System.Data.SQLite

module AvatarGamblingHand =
    let private toCard (value : int) : Card =
        if value > 56 && value <=78 then
            Major ((value- 56) |> enum<Arcana>)
        elif value > 42 && value <=56 then
            Minor (Pentacles, (value-42) |> enum<Rank>)
        elif value > 28 && value <=42 then
            Minor (Swords, (value-28) |> enum<Rank>)
        elif value > 14 && value <=28 then
            Minor (Cups, (value-14) |> enum<Rank>)
        elif value > 0 && value <=14 then
            Minor (Wands, (value) |> enum<Rank>)
        else
            raise (NotImplementedException "invalid card value")
            
    let private ofCard (card:Card) : int =
        match card with
        | Major arcana ->
            56 + (arcana |> int)
        | Minor (Pentacles, rank) ->
            42 + (rank |> int)
        | Minor (Swords, rank) ->
            28 + (rank |> int)
        | Minor (Cups, rank) ->
            14 + (rank |> int)
        | Minor (Wands, rank) ->
            (rank |> int)
        
    let internal GetForAvatar
            (connection : SQLiteConnection) 
            (avatarId   : string) 
            : Result<(Card * Card * Card) option, string> =
        try
            use command = new SQLiteCommand("SELECT [FirstCard], [SecondCard], [FinalCard] FROM [AvatarGamblingHands] WHERE [AvatarId]=$avatarId;", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            let reader = command.ExecuteReader()
            if reader.Read() then
                let firstCard = reader.GetInt32(0) |> toCard
                let secondCard = reader.GetInt32(1) |> toCard
                let finalCard = reader.GetInt32(2) |> toCard
                (firstCard, secondCard, finalCard)
                |> Some
                |> Ok
            else
                None
                |> Ok
        with
        | ex ->
            ex.ToString() |> Error
            
    let internal SetForAvatar
            (connection : SQLiteConnection)
            (avatarId : string)
            (hand: (Card*Card*Card) option)
            : Result<unit, string> =
        try
            match hand with
            | None ->
                use command = new SQLiteCommand("DELETE FROM [AvatarGamblingHands] WHERE [AvatarId]=$avatarId", connection)
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
                command.ExecuteNonQuery() |> ignore
            | Some (first, second, final) ->
                use command = new SQLiteCommand("REPLACE INTO [AvatarGamblingHands] ([AvatarId], [FirstCard], [SecondCard], [FinalCard]) VALUES ($avatarId, $firstCard, $secondCard, $finalCard);", connection)
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
                command.Parameters.AddWithValue("$firstCard", first |> ofCard) |> ignore
                command.Parameters.AddWithValue("$secondCard", second |> ofCard) |> ignore
                command.Parameters.AddWithValue("$finalCard", final |> ofCard) |> ignore
                command.ExecuteNonQuery() |> ignore
            |> Ok
        with
        | ex ->
            ex.ToString() |> Error
        
    

