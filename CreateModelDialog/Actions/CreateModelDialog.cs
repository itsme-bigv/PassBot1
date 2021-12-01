using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using alps.net_api.ALPS.ALPSModelElements;
using alps.net_api.StandardPASS;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace CreateModelDialog
{
    public class CreateModelDialog : Dialog
    {
        [JsonConstructor]
        public CreateModelDialog([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "CreateModelDialog";

        //[JsonProperty("wishToCreate")]
        //public BoolExpression CreateModel { get; set; }

        [JsonProperty("createdModel")]
        public PASSProcessModel Model { get; set; }

        //TODO: update schema in CreateModelDialog and PassBot1
        [JsonProperty("modelName")]
        public StringExpression ModelName { get; set; }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {

                dc.State.TryGetValue("user", out Object user);
                ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("modelName", out Object modelName);

                //  string baseURI = ModelName.ToString();
                this.Model = new PASSProcessModel(string.Format("http://www.imi.kit.edu/{0}","bla"));

                //Set default 
                Model.addLayer(new ModelLayer(Model, "defaultLayer"));

                ModelManagement management = ModelManagement.getInstance();
                management.Model = this.Model;
            

            //Create Model and save it to state
            return dc.EndDialogAsync(result: Model, cancellationToken: cancellationToken);
        }
    }
}
