using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using alps.net_api.StandardPASS;
using alps.net_api.StandardPASS.BehaviorDescribingComponents;
using alps.net_api.StandardPASS.InteractionDescribingComponents;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Newtonsoft.Json;

namespace CreateModelDialog.Actions
{
    public class BehaviorEditingComponent : ComponentDialog
    {

        [JsonProperty("$kind")]
        public const string Kind = "BehaviorEditing";

        ModelManagement management = ModelManagement.getInstance();
        IDictionary<string, ISubject> subjectCollection;
        IFullySpecifiedSubject subject;
        IState addedState;
        ISubjectBehavior defaultBehavior;

        bool containsBehavior = false;
        bool subjectSet;
        WaterfallStepContext backUpContext;

        [JsonConstructor]
        public BehaviorEditingComponent([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        : base(nameof(BehaviorEditingComponent))
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ChooseSubjectAsync,
                SubjectCheck,
                ChooseEntryPointAsync,
                ContinueDescribingAsync,
                TransitioningAsync,
                MoreStatesAsync,
                EndOrReRunAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new SubjectSuggestions());
            AddDialog(new SubjectSuggestionComponent("Fully specified subjects","",0));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new AddStatesComponent());
            AddDialog(new AddTransitionsComponent("Coming from BehaviorEditing","",0));
            AddDialog(new InterruptionDialog());
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = nameof(WaterfallDialog);

            subjectCollection = management.getSubjectCollection();
        }
        private async Task<DialogTurnResult> ChooseSubjectAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Which subject's behavior do you want to edit?");

            return await stepContext.BeginDialogAsync(nameof(SubjectSuggestionComponent), null, cancellationToken);
        }
        
        private async Task<DialogTurnResult> SubjectCheck(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string s = "subjectToBeEdited";
            stepContext.Values[s] = ((FoundChoice)stepContext.Result).Value;
            subjectSet = false;

            foreach (IFullySpecifiedSubject sub in management.subjectCollection.Values.OfType<IFullySpecifiedSubject>())
            {
                if (sub.getModelComponentID().Contains((string)stepContext.Values[s]))
                {
                    subject = sub;
                    subjectSet = true;
                }
            }

            if (subjectSet)
            {
                await stepContext.Context.SendActivityAsync($"let's talk about {(string)stepContext.Values[s]}'s behavior");
            }

            else
            {
                await stepContext.Context.SendActivityAsync($"Cant't find {(string)stepContext.Values[s]}. Please try again with another subject. [skipping the rest of this dialog]");
                await stepContext.EndDialogAsync();
            }
            //this ContinueDialogAsync method works properly only on the highest "klammer" of a waterfall step 
            return await stepContext.NextAsync(null,cancellationToken);
        }

        private async Task<DialogTurnResult> ChooseEntryPointAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            defaultBehavior = subject.getSubjectBaseBehavior();
            stepContext.Values["behavior"] = defaultBehavior;

            if (defaultBehavior.getBehaviorDescribingComponents().Any())
            {
                containsBehavior = true;
            }
            else
            {
                containsBehavior = false;
            }

            if (!containsBehavior)
            {
                await stepContext.Context.SendActivityAsync($"{(string)stepContext.Values["subjectToBeEdited"]} does not specify any behavior yet. What's the first action?");

            }

            return await stepContext.BeginDialogAsync(nameof(AddStatesComponent), defaultBehavior, cancellationToken);
        }

        private async Task<DialogTurnResult> ContinueDescribingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //use result of child dialog (AddStatesComponent)

            if (stepContext.Result is IState addState)
            {
                addedState = addState;
            }
            string msg;
            if (!containsBehavior)
            {
                msg = "This was the first state. do you want to make it start state?";
            }
            else
            {
                msg = "Do you want to connect this state to a previously defined state?";
            }
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text(msg),
                Choices = new[] { new Choice { Value = "yes" }, new Choice { Value = "no" } }.ToList()
            });
        }

        private async Task<DialogTurnResult> TransitioningAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string[] option = new string[] { "exit",null };
            if (stepContext.Result is bool choices)
            {
                if (!containsBehavior)
                {
                    if (choices)
                    {
                        //make it start state (same way to make end state)
                        addedState.setIsStateType(IState.StateType.InitialStateOfBehavior);
                    }
                }
                if (containsBehavior)
                {
                    if (choices)
                    {
                        option[0] = "state selection";
                        option[1] = addedState.getModelComponentID();
                    }
                }
            }
            return await stepContext.BeginDialogAsync(nameof(AddTransitionsComponent), options: option, cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> MoreStatesAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("debugging reasons"));
            backUpContext = stepContext;

            PromptOptions promptOptions = new PromptOptions
            {
                Prompt = MessageFactory.Text("Do you want to tell me what the subject is doing next?"),
                Choices = new[] { new Choice { Value = "yes" }, new Choice { Value = "no" } }.ToList(),
                Style = ListStyle.HeroCard
            };

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), promptOptions,cancellationToken);
        }

        private async Task<DialogTurnResult> EndOrReRunAsync(WaterfallStepContext stepContext,CancellationToken cancellationToken)
        {
            if (stepContext.Result is bool choices)
            {
                if (choices)
                {
                    //return await stepContext.ReplaceDialogAsync(nameof(BehaviorEditingComponent),null, cancellationToken);
                }
            }
            //return await stepContext.NextAsync(null, cancellationToken);
            //TODO how to not restart this dialog?
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> DefinetelyEndAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken: cancellationToken);
        }
    }
}