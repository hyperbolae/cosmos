namespace Cosmos.Types

open System
open System.Text.RegularExpressions
open Cosmos.Prelude.Result

type TopicId = TopicId of string

[<Measure>]
type hr

type TimeToLive = TimeToLive of int<hr>

type Body = private Body of string
type Username = private Username of string

type Message = { Body: Body; Username: Username }

type Direction =
    | Up
    | Down
    | Left
    | Right

type Steps = Direction seq

type Thread =
    { Body: string
      Replies: Map<Direction, Thread>
      Position: Steps }

type Offset = private Offset of int

type Update =
    { Offset: Offset
      At: DateTime
      Message: Message
      Steps: Steps }

type Updates = Update seq

type Topic =
    { Id: TopicId
      Topic: Message
      Threads: Map<Direction, Thread>
      LatestOffset: Offset }

module TopicId =
    let create () =
        let guid =
            Guid.NewGuid().ToByteArray()
            |> Convert.ToBase64String

        Regex.Replace(guid, "[/+=]", "", RegexOptions.Compiled)

module Body =
    let create (body: string) =
        if body.Length <= 100 then
            body |> Body |> Ok
        else
            "Message body should be less than 100 characters"
            |> (ValidationError >> Error)

module Username =
    let create (username: string) =
        if username.Length <= 15 then
            username |> Username |> Ok
        else
            "Username should be less than 15 characters"
            |> (ValidationError >> Error)

module Message =
    let create body username =
        let create b u = { Body = b; Username = u }

        create <!> (Body.create body)
        <*> (Username.create username)

module Offset =
    let create offset =
        if offset > 0 then
            offset |> Offset |> Ok
        else
            "Offset should be positive"
            |> (ValidationError >> Error)
