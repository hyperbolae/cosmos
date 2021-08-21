module Cosmos.Tests.Result

open Cosmos.Prelude.Result

let IsOk res =
    match res with
    | Ok _ -> true
    | Error _ -> false

let IsError (errorType: Error) res =
    match res with
    | Ok _ -> true
    | Error e -> e = errorType
