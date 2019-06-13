module Tests

open System
open Xunit
open StockChartBot.Parser

type TestQuery = 
    {
        TextQuery: string
        Sender : string
        Ticker : string
        From : DateTime
        Until : DateTime
    }

type ClassDataBase(generator : TestQuery [] seq) = 
    interface seq<TestQuery []> with
        member this.GetEnumerator() = generator.GetEnumerator()
        member this.GetEnumerator() = 
            generator.GetEnumerator() :> System.Collections.IEnumerator

let validValues : TestQuery array seq = 
    seq { 
        yield [| { TextQuery = "@twitter-account-name LNKD 1/1/2000 04/16/2019";
                  Sender = "@twitter-account-name";
                  Ticker = "LNKD";
                  From = DateTime(2000, 1, 1);
                  Until = DateTime(2019, 4, 16) } |] 
        yield [| { TextQuery = "@another-account LNKD 1/1/2000 04/16/2019";
                  Sender = "@another-account";
                  Ticker = "LNKD";
                  From = DateTime(2000, 1, 1);
                  Until = DateTime(2019, 4, 16) } |] 
        yield [| { TextQuery = "@another-account MSFT 1/1/2000 04/16/2019";
                  Sender = "@another-account";
                  Ticker = "MSFT";
                  From = DateTime(2000, 1, 1);
                  Until = DateTime(2019, 4, 16) } |] 
        yield [| { TextQuery = "@another-account MSFT 2007";
                  Sender = "@another-account";
                  Ticker = "MSFT";
                  From = DateTime(2007, 1, 1);
                  Until = DateTime(2007, 12, 31) } |] 
    }

type ValidTestCases() = 
    inherit ClassDataBase(validValues)

[<Theory; ClassData(typeof<ValidTestCases>)>]
let ``Parser retrieves data in a proper manner`` (testCase) =
    let expected = 
        {
            Sender = testCase.Sender
            Ticker = testCase.Ticker
            From = testCase.From
            Until = testCase.Until
        } |> Outcome.Success
    let actual = Parse testCase.TextQuery
    Assert.Equal(expected, actual);

[<Theory>]
[<InlineData("", "Input is of wrong format")>]
[<InlineData("@twitter APPL year", "Error occured Input string was not in a correct format.")>]
[<InlineData("@gibberish incorrect no date", "Error occured String 'no' was not recognized as a valid DateTime.")>]
let ``Parser handles invalid text`` (input, expectation) =
    let expected = Outcome.Failure expectation
    let actual = Parse input
    Assert.Equal(expected, actual);