namespace Cosmos

module Message =
    type Message = { Username: string; Message: string }

    let mk username message =
        { Username = username
          Message = message }
