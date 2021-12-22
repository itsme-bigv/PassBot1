using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using alps.net_api.ALPS.ALPSModelElements;
using alps.net_api.StandardPASS;
using alps.net_api.StandardPASS.InteractionDescribingComponents;
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

        [JsonProperty("modelName")]
        public StringExpression ModelName { get; set; }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {

                dc.State.TryGetValue("user", out Object user);
                ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("modelName", out Object modelName);

                //  string baseURI = ModelName.ToString();
                this.Model = new PASSProcessModel(string.Format("http://www.imi.kit.edu/{0}",modelName.ToString()));
            
                ModelManagement management = ModelManagement.getInstance();
                management.Model = this.Model;

            //For testing purposes
            if (modelName.ToString()=="test")
            {
                IModelLayer layer = management.Model.getBaseLayer();

                //create testing subjects
                ISubject multiSub = new MultiSubject(layer, "multiSub");
                ISubject fullySub = new FullySpecifiedSubject(layer, "fullySub");
                ISubject interSub = new MultiSubject(layer, "interSub");
                management.getSubjectCollection();


                management.subjectCollection.Add("multiSub", multiSub);
                management.subjectCollection.Add("interSub", interSub);
                management.subjectCollection.Add("fullySub", fullySub);

                //create testing message exchange
                MessageSpecification msgSpecification = new MessageSpecification(management.Model.getBaseLayer(), null, null, null, "this is a test message");
                var messageExchange = new MessageExchange(management.Model.getBaseLayer(), "this is for testing purposes",msgSpecification, fullySub, interSub);

                //create testing states (TODO: to be added, first make sure addings states works properly)

            }
            

            //Create Model and save it to state
            return dc.EndDialogAsync(result: Model, cancellationToken: cancellationToken);
        }
    }
}
