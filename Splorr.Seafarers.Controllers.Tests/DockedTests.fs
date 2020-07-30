module DockedTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open DockedTestFixtures
open Splorr.Seafarers.Services

let private functionUnderTest itemMarketSource itemSingleMarketSink =  Docked.Run dockedCommoditySource dockedItemSource itemMarketSource itemSingleMarketSink
let private functionUnderTestStubbed = functionUnderTest dockedItemMarketSourceStub dockedItemSingleMarketSourceStub dockedItemSingleMarketSinkStub
[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let input = deadDockWorld   
    let inputLocation= deadDockLocation
    let inputSource(): Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let expected =
        input.Avatars.[avatarId].Messages
        |> Gamestate.GameOver
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given Undock Command.`` () =
    let input = dockWorld
    let inputLocation= dockLocation
    let inputSource = Command.Undock |> Some |> toSource
    let expectedMessages = ["You undock."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with Messages = expectedMessages}
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = Command.Quit |> Some |> toSource
    let expected =
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.ConfirmQuit 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub 
    Assert.AreEqual(expected,actual)

[<Test>]
let ``Run.It returns Metrics when given Metrics Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Metrics 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.Metrics 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Help when given Help Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Help 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.Help 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Inventory when given Inventory Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Inventory 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.Inventory 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns InvalidInput when given invalid Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource =
        None 
        |> toSource
    let expected = 
        ("Maybe try 'help'?",(Dock, inputLocation, input) 
        |> Gamestate.Docked)
        |> Gamestate.ErrorMessage
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub 
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns AtSea when given invalid docked location.`` () =
    let input = dockWorld
    let inputLocation = (1.0, 1.0)
    let inputSource () = 
        Assert.Fail("This should not be called.")
        Command.Help 
        |> Some
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub 
    Assert.AreEqual(expected,actual)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Status 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.Status 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub 
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns Docked (at Jobs) gamestate when given the command Jobs.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Jobs 
        |> Some 
        |> toSource
    let expected = 
        (Jobs, inputLocation, input) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed inputSource sinkStub  
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given the Accept Job command and the given job number does not exist.`` () =
    let input = smallWorldDocked
    let inputLocation = smallWorldIslandLocation
    let inputSource = 0u |> Command.AcceptJob |> Some |> toSource
    let expectedMessages = [ "That job is currently unavailable." ]
    let expectedAvatar =
        {input.Avatars.[avatarId] with Messages = expectedMessages}
    let expectedWorld = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
    let expected = 
        (Dock, inputLocation,  expectedWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> functionUnderTestStubbed inputSource sinkStub inputLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given the command Abandon Job and the avatar has no current job.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Job 
        |> Command.Abandon 
        |> Some 
        |> toSource
    let expectedAvatar =
        {input.Avatars.[avatarId] with Messages = ["You have no job to abandon."]}
    let expectedWorld = 
        {input with Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
    let expected = 
        (Dock, inputLocation, expectedWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> functionUnderTestStubbed inputSource sinkStub inputLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and abandons the job when given the command Abandon Job and the avatar has a current job.`` () =
    let input = abandonJobWorld
    let inputLocation = dockLocation
    let inputSource = Job |> Command.Abandon |> Some |> toSource
    let expectedMessages = ["You abandon your job."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Job=None
            Reputation = input.Avatars.[avatarId].Reputation-1.0
            Messages = expectedMessages
            Metrics = input.Avatars.[avatarId].Metrics |> Map.add Metric.AbandonedJob 1u}
    let expectedWorld = 
        {input with 
            Avatars= input.Avatars |> Map.add avatarId expectedAvatar}
    let expected = 
        (Dock, inputLocation, expectedWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> functionUnderTestStubbed inputSource sinkStub inputLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at ItemList) gamestate when given the Items command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = 
        Command.Items 
        |> Some 
        |> toSource
    let expected = 
        (ItemList, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Buy command for a non-existent item.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (1u |> Specific, "non existent item") |> Command.Buy |> Some |> toSource
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["Round these parts, we don't sell things like that."]
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Buy command and the avatar does not have enough money to complete the purchase.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = smallWorldDocked |> World.ClearMessages avatarId
    let inputSource = (1u |> Specific, "item under test") |> Command.Buy |> Some |> toSource
    let markets =
        Map.empty
        |> Map.add 1UL {Demand=5.0; Supply=5.0}
    let islandMarketSource (_) = markets
    let islandSingleMarketSource x y =
        islandMarketSource x
        |> Map.tryFind y
    let islandSingleMarketSink (_) (_) =
        Assert.Fail("This should not be called.")
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["You don't have enough money."]
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTest islandMarketSource islandSingleMarketSource islandSingleMarketSink inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message and completes the purchase when given the Buy command and the avatar has enough money.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Money = 1000000.0}
    let inputWorld = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputSource = (1u |> Specific, "item under test") |> Command.Buy |> Some |> toSource
    let markets =
        Map.empty
        |> Map.add 1UL {Demand=5.0; Supply=5.0}
    let islandMarketSource (_) = markets
    let islandSingleMarketSource x y =
        islandMarketSource x
        |> Map.tryFind y
    let expectedPrice = Item.DetermineSalePrice commodities markets smallWorldItems.[1UL]
    let expectedDemand = 
        markets.[1UL].Demand + commodities.[1UL].SaleFactor
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(markets.[commodityId].Supply, market.Supply)
        Assert.AreEqual(expectedDemand, market.Demand)
    let expectedIsland = 
        inputWorld.Islands.[inputLocation]
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["You complete the purchase of 1 item under test."]
        |> World.TransformIsland inputLocation (fun _ -> expectedIsland |> Some)
        |> World.TransformAvatar avatarId (Avatar.AddInventory 1UL 1u >> Some)
        |> World.TransformAvatar avatarId (Avatar.SpendMoney expectedPrice >> Some)
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTest islandMarketSource islandSingleMarketSource islandSingleMarketSink inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Sell command for a non-existent item.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (Specific 1u, "non existent item") |> Command.Sell |> Some |> toSource
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["Round these parts, we don't buy things like that."]
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Sell command and the avatar does not sufficient items to sell.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (Specific 1u, "item under test") |> Command.Sell |> Some |> toSource
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["You don't have enough of those to sell."]
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message and completes the sale when given the Sell command and the avatar sufficient items to sell.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputItems = [(1UL, 1u)] |> Map.ofList
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Inventory = inputItems}
    let markets =
        Map.empty
        |> Map.add 1UL {Supply = 5.0; Demand =5.0}
    let islandMarketSource (_) = markets
    let islandSingleMarketSource x y =
        islandMarketSource x
        |> Map.tryFind y
    let inputWorld = 
        {shopWorld with 
            Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputSource = (Specific 1u, "item under test") |> Command.Sell |> Some |> toSource
    let expectedSupply = 
        markets.[1UL].Supply + commodities.[1UL].PurchaseFactor
    let expectedMarket = 
        {markets.[1UL] with 
            Supply= expectedSupply}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(expectedMarket, market)
    let expectedIsland = 
        inputWorld.Islands.[inputLocation]
    let expectedAvatar = 
        {inputAvatar with 
            Inventory = Map.empty
            Money = 0.5
            Messages = ["You complete the sale of 1 item under test."]}
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["You complete the sale."]
        |> World.TransformIsland inputLocation (fun _ -> expectedIsland |> Some)
        |> World.TransformAvatar avatarId (fun _ -> expectedAvatar |> Some)
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTest islandMarketSource islandSingleMarketSource islandSingleMarketSink inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

