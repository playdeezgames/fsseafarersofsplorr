module MessageTests

open NUnit.Framework
open Splorr.Seafarers.Persistence
open System

module Dummy =
    let AvatarId = "avatar"
    let Messages : Splorr.Seafarers.DataStore.Message.Record list =
        [
            {
                avatarId = AvatarId
                text="i am text"
                foregroundColor = ConsoleColor.White
                backgroundColor = ConsoleColor.Black
            }
            {
                avatarId = AvatarId
                text="i am inverted text"
                foregroundColor = ConsoleColor.Black
                backgroundColor = ConsoleColor.White
            }

        ]

[<Test>]
let ``Repository Destroy.It calls data store to remove all messages for a given avatar.`` () =
    let mutable called = false
    let destroyer(avatarId:string) =
        called <- true
        Assert.AreEqual(Dummy.AvatarId, avatarId)
        Set.empty
        |> Ok
    let repository = 
        Message.Repository(None, None, None, Some destroyer) :> Splorr.Seafarers.DataStore.Message.IRepository
    let actual = repository.Destroy Dummy.AvatarId
    Assert.AreEqual(Set.empty, actual)
    Assert.IsTrue called

[<Test>]
let ``Repository Destroy.It calls data store to remove all messages for a given avatar and throws an exception if the functor returns an error message.`` () =
    let mutable called = false
    let destroyer(avatarId:string) =
        called <- true
        Assert.AreEqual(Dummy.AvatarId, avatarId)
        "I Am Error"
        |> Error
    let repository = 
        Message.Repository(None, None, None, Some destroyer) :> Splorr.Seafarers.DataStore.Message.IRepository
    let result = 
        Assert.Throws<System.Exception>(fun () ->repository.Destroy Dummy.AvatarId |> ignore)
    Assert.AreEqual
        ("I Am Error", result.Message)
    Assert.IsTrue called

[<Test>]
let ``Repository Create.It calls data store to add for each message provided.`` () =
    let mutable recordId = 0L
    let creator(record:Splorr.Seafarers.DataStore.Message.Record) : Result<int64, string> =
        recordId <- recordId + 1L
        recordId
        |> Ok
    let repository = 
        Message.Repository(Some creator, None, None, None) :> Splorr.Seafarers.DataStore.Message.IRepository
    let actual = 
        Dummy.Messages
        |> List.iter
            (repository.Create >> ignore)
            
    Assert.AreEqual(2L, recordId)

[<Test>]
let ``Repository Create.It throws an exception when the function returns an error message.`` () =
    let mutable recordId = 0L
    let creator(record:Splorr.Seafarers.DataStore.Message.Record) : Result<int64, string> =
        "No shirt. No shoes. No dice."
        |> Error
    let repository = 
        Message.Repository(Some creator, None, None, None) :> Splorr.Seafarers.DataStore.Message.IRepository
    let result = 
        Assert.Throws<Exception>
            (fun () -> 
                Dummy.Messages
                |> List.iter
                    (repository.Create >> ignore))
    Assert.AreEqual("No shirt. No shoes. No dice.", result.Message)
    Assert.AreEqual(0L, recordId)

[<Test>]
let ``Repository Retrieve.It calls the retrieval functor on the repository.`` () =
    let mutable called = false
    let retriever(avatarId: string) : Result<(Splorr.Seafarers.DataStore.Message.Identifier * Splorr.Seafarers.DataStore.Message.Record) list, string> =
        called <- true
        List.zip [1L..(Dummy.Messages.Length |> int64)] Dummy.Messages
        |> Ok
    let repository = 
        Message.Repository(None, Some retriever, None, None) :> Splorr.Seafarers.DataStore.Message.IRepository
    let actual = 
        repository.Retrieve
            Dummy.AvatarId
        |> List.map snd
    Assert.AreEqual(Dummy.Messages, actual)
    Assert.IsTrue(called)

[<Test>]
let ``Repository Retrieve.It throws an exception when the functor returns an error message.`` () =
    let mutable called = false
    let retriever(avatarId: string) : Result<(Splorr.Seafarers.DataStore.Message.Identifier * Splorr.Seafarers.DataStore.Message.Record) list, string> =
        called <- true
        "No shirt. No shoes. No Dice."
        |> Error
    let repository = 
        Message.Repository(None, Some retriever, None, None) :> Splorr.Seafarers.DataStore.Message.IRepository
    let actual = 
        Assert.Throws<Exception>
            (fun () ->
                repository.Retrieve
                    Dummy.AvatarId
                |> ignore)
    Assert.AreEqual(actual.Message, "No shirt. No shoes. No Dice.")
    Assert.IsTrue(called)
