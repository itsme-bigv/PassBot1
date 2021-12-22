using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using alps.net_api.ALPS.ALPSModelElements;
using alps.net_api.StandardPASS.InteractionDescribingComponents;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace CreateModelDialog.Actions
{
    public class CreateSubject : Dialog
    {
        [JsonConstructor]
        public CreateSubject([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }
        
        [JsonProperty("$kind")]
        public const string Kind = "CreateSubject";

        [JsonProperty("subjectType")]
        public StringExpression SubjectType { get; set; }

        [JsonProperty("subjectName")]
        public StringExpression SubjectName { get; set; }

        [JsonProperty("createdSubject")]
        public ISubject Subject { get; set; }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {

            ModelManagement management = ModelManagement.getInstance();
            IModelLayer layer = management.Model.getBaseLayer();
            dc.State.TryGetValue("user", out Object user);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("subjectType", out Object subjecttype);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("subjectName", out Object subjectname);

            this.Subject = null;
            
            if (subjecttype.ToString() == "Interface Subject")
            {
                 this.Subject = new InterfaceSubject(layer, subjectname.ToString());
            }
            else if (subjecttype.ToString() == "Fully Specified Subject")
            {
                this.Subject = new FullySpecifiedSubject(layer, subjectname.ToString());
                //FullySpecifiedSubject sub = new FullySpecifiedSubject(layer, subjectname.ToString());
            }
            else if (subjecttype.ToString() == "Multi Subject")
            {
                this.Subject = new MultiSubject(layer, subjectname.ToString());
                //MultiSubject sub = new MultiSubject(layer, subjectname.ToString());

            }

            //Create Subject and save it to state

            management.getSubjectCollection();
            string nameToAdd = (string)subjectname;
            if(management.subjectCollection.TryAdd(nameToAdd, Subject)) { }
            else
            {
                nameToAdd = nameToAdd + "(1)";
                management.subjectCollection.Add(nameToAdd, Subject);
            }
            return dc.EndDialogAsync(result: Subject, cancellationToken: cancellationToken);
        }
    }
}
