// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.
#load "MarkovText.fs"
#load "MarkovTextBuilder.fs"

open System.IO
open FsMarkov
open FsMarkov.MarkovText

// try different values of n
let nGramSize = 3
// get a sequence of n-grams from a text file
let corpus = File.ReadAllText "/Users/Taylor/Projects/Raw Text/Picture of Dorian Gray.txt"
let nGrams = getWordPairs nGramSize corpus
// build the Markov string transition map
let map = buildMarkovMap nGrams
let generator = MarkovTextBuilder(map)

// print a few sentences out
generator.GenerateSentences 3
|> joinWords
|> printfn "%A"