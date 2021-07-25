namespace Cosmos.Endpoints

open Giraffe

module Threads =

    let handler : HttpHandler =
        route "/threads"
        >=> choose [ route "/" >=> text "benga threads" ]

module Updates =

    let handler : HttpHandler =
        route "/updates"
        >=> choose [ route "/updates" >=> text "benga updates" ]
