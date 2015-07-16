namespace FsMarkov

open System
open System.Text.RegularExpressions
open FsMarkov.MarkovText

/// Generates Markov sentences using a map of Markov string transitions.
type MarkovTextBuilder(map : Map<string, string list>, ?rng : int -> int, ?isStartWord, ?isEndWord) = 
    let rng = defaultArg rng (Random().Next)
    // determines if an n-gram can be used as the beginning of a sentence
    let isStartSentence = defaultArg isStartWord (Regex(@"^""?[A-Z]").IsMatch)
    // determines if an n-gram can be used as the end of a sentence
    let isEndSentence = defaultArg isEndWord (Regex(@"(?<!Mr(s)?)[\.\?\!]""?$").IsMatch)
    // partition n-grams into "sentence starters" and everything else
    let startWords, otherWords = map |> Map.partition (fun k _ -> isStartSentence k)
    
    // array of n-grams that can be used to start a sentence
    let startWordArray = 
        map
        |> Seq.map (fun kvp -> kvp.Key)
        |> Array.ofSeq
    
    // recursive state+accumulator function to construct a markov chain
    // until the stop condition is met, i.e. isEndSentence
    let rec markovChain state acc = 
        let nextChoice = map.[state] |> getRandomItem rng
        if isEndSentence nextChoice then nextChoice :: acc
        else 
            let currWords = 
                state
                |> splitOnSpace
                |> Seq.skip 1
                |> joinWords
            markovChain (combineWords currWords nextChoice) (nextChoice :: acc)
    
    // constructs a markov sentence given a particular start n-gram
    let getMarkovSentence startWord = 
        markovChain startWord [ startWord ]
        |> List.rev
        |> joinWords
    
    /// Generates a sequence of Markov sentences.
    member x.GenerateSentences count = 
        seq { 
            for i in 1..count do
                yield getMarkovSentence (getRandomItem rng startWords).Key
        }