namespace Cosmos

open Message

module rec Threads =

    type Direction =
        | Up
        | Down
        | Left
        | Right

    type Thread =
        { ParentDirection: Direction
          Message: Message
          Replies: Replies }

    type Reply = Direction * Thread

    type Replies =
        | Zero
        | One of Reply
        | Two of Reply * Reply
        | Three of Reply * Reply * Reply

    let addReply direction message thread =
        let parentDirection =
            match direction with
            | Up -> Down
            | Down -> Up
            | Left -> Right
            | Right -> Left

        let childThread =
            { ParentDirection = parentDirection
              Message = message
              Replies = Zero }

        let isParentDirection = direction = thread.ParentDirection

        let isNewDirection (d, _) =
            not (isParentDirection || d = direction)

        let replies =
            match thread.Replies with
            | Zero when not isParentDirection -> Some(One(direction, childThread))
            | One r when isNewDirection r -> Some(Two(r, (direction, childThread)))
            | Two (r1, r2) when isNewDirection r1 && isNewDirection r2 -> Some(Three(r1, r2, (direction, childThread)))
            | _ -> None

        replies
        |> Option.map (fun r -> { thread with Replies = r })
