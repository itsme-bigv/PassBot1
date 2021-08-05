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
    public class CreateSubjectMessage : Dialog
    {
        [JsonConstructor]
        public CreateSubjectMessage([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "CreateSubject";

        [JsonProperty("messageName")]
        public StringExpression SubjectName { get; set; }

        [JsonProperty("sendingSubject")]
        public StringExpression SendingSubjectName { get; set; }

        [JsonProperty("receivingSubject")]
        public StringExpression ReceivingSubjectName { get; set; }

        [JsonProperty("createdMessage")]
        public IMessageExchange Message { get; set; }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {

            ModelManagement management = ModelManagement.getInstance();
            dc.State.TryGetValue("user", out Object user);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("messageName", out Object messageName);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("sendingSubject", out Object sendingSubject);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("receivingSubject", out Object receivingSubject);

            ISubject sender = new Subject();
            ISubject receiver = new Subject();
            bool senderSet = false;
            bool receiverSet = false;
            System.Collections.Generic.Dictionary<string, IPASSProcessModelElement> elements = management.Model.getModelLayer(0).getElements();
            foreach (IPASSProcessModelElement element in elements.Values){
                string[] splitName = element.getModelComponentID().Split("#");
                if (splitName.Length > 1)
                {
                    if (splitName[0] == sendingSubject.ToString())
                    {
                        sender = (ISubject)element;
                        senderSet = true;
                    }
                    else if (splitName[0] == receivingSubject.ToString())
                    {
                        receiver = (ISubject)element;
                        receiverSet = true;
                    }
                }
            }
            IMessageExchange messageExchange = new MessageExchange();
            if (receiverSet && senderSet)
            {
                messageExchange = new MessageExchange(messageName.ToString(), "", null, sender, receiver);
            }

            return dc.EndDialogAsync(result: messageExchange, cancellationToken: cancellationToken);
        }
    }
}
