using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using alps.net_api.parsing;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace CreateModelDialog
{
    public class ExportModel : Dialog
    {
        [JsonConstructor]
        public ExportModel([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "ExportModel";

        [JsonProperty("FileName")]
        public StringExpression File { get; set; }


        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {

            dc.State.TryGetValue("user", out Object user);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("modelName", out Object filename);

            ModelManagement management = ModelManagement.getInstance();
            
            PASSReaderWriter graph = PASSReaderWriter.getInstance();

            graph.exportModel(management.Model, filename.ToString());
            

            //Create Model and save it to state // useless in this class
            return dc.EndDialogAsync(result: management.Model, cancellationToken: cancellationToken);
        }
    }
}
