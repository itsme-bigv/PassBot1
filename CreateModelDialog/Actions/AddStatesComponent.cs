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
using alps.net_api.StandardPASS.BehaviorDescribingComponents;
using alps.net_api.StandardPASS;

namespace CreateModelDialog.Actions
{
    public class AddStatesComponent : ComponentDialog
    {

        [JsonProperty("$kind")]
        public const string Kind = "AddStatesComponent";

        ModelManagement management = ModelManagement.getInstance();
        IDictionary<string, ISubject> subjectCollection;
        IFullySpecifiedSubject subject;
        IState state;
        ISubjectBehavior behavior;
        string s = "stateChoice";
        string d = "stateDescription";

        bool containsBehavior = false;
        bool subjectSet = false;

        [JsonConstructor]
        public AddStatesComponent([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        : base(nameof(AddStatesComponent))
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                StateTypeAsync,
                StateDescriptionAsync,
                SummaryStepAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);

            subjectCollection = management.getSubjectCollection();
        }

        private async Task<DialogTurnResult> StateTypeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            IList<string> stateTypes = new List<string>();
            stateTypes.Add("Do state");
            stateTypes.Add("Send state");
            stateTypes.Add("Receive state");

            if (stepContext.Options is ISubjectBehavior subjectBehavior)
            {
                behavior = subjectBehavior;
            }

            else
            {
                await stepContext.Context.SendActivityAsync("Can't find the behavior for this subject. please try again.");
                return await stepContext.EndDialogAsync();
            }

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please choose a state type"),
                Choices = ChoiceFactory.ToChoices(stateTypes),
                Style = ListStyle.HeroCard
            }, cancellationToken);

            //return await stepContext.ContinueDialogAsync();
        }

        private async Task<DialogTurnResult> StateDescriptionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            stepContext.Values[s] = ((FoundChoice)stepContext.Result).Value;
            switch (stepContext.Values[s])
            {
                case "Do state":
                    state = new DoState(behavior);
                    break;
                case "Send state":
                    state = new SendState(behavior);
                    break;
                case "Receive state":
                    state = new ReceiveState(behavior);
                    break;
                default:
                    await stepContext.Context.SendActivityAsync($"I can't add a state named {stepContext.Values[s]}. Please try again");
                    await stepContext.ReplaceDialogAsync(nameof(AddStatesComponent));
                    break;
            }
            
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text($"Please briefly describe this {stepContext.Values[s]}") });
        }

        private async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values[d] = (string)stepContext.Result;
            state.addModelComponentLabel(stepContext.Values[d].ToString());
            await stepContext.Context.SendActivityAsync($"I added a {stepContext.Values[s]} with this description: {stepContext.Values[d]}");

            return await stepContext.EndDialogAsync(state, cancellationToken);
        }
    }
}