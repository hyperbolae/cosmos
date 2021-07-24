namespace Cosmos.Prelude

module Result =

    type Error =
        | ValidationError of string
        | MessageCollisionError

    type Result<'success> =
        | Ok of 'success
        | Error of Error

    let Ok (a: 'a) : Result<'a> = Ok a

    let map f a : Result<'b> =
        match a with
        | Ok a' -> Ok(f a')
        | Error e -> Error e

    let mapError f e : Result<'b> =
        match e with
        | Ok a' -> Ok a'
        | Error e' -> Error(f e')

    let bind f a : Result<'b> =
        match a with
        | Ok a' -> f a'
        | Error e -> Error e

    let apply fR xR =
        match fR, xR with
        | Ok f, Ok x -> Ok(f x)
        | Error err1, Ok _ -> Error err1
        | Ok _, Error err2 -> Error err2
        | Error err1, Error _ -> Error err1

    let (<*>) = apply
    let (<!>) = map
