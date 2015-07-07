// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "MarkovText.fs"
#load "MarkovTextBuilder.fs"

open FsMarkov
open FsMarkov.MarkovText

let map =
    getWordPairs 3 "/Users/Taylor/Projects/Raw Text/Picture of Dorian Gray.txt"
    |> buildMarkovMap

let builder = MarkovTextBuilder(map)
builder.GenerateSentences 3 |> joinWords |> printfn "%A"