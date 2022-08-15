#### Imato.CLR.Logger
Log events in MS SQL CLR functions and procedures

#### Examples

Send log events to output
```csharp
using Microsoft.SqlServer.Server;
using System.Data.SqlClient;
using Imato.CLR.Logger;

public class Procedures
{
    [SqlProcedure]
    public static void SameProcesure(string param1)
    {
        using (var log = new Logger(LogLevel.Info))
        {
            log.Info($"Start SameProcesure with param1 = {param1}");
            try
            {
                var arr = new string[1];
                log.Info($"{arr[1]}");
            }
            catch(Exception ex)
            {
                log.Error(ex);
            }

            log.Info("End SameProcesure");
        }
    }
}
```
