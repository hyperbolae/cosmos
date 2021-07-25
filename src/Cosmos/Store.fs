namespace Cosmos.Store

open System
open System.Collections.Generic
open Cosmos.Types
open Cosmos.Prelude.Result

type ExpiringCache<'a>() =
    let cache = Dictionary<string, DateTime * 'a>()

    member _.Add (TimeToLive ttl) id item =
        let validUntil = DateTime.UtcNow.AddHours(float ttl)

        let addToCache () =
            match cache.TryAdd(id, (validUntil, item)) with
            | true -> Ok cache.[id]
            | false -> Error StoreError
            |> Result.map snd

        let handleExisting expiry =
            if expiry < DateTime.UtcNow then
                cache.Remove id |> ignore
                addToCache ()
            else
                Error AlreadyExistsError

        match cache.TryGetValue id with
        | true, (expiry, _) -> handleExisting expiry
        | false, _ -> addToCache ()

    member _.Get id =
        let handleExisting expiry value =
            if expiry < DateTime.UtcNow then
                cache.Remove id |> ignore
                Error ExpiredError
            else
                Ok value

        match cache.TryGetValue id with
        | true, (expiry, value) -> handleExisting expiry value
        | false, _ -> Error NotFoundError

    member _.Values() =
        cache.Values
        |> Seq.filter (fun (expiry, _) -> expiry > DateTime.UtcNow)
        |> Seq.map snd
