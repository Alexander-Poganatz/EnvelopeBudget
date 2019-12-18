module Operations

open System
open Domain

let IntRegex = Text.RegularExpressions.Regex("^[\-]?\d+$")
let MoneyRegex = Text.RegularExpressions.Regex("^[\-]?\d+\.\d\d?$")
let YYYYMMDDRegex = Text.RegularExpressions.Regex("^\d{4}-\d{2}-\d{2}$")

let DollarStringToCents str = 
    if String.IsNullOrEmpty(str) then 
        None
    elif IntRegex.Match(str).Success then
        Some(Convert.ToInt32(str) * 100)
    elif MoneyRegex.Match(str).Success then
        let bothSides = str.Split('.')
        let rs = Convert.ToInt32(bothSides.[1])
        let rightSide = 
            if bothSides.[1].Length = 1 && rs < 10 then 
                rs * 10
            else rs
        Some((Convert.ToInt32(bothSides.[0]) * 100) + rightSide)
    else 
        None

let CentsToDollarString cents =
    let rs = cents % 100
    let rightSideString = if rs < 10 then String.Format("0{0}", rs) else rs.ToString()
    String.Format("{0}.{1}", cents / 100, rightSideString )

let CheckValidDate str = YYYYMMDDRegex.Match(str).Success && fst (DateTime.TryParse(str)) = true

let private findEnvelope (state:Envelope list) name =
    List.tryFind(fun e -> e.Name = name) state

let ExecuteCommandOnState (state:Envelope list) command =
    match command with
    | Invalid -> 
        Failure("Invalid command"), state
    | AddEnvolope name ->
        let existing = findEnvelope state name
        match existing with
        | None ->
            let newE = { Name = name; Transactions = []}
            Success(sprintf "Added Envelope %s" name), newE :: state
        | Some e ->
            Failure(sprintf "Envolope %s already exists" e.Name),state
    | DeleteEnvolope name ->
        let existing = findEnvelope state name
        match existing with
        | None ->
            Failure(sprintf "Envelope %s does not exists" name), state
        | Some e -> 
            Success(sprintf "Deleted %s" name), List.filter(fun f -> f.Name <> e.Name) state
    | AddTransaction (eName, value, date, desc) ->
        let envelope = findEnvelope state eName
        match envelope with
        | None -> 
            Failure(sprintf "%s is not a valid envelope name" eName),state
        | Some envelope ->
            let transaction = {AmountInCents = value; Date = date; Description = desc}
            let newTList = envelope.Transactions @ [transaction]
            Success(sprintf "Added transaction"), List.map(fun e -> if e.Name = eName then {e with Transactions = newTList} else e) state
    | DeleteTransaction (ename, index) -> 
        let envelope = findEnvelope state ename
        match envelope with
        | None -> 
            Failure(sprintf "Envelope %s does not exist" ename), state
        | Some envelope ->
            if index < 0 || index >= envelope.Transactions.Length then
                Failure("Transaction index is out of range"), state
            else
                let newTList =
                    envelope.Transactions
                    |> List.indexed
                    |> List.filter(fun (i, t) -> i <> index)
                    |> List.map( fun (i, t) -> t)
                Success("Removed transaction"),List.map(fun e -> if e.Name = ename then {e with Transactions = newTList} else e) state
    | ShowAccount ename ->
        let envelope = findEnvelope state ename
        match envelope with
        | None -> Failure(sprintf "Envelope %s does not exist" ename), state
        | Some envelope ->ShowResult(envelope.Transactions),state
    | Summary ->
        let sumByToDollars (transactions : EnvelopeTransaction list) =
            let sum = List.sumBy(fun t -> t.AmountInCents) transactions
            CentsToDollarString sum
        SummaryResult(List.map(fun envelope -> envelope.Name, (sumByToDollars envelope.Transactions)) state),state
    | Save (saveFunc, fileName) ->
        let json = Newtonsoft.Json.JsonConvert.SerializeObject(state)
        let saveResult = saveFunc fileName json
        match saveResult with
        | true -> Success("File saved"), state
        | false -> Failure("Unable to save file"), state
    | Load (loadFunc, fileName) ->
        let loadResult = loadFunc fileName
        match (fst loadResult) with
        | true ->
            let newState = Newtonsoft.Json.JsonConvert.DeserializeObject<Envelope list>(snd loadResult)
            Success("Loaded data"), newState
        | false -> Failure("No data"), state
    | Quit -> Success(""),state