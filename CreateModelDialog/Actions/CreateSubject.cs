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
            dc.State.TryGetValue("user", out Object user);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("subjectType", out Object subjecttype);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("subjectName", out Object subjectname);

            if (subjecttype.ToString() == "Interface Subject")
            {
                this.Subject = management.Model.getModelLayer(0).addInterfaceSubject(subjectname.ToString() + "#");
            }
            else if (subjecttype.ToString() == "Fully Specified Subject")
            {
                this.Subject = management.Model.getModelLayer(0).addFullySpecifiedSubject(subjectname.ToString() + "#");

            }
            else if (subjecttype.ToString() == "Multi Subject")
            {
                this.Subject = management.Model.getModelLayer(0).addMultiSubject(subjectname.ToString() + "#");
            }

                //Create Subject and save it to state
                return dc.EndDialogAsync(result: Subject, cancellationToken: cancellationToken);
        }
    }
}
