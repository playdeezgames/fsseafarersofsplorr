module MessageTests

open NUnit.Framework
open Splorr.Seafarers.Persistence
open System

module Dummy =
    let AvatarId = "avatar"
    let Messages : Message.Record list =
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

type Mock(creator,destroyer, retriever) =
    interface Splorr.Seafarers.Persistence.Message.Repository with
        member this.Creator: Splorr.DataStore.StoreCreator<Message.Identifier,Message.Record> option = creator
        member this.Destroyer: Splorr.DataStore.StoreDestroyer<Message.Identifier,string> option = destroyer
        member this.Retriever: Splorr.DataStore.StoreRetriever<Message.Identifier,Message.Record,string> option = retriever
        member this.Upsertor: Splorr.DataStore.StoreUpsertor<Message.Identifier,Message.Record> option = 
            raise (System.NotImplementedException())

[<Test>]
let ``ClearForAvatar.It calls data store to remove all messages for a given avatar.`` () =
    let mutable called = false
    let destroyer(avatarId:string) =
        called <- true
        Assert.AreEqual(Dummy.AvatarId, avatarId)
        Set.empty
        |> Ok
    let repository = 
        Mock(None, Some destroyer, None)
    Message.ClearForAvatar repository Dummy.AvatarId
    Assert.IsTrue called

[<Test>]
let ``ClearForAvatar.It calls data store to remove all messages for a given avatar and throws an exception if the functor returns an error message.`` () =
    let mutable called = false
    let destroyer(avatarId:string) =
        called <- true
        Assert.AreEqual(Dummy.AvatarId, avatarId)
        "I Am Error"
        |> Error
    let repository = 
        Mock(None, Some destroyer, None)
    let result = 
        Assert.Throws<System.Exception>(fun () ->Message.ClearForAvatar repository Dummy.AvatarId)
    Assert.AreEqual
        ("I Am Error", result.Message)
    Assert.IsTrue called

[<Test>]
let ``AddForAvatar.It calls data store to add for each message provided.`` () =
    let mutable recordId = 0L
    let creator(record:Message.Record) : Result<int64, string> =
        recordId <- recordId + 1L
        recordId
        |> Ok
    let repository = 
        Mock(Some creator, None, None)
    Message.AddForAvatar
        repository
        Dummy.Messages
    Assert.AreEqual(2L, recordId)

[<Test>]
let ``AddForAvatar.It throws an exception when the function returns an error message.`` () =
    let mutable recordId = 0L
    let creator(record:Message.Record) : Result<int64, string> =
        "No shirt. No shoes. No dice."
        |> Error
    let repository = 
        Mock(Some creator, None, None)
    let result = 
        Assert.Throws<Exception>
            (fun () -> 
                Message.AddForAvatar
                    repository
                    Dummy.Messages)
    Assert.AreEqual("No shirt. No shoes. No dice.", result.Message)
    Assert.AreEqual(0L, recordId)

[<Test>]
let ``GetForAvatar.It calls the retrieval functor on the repository.`` () =
    let mutable called = false
    let retriever(avatarId: string) : Result<(Message.Identifier * Message.Record) list, string> =
        called <- true
        List.zip [1L..(Dummy.Messages.Length |> int64)] Dummy.Messages
        |> Ok
    let repository = 
        Mock(None, None, Some retriever)
    let actual = 
        Message.GetForAvatar
            repository
            Dummy.AvatarId
    Assert.AreEqual(Dummy.Messages, actual)
    Assert.IsTrue(called)

[<Test>]
let ``GetForAvatar.It throws an exception when the functor returns an error message.`` () =
    let mutable called = false
    let retriever(avatarId: string) : Result<(Message.Identifier * Message.Record) list, string> =
        called <- true
        "No shirt. No shoes. No Dice."
        |> Error
    let repository = 
        Mock(None, None, Some retriever)
    let actual = 
        Assert.Throws<Exception>
            (fun () ->
                Message.GetForAvatar
                    repository
                    Dummy.AvatarId
                |> ignore)
    Assert.AreEqual(actual.Message, "No shirt. No shoes. No Dice.")
    Assert.IsTrue(called)
