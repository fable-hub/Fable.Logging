namespace Fable.Logging

open System.Collections.Generic

module Common =
    // Translate format string from named placeholders to final string.
    // Scans left-to-right for {Name} placeholders, replaces each with the
    // corresponding arg by index. Uses simple string ops instead of
    // Regex/String.Format for BEAM compatibility.
    let translateFormat (categoryName: string) (format: string) (args: obj array) =
        let parameters = Dictionary<string, obj>()
        let result = System.Text.StringBuilder()
        let mutable i = 0
        let mutable argIndex = 0

        while i < format.Length do
            if format.[i] = '{' then
                let afterOpen = format.Substring(i + 1)
                let closeIdx = afterOpen.IndexOf('}')

                if
                    closeIdx > 0
                    && argIndex < args.Length
                then
                    let name = afterOpen.Substring(0, closeIdx)
                    let value = args.[argIndex]
                    parameters[name] <- value
                    result.Append(string value) |> ignore
                    argIndex <- argIndex + 1
                    i <- i + closeIdx + 2
                else
                    result.Append(format.[i]) |> ignore
                    i <- i + 1
            else
                result.Append(format.[i]) |> ignore
                i <- i + 1

        let formatted = result.ToString()

        let event =
            if categoryName.Length > 0 then
                categoryName + " - " + formatted
            else
                formatted

        parameters["event"] <- event
        parameters["category_name"] <- categoryName
        event, parameters
