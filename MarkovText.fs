namespace FsMarkov

open System
open System.IO
open System.Text.RegularExpressions

module MarkovText = 
    let getRandomItem (rng : Random) seq = 
        let randIndex = rng.Next(Seq.length seq)
        seq |> Seq.nth randIndex
    
    let splitOnSpace text = Regex.Split(text, @"\s+")
    let joinWords words = String.concat " " words
    
    let combineWords prev next = 
        [ prev; next ]
        |> List.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        |> joinWords
    
    let getWordPairs pairSize filePath = 
        File.ReadAllText filePath
        |> splitOnSpace
        |> Seq.windowed pairSize
    
    let bisectWords (arr : _ []) = 
        let len = Array.length arr
        let preds = arr |> Seq.take (len - 1)
        (preds |> joinWords, arr.[len - 1])
    
    let updateMap (map : Map<_, _>) key value = 
        if map.ContainsKey key then 
            let existingValue = map.[key]
            let map = map |> Map.remove key
            map |> Map.add key (value :: existingValue)
        else map.Add(key, [ value ])
    
    let mapBuilder map words = bisectWords words ||> updateMap map
    let buildMarkovMap<'a> = Seq.fold mapBuilder Map.empty