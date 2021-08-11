using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using alps.net_api;

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

        /*       private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
               {
                   foreach (var member in turnContext.Activity.MembersAdded)
                   {
                       if (member.Id != turnContext.Activity.Recipient.Id)
                       {
                           await turnContext.SendActivityAsync(
                               $"Welcome to SuggestedActionsBot {member.Name}",
                               cancellationToken: cancellationToken);
                           await SendSuggestedActionsAsync(turnContext, cancellationToken);
                       }
                   }
               }
               private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
               {
                   SubjectLoader loader = new SubjectLoader();
                   List<String> subjekte = loader.GetListOfAvailableSubjects();

                   var reply = MessageFactory.Text("Which subject is sending the message?");

                   reply.SuggestedActions = new SuggestedActions()
                   {
                       Actions = new List<CardAction>()
                       {
                           foreach (string sub in subjekte)
                           {
                               new CardAction() { Title = "sub" },
                           }
               }


               }


                   reply.SuggestedActions = new SuggestedActions()
                   {
                       Actions = new List<CardAction>()
                       {
                           new CardAction() { Title = "", Type = ActionTypes.ImBack, Value = "Red", Image = "https://via.placeholder.com/20/FF0000?text=R", ImageAltText = "R" },
                           new CardAction() { Title = "Yellow", Type = ActionTypes.ImBack, Value = "Yellow", Image = "https://via.placeholder.com/20/FFFF00?text=Y", ImageAltText = "Y" },
                           new CardAction() { Title = "Blue", Type = ActionTypes.ImBack, Value = "Blue", Image = "https://via.placeholder.com/20/0000FF?text=B", ImageAltText = "B"   },
                       },
                   };
                   await turnContext.SendActivityAsync(reply, cancellationToken);
               }
               */
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
