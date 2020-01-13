

open System
open Domain
open Operations

let DEFAULT_FILE_NAME = "./budget.json"

let saveFunc fileName json =
    try
        System.IO.File.WriteAllText(fileName, json);
        true
    with
     | _ -> false

let loadFile fileName = 
    try
        true, System.IO.File.ReadAllText(fileName)
    with
    | _ -> false, ""

let parseLine (line:string) =
    let line = line.Trim()
    let argsWithWhiteSpace = line.Split(' ') |> Array.toList // splitted the white space arg out for dealing with files
    let args = argsWithWhiteSpace |> List.filter(fun str -> String.IsNullOrWhiteSpace str = false)
    match args with
    | [] -> Invalid
    | head::tail ->
        match head.ToLower() with
        | "summary" | "sum" -> Summary
        | "quit" | "exit" | "e" | "q" -> Quit
        | "add" ->
                    if tail.Length >= 4 then
                        let cents = DollarStringToCents tail.[1]
                        let isValidDate = CheckValidDate tail.[2]
                        let description = String.Join(" ", tail.[3..])
                        if cents.IsSome && isValidDate then
                            AddTransaction(tail.[0],cents.Value,tail.[2], description)
                        else Invalid
                    else Invalid
        | "new" -> 
            if tail.Length = 0 then Invalid else AddEnvolope(tail.Head)
        | "show" -> 
            if tail.Length = 0 then Invalid else ShowAccount(tail.Head)
        | "remove" ->
            if tail.Length < 2 then Invalid
            else if IntRegex.Match(tail.[1]).Success = false then Invalid
            else DeleteTransaction(tail.Head, Convert.ToInt32(tail.[1]))
        | "delete_e" -> 
            if tail.Length <> 1 then Invalid else DeleteEnvolope(tail.Head)
        | "load" ->
            // I was originally going to get file names by just substringing the line at the length of the head but
            // System.IO.File methods didn't find the files when I did it that way.
            let fileName = if tail.Length >= 1 then String.Join(' ', argsWithWhiteSpace.Tail).Trim() else DEFAULT_FILE_NAME
            Load(loadFile, fileName)
        | "save" | "\u0013" -> // On windows console, 0013
            let fileName = if tail.Length >= 1 then tail.Head else DEFAULT_FILE_NAME
            Save(saveFunc, fileName)
        | _ -> Invalid
        


let handleCommand state command =
    let executeResult = ExecuteCommandOnState state command
    match (fst executeResult) with
    | Success message -> printfn "%s" message
    | Failure message -> printfn "%s" message
    | ShowResult transactionList ->
        transactionList 
        |> List.mapi(fun index tr -> index, tr)
        |> List.sortBy(fun (_, tr) -> tr.Date)
        |> List.iter(fun (index, tr) -> printfn "%d, %s %s %s" index (CentsToDollarString(tr.AmountInCents)) tr.Date tr.Description )
    | SummaryResult list ->
        List.iter(fun tup -> printfn "%s: %s" (fst tup) (snd tup) ) list
    snd executeResult


let getInput() =
    seq {
        while true do
            yield Console.ReadLine()
    }

[<EntryPoint>]
let main argv =
    printfn "Welcome to Envelope Budget App"

    let initialStateResult = ExecuteCommandOnState [] (Load(loadFile, DEFAULT_FILE_NAME))
    let initialState = snd initialStateResult

    match (fst initialStateResult) with
    | Success message -> printfn "Loaded default file"
    | Failure message -> printfn "Started app with no loaded data"
    | _ -> printf "Unknown startup state"

    let takeWhileFilter command =
        match command with
        | Quit -> false
        | _ -> true

    getInput()
    |> Seq.map parseLine
    |> Seq.takeWhile takeWhileFilter
    |> Seq.fold handleCommand initialState
    |> ignore

    0 // return an integer exit code
