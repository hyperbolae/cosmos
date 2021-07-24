namespace Cosmos.Prelude

module Result =

    type Error =
        | ValidationError of string
        | MessageCollisionError
        | AggregateError of Error list

    type Result<'success> = Result<'success, Error>

    let Ok (a: 'a) : Result<'a> = Ok a

    let map f a : Result<'b> =
        match a with
        | Ok a' -> Ok(f a')
        | Error e -> Error e

    let bind f a : Result<'b> =
        match a with
        | Ok a' -> f a'
        | Error e -> Error e

    let createAggregateError error1 error2 =
        match error1, error2 with
        | AggregateError e1, AggregateError e2 -> e1 @ e2
        | AggregateError e, _ -> error2 :: e
        | _, AggregateError e -> error1 :: e
        | _, _ -> [ error1; error2 ]
        |> AggregateError

    let apply fR xR =
        match fR, xR with
        | Ok f, Ok x -> Ok(f x)
        | Error err1, Ok _ -> Error err1
        | Ok _, Error err2 -> Error err2
        | Error err1, Error err2 -> Error <| createAggregateError err1 err2

    let (<*>) = apply
    let (<!>) = map
