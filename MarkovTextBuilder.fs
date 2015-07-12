namespace FsMarkov

open System
open System.Text.RegularExpressions
open FsMarkov.MarkovText

type MarkovTextBuilder(map : Map<string, string list>, ?rng, ?isStartWord, ?isEndWord) = 
    let mutable rng = defaultArg rng (Random())
    let mutable isStartSentence = defaultArg isStartWord (Regex(@"^[A-Z]").IsMatch)
    let mutable isEndSentence = defaultArg isEndWord (Regex(@"(?<!Mr(s)?)[\.\?\!]$").IsMatch)
    let startWords, otherWords = map |> Map.partition (fun k _ -> isStartSentence k)
    
    let startWordArray = 
        map
        |> Seq.map (fun kvp -> kvp.Key)
        |> Array.ofSeq
    
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
    
    let getMarkovSentence startWord = 
        markovChain startWord [ startWord ]
        |> List.rev
        |> joinWords
    
    member x.GenerateSentences count = 
        seq { 
            for i in 1..count do
                yield getMarkovSentence (getRandomItem rng startWords).Key
        }