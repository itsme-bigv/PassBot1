using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using CreateModelDialog;

namespace CreateModelDialog.Actions
{
    public class InterruptionDialog : Dialog
    {

        [JsonProperty("$kind")]
        public const string Kind = "Interruption";


        [JsonConstructor]
        public InterruptionDialog([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dc.CancelAllDialogsAsync(cancellationToken: cancellationToken);
        }
    }
}
