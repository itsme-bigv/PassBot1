using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CreateModelDialog.Actions;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;


    public class SubjectSuggestions : Dialog
    {
        [JsonConstructor]
        public SubjectSuggestions([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);

        }

        [JsonProperty("$kind")]
        public const string Kind = "SubjectSuggestions";

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {

            return await dc.BeginDialogAsync(nameof(SubjectSuggestionComponent));
        }



    }


