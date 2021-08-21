module Cosmos.Tests.ServiceTests

open Xunit
open Cosmos.Domain
open Cosmos.Prelude.Result
open Cosmos.Services
open Cosmos.Tests.Result

module ExpiringCacheTests =
    let id = RoomId.create ()

    let createMessage position direction =
        { Body = Body "Test message"
          Username = Username "Test user"
          Position = position
          ParentDirection = direction }

    let createRoom messages =
        { Id = id
          Title = Title "Test Room"
          Messages = messages }

    [<Fact>]
    let ``Can add message to room`` () =
        let room = createRoom Seq.empty
        let newMessage = createMessage (0, 1) Down

        let newRoom = Room.addMessage room newMessage

        let expectedMessages = [ newMessage ]

        res {
            Assert.True(Result.IsOk newRoom)

            let! okRoom = newRoom
            Assert.Equal(expectedMessages, okRoom.Messages)
        }

    [<Fact>]
    let ``Can add a reply to another message in to room`` () =
        let room = createRoom [ createMessage (0, 1) Down ]
        let newMessage = createMessage (0, 2) Down

        let newRoom = Room.addMessage room newMessage

        let expectedMessages =
            [ createMessage (0, 1) Down
              createMessage (0, 2) Down ]

        res {
            Assert.True(IsOk newRoom)

            let! okRoom = newRoom
            Assert.Equal(expectedMessages, okRoom.Messages)
        }

    [<Fact>]
    let ``Cannot add a reply when message with same position exists`` () =
        let room = createRoom [ createMessage (0, 1) Down ]
        let newMessage = createMessage (0, 1) Down

        let newRoom = Room.addMessage room newMessage

        Assert.True(newRoom |> IsError MessageCollisionError)

    [<Fact>]
    let ``Cannot add a reply when parent does not exist in room`` () =
        let room = createRoom Seq.empty
        let newMessage = createMessage (0, 2) Down

        let newRoom = Room.addMessage room newMessage

        Assert.True(newRoom |> IsError MessageCollisionError)
