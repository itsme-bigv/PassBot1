using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using alps.net_api.StandardPASS.InteractionDescribingComponents;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace CreateModelDialog.Actions
{
    public class CreateMessageExchange : Dialog
    {
        [JsonConstructor]
        public CreateMessageExchange([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "CreateMessageExchange";

        [JsonProperty("messageTitle")]
        public StringExpression Title { get; set; }

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
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("messageTitle", out Object messageName);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("sendingSubject", out Object sendingSubject);
            ((System.Collections.Generic.Dictionary<string, object>)user).TryGetValue("receivingSubject", out Object receivingSubject);

            ISubject sender = null;
            ISubject receiver = null;
            bool senderSet = false;
            bool receiverSet = false;

            foreach (ISubject sub in management.Model.getAllElements().Values.OfType<ISubject>())
            {
                if (sub.getModelComponentID().Contains(sendingSubject.ToString()))
                {
                    sender = sub;
                    senderSet = true;
                }

                else if (sub.getModelComponentID().Contains(receivingSubject.ToString()))
                {
                    receiver = sub;
                    receiverSet = true;
                }
            }

            IMessageExchange messageExchange = null;
            if (receiverSet && senderSet)
            {
                messageExchange = new MessageExchange(management.Model.getBaseLayer(), messageName.ToString(), "", null, sender, receiver);

            }



            return dc.EndDialogAsync(result: messageExchange, cancellationToken: cancellationToken);
        }
        
    }
}
