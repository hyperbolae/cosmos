namespace Cosmos.Domain

open System
open System.Text.RegularExpressions
open Cosmos.Prelude.Result

type RoomId = RoomId of string

[<Measure>]
type hr

type TimeToLive = TimeToLive of int<hr>

type Body = Body of string

type Username = Username of string

type Title = Title of string

type Position = int * int

type Direction =
    | Up
    | Down
    | Left
    | Right

type Message =
    { Body: Body
      Username: Username
      Position: Position
      ParentDirection: Direction }

type Room =
    { Id: RoomId
      Title: Title
      Messages: Message seq }

module RoomId =
    let create () =
        let guid =
            Guid.NewGuid().ToByteArray()
            |> Convert.ToBase64String

        Regex.Replace(guid, "[/+=]", "", RegexOptions.Compiled).[..6]
        |> RoomId

module Title =
    let create (title: string) =
        if title.Length <= 100 then
            title |> Title |> Ok
        else
            "Message title should be less than 100 characters"
            |> (ValidationError >> Error)

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
    let create body username position parentDirection =
        let create b u =
            { Body = b
              Username = u
              Position = position
              ParentDirection = parentDirection }

        create <!> (Body.create body)
        <*> (Username.create username)
