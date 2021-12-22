using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using alps.net_api.StandardPASS.InteractionDescribingComponents;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Newtonsoft.Json;
using CreateModelDialog;

namespace CreateModelDialog.Actions
{
    public class CreateMessageExchangeComponent : ComponentDialog
    {

        [JsonProperty("$kind")]
        public const string Kind = "CreateMessageExchange";

        ISubject sender;
        ISubject receiver;

        ModelManagement management = ModelManagement.getInstance();
        IDictionary<string, ISubject> subjectCollection;

        [JsonConstructor]
        public CreateMessageExchangeComponent([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        : base(nameof(CreateMessageExchangeComponent))
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                getSenderAsync,
                getReceiverAsync,
                getMessageNameAsync,
                summaryStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new SubjectSuggestions());
            AddDialog(new SubjectSuggestionComponent());

            InitialDialogId = nameof(WaterfallDialog);

            subjectCollection = management.getSubjectCollection();
        }


        private async Task<DialogTurnResult> getSenderAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Who is sending the message?");
            
            return await stepContext.BeginDialogAsync(nameof(SubjectSuggestions));
        }

        private async Task<DialogTurnResult> getReceiverAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            sender = null;

            stepContext.Values["sendingSubject"] = ((FoundChoice) stepContext.Result).Value;

            await stepContext.Context.SendActivityAsync("Who is receiving the message?");

            return await stepContext.BeginDialogAsync(nameof(SubjectSuggestions));
        }

        private async Task<DialogTurnResult> getMessageNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            receiver = null;

            stepContext.Values["receivingSubject"] = ((FoundChoice)stepContext.Result).Value;            

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("What is the message?") });
        }


        private async Task<DialogTurnResult> summaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["messageName"] = (string)stepContext.Result;

            bool senderSet = false;
            bool receiverSet = false;

            foreach (ISubject sub in management.subjectCollection.Values)
            {
                if (sub.getModelComponentID().Contains((string)stepContext.Values["sendingSubject"]))
                {
                    sender = sub;
                    senderSet = true;
                }

                else if (sub.getModelComponentID().Contains((string)stepContext.Values["receivingSubject"]))
                {
                    receiver = sub;
                    receiverSet = true;
                }
            }

            IMessageExchange messageExchange = null;
            if (receiverSet && senderSet)
            {
                MessageSpecification msgSpecification = new MessageSpecification(management.Model.getBaseLayer(), null, null, null, (string)stepContext.Values["messageName"]);
                messageExchange = new MessageExchange(management.Model.getBaseLayer(), $"messageFrom{(string)stepContext.Values["sendingSubject"]}To{(string)stepContext.Values["receivingSubject"]}",
                     msgSpecification, sender, receiver);
            }
            
            await stepContext.Context.SendActivityAsync($"{(string)stepContext.Values["sendingSubject"]} sends {(string)stepContext.Values["receivingSubject"]} this message: {(string)stepContext.Values["messageName"]}");
            return await stepContext.EndDialogAsync(result: messageExchange, cancellationToken: cancellationToken);
        }
    }
}
        
            






 
          
