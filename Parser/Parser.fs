module StockChartBot.Parser

open System
open System.Globalization

type Outcome<'S> = 
    | Success of s: 'S
    | Failure of message: string

type Query =
    {
        Sender : string
        Ticker : string
        From : DateTime
        Until : DateTime
    }

let private parseDate (s : string) =
    DateTime.ParseExact(s, "M/d/yyyy",
        CultureInfo.InvariantCulture,
        DateTimeStyles.AssumeLocal)

let Parse (text : string) =
    let elements = text.Split([|' '|])
    match elements with 
    | [| _sender; _ticker; _year|] -> 
        try
            {
                Sender = elements.[0]
                Ticker = elements.[1]
                From = DateTime(int _year, 1, 1)
                Until = DateTime(int _year, 12, 31)
            } |> Outcome.Success
        with
        | e -> Failure (sprintf "Error occured %s" e.Message)
    | [| _sender; _ticker; _from; _until|] ->
        try
            {
                Sender = elements.[0]
                Ticker = elements.[1]
                From = elements.[2] |> parseDate
                Until = elements.[3] |> parseDate
            } |> Outcome.Success
        with 
        | e -> Failure (sprintf "Error occured %s" e.Message)
    | _ -> Failure "Input is of wrong format"