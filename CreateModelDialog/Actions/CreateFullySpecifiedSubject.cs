using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using alps.net_api;

namespace CreateModelDialog.Actions
{
    public class CreateFullySpecifiedSubject : Dialog
    {
        [JsonConstructor]
        public CreateFullySpecifiedSubject([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "CreateFullySpecifiedSubject";

        [JsonProperty("wishToCreate")]
        public BoolExpression CreateModel { get; set; }

        [JsonProperty("createdModel")]
        public PassProcessModel Model { get; set; }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (CreateModel.GetValue(dc.State))
            {
                this.Model = new PassProcessModel();

                //Set default layer
                Model.addLayer("defaultLayer");
            }

            //Create Model and save it to state
            return dc.EndDialogAsync(result: Model, cancellationToken: cancellationToken);
        }
    }
}
