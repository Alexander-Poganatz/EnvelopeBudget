namespace Domain

type EnvelopeTransaction = {
    AmountInCents: int;
    Date: string
    Description: string;
}

type Envelope = {
    Name: string;
    Transactions: EnvelopeTransaction list
}

type Command =
| AddEnvolope of Name:string
| DeleteEnvolope of Name:string
| AddTransaction of EnvolopeName:string * Value:int * Date:string * Description:string
| DeleteTransaction of EnvolopeName:string * ID:int
| ShowAccount of Name:string
| Summary
| Quit
| Save of SaveFunc:(string -> string -> bool) * FileName:string // function is file name then json
| Load of LoadFunc:(string -> bool * string) * Data:string
| Invalid

type CommandProcessResult =
| Success of Message:string
| Failure of Message:string
| ShowResult of EnvelopeTransaction list
| SummaryResult of (string * string) list // First value is envelope, second is dollar amount