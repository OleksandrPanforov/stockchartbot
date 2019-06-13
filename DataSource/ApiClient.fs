module DataSource.ApiClient

open System
open FSharp.Data
open StockChartBot.Parser

let private apiKey = "kfUD9k9Fdh4eefyyUNQH"

let private BuildUrl (query:Query) = 
    let (~~) (d : DateTime) = d.ToString("yyyy-MM-dd")
    sprintf "https://www.quandl.com/api/v3/datasets/FSE/%s.json?start_date=%s&end_date=%s&api_key=%s" query.Ticker ~~query.From ~~query.Until apiKey

type Quandl = JsonProvider<"./sample.json">

//TODO Add error logging
type Error = JsonProvider<"{\"quandl_error\":{\"code\":\"\",\"message\":\"\"}}">

let private BuildQuery (data: string) = 
    match Parse(data) with
    | Success s -> s
    | Failure _ -> raise (InvalidOperationException("Parsing can't be completed"))

let GetStockDataWithQuery (query: Query) = 
    let url = BuildUrl(query)
    try 
       match Http.Request(url).Body with
       | Text t ->  Quandl.Parse t
       | Binary _ -> raise (new InvalidOperationException "Could not retrieve data")
    |> Outcome.Success
    with 
    | e -> Failure(sprintf "Error occured %s" e.Message)

let GetStockDataWithString (query: string) = GetStockDataWithQuery(BuildQuery(query))
