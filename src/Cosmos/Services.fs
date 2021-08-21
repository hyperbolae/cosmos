namespace Cosmos.Services

open Cosmos.Domain
open Cosmos.Prelude.Result

module Room =
    let private origin = 0, 0

    let private getParentDirection message : Position =
        let { Position = (x, y)
              ParentDirection = direction } =
            message

        match direction with
        | Up -> (x, y + 1)
        | Down -> (x, y - 1)
        | Left -> (x - 1, y)
        | Right -> (x + 1, y)

    let addMessage room newMessage =
        let parentDirection = getParentDirection newMessage

        let isEligiblePosition message =
            (parentDirection = origin
             || parentDirection = message.Position)
            && newMessage.Position <> message.Position

        let messageIsEligible =
            newMessage.Position <> origin
            && room.Messages |> Seq.forall isEligiblePosition

        if messageIsEligible then
            let messages =
                newMessage
                |> Seq.singleton
                |> Seq.append room.Messages

            Ok { room with Messages = messages }
        else
            Error MessageCollisionError
