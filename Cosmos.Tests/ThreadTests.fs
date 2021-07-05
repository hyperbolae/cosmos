module Cosmos.Tests.ThreadTests

open Xunit
open Cosmos
open Cosmos.Threads

let mkMsg = Message.mk "Test"

let mkThread dir msg =
    { ParentDirection = dir
      Message = mkMsg msg
      Replies = Zero }

[<Fact>]
let ``Can add a reply to the right of a thread`` () =
    let thread = mkThread Left "Test"

    let repliedThread = thread |> addReply Right (mkMsg "Right")

    let expected =
        { thread with
              Replies = One(Right, mkThread Left "Right") }
        |> Some

    Assert.Equal(expected, repliedThread)

[<Fact>]
let ``Can add reply to thread with one reply`` () =
    let thread = mkThread Left "Test"

    let repliedThread =
        thread
        |> addReply Right (mkMsg "Right")
        |> Option.bind (addReply Up (mkMsg "Up"))

    let expected =
        { thread with
              Replies = Two((Right, mkThread Left "Right"), (Up, mkThread Down "Up")) }
        |> Some

    Assert.Equal(expected, repliedThread)

[<Fact>]
let ``Can add reply to thread with two replies`` () =
    let thread = mkThread Left "Test"

    let repliedThread =
        thread
        |> addReply Right (mkMsg "Right")
        |> Option.bind (addReply Up (mkMsg "Up"))
        |> Option.bind (addReply Down (mkMsg "Down"))

    let expected =
        { thread with
              Replies = Three((Right, mkThread Left "Right"), (Up, mkThread Down "Up"), (Down, mkThread Up "Down")) }
        |> Some

    Assert.Equal(expected, repliedThread)

[<Fact>]
let ``Cannot add reply to thread with another reply in the same direction`` () =
    let thread =
        mkThread Left "Test"
        |> addReply Right (mkMsg "Right")
        |> Option.bind (addReply Right (mkMsg "Right again"))

    Assert.Equal(None, thread)

[<Fact>]
let ``Cannot add reply in the same direction of the parent thread`` () =
    let thread =
        mkThread Left "Left"
        |> addReply Right (mkMsg "Right")
        |> Option.bind (addReply Left (mkMsg "Now same as parent"))

    Assert.Equal(None, thread)
