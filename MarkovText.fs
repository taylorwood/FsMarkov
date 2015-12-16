namespace FsMarkov

open System
open System.Text.RegularExpressions

module MarkovText = 
    /// Picks a random item from a sequence.
    let getRandomItem (rng : int -> int) seq = 
        let randIndex = rng (Seq.length seq)
        seq |> Seq.item randIndex
    
    /// Splits a string by whitespace into a string array
    let splitOnSpace text = Regex.Split(text, @"\s+")
    
    /// Concatenates a sequence of strings using a whitespace separator.
    let joinWords words = String.concat " " words
    
    /// Joins two n-grams together, allowing for either to be null or whitespace.
    let combineWords prev next = 
        [ prev; next ]
        |> List.filter (fun s -> not (String.IsNullOrWhiteSpace s))
        |> joinWords
    
    /// Reads a sequence of n-grams from a text file.
    let getWordPairs pairSize = splitOnSpace >> Seq.windowed pairSize
    
    /// Returns a string tuple from a string array. The first item is every word
    /// but the last, joined as a single string. The second item is the last word
    /// in the array.
    let bisectWords (arr : _ []) = 
        let len = arr.Length
        let preds = arr |> Seq.take (len - 1)
        (preds |> joinWords, arr.[len - 1])
    
    /// Adds or updates a map binding, returning a new map.
    let updateMap (map : Map<_, _>) key value = 
        if map.ContainsKey key then 
            let existingValue = map.[key]
            let map = map |> Map.remove key
            map |> Map.add key (value :: existingValue)
        else map.Add(key, [ value ])
    
    // fold function for constructing a map from the n-gram sequence
    let mapBuilder map words = bisectWords words ||> updateMap map
    
    /// Constructs a Markov text map from a sequence of n-grams.
    let buildMarkovMap<'a> = Seq.fold mapBuilder Map.empty