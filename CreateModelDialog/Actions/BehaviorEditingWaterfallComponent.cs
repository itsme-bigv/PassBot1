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

namespace CreateModelDialog.Actions
{
    public class BehaviorEditingWaterfallComponent : ComponentDialog
    {

        [JsonProperty("$kind")]
        public const string Kind = "BehaviorEditing";

        ModelManagement management = ModelManagement.getInstance();
        IDictionary<string, ISubject> subjectCollection;
        IFullySpecifiedSubject subject;
        IState doState;

        bool containsBehavior = false;
        bool subjectSet=false;

        [JsonConstructor]
        public BehaviorEditingWaterfallComponent([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        : base(nameof(BehaviorEditingWaterfallComponent))
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                ChooseSubjectAsync,
                SubjectCheck,
                ChooseEntryPointAsync,


            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new SubjectSuggestions());
            AddDialog(new SubjectSuggestionComponent("Fully specified subjects","",0));
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            InitialDialogId = nameof(WaterfallDialog);

            subjectCollection = management.getSubjectCollection();

        }
        private async Task<DialogTurnResult> ChooseSubjectAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync("Which subject's behavior do you want to edit?");

            //TODO: can also select subjects that dont specify any behavior. how to fix? Can I call SubjectSuggestionComponent directly?
            return await stepContext.BeginDialogAsync(nameof(SubjectSuggestionComponent));
        }
        
        private async Task<DialogTurnResult> SubjectCheck(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string s = "subjectToBeEdited";
            stepContext.Values[s] = ((FoundChoice)stepContext.Result).Value;

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
                return await stepContext.ContinueDialogAsync(cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"Cant't find {(string)stepContext.Values[s]}. Please try again with another subject. [skipping the rest of this dialog]");
                return await stepContext.EndDialogAsync();
            }
            
        }

        private async Task<DialogTurnResult> ChooseEntryPointAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var defaultBehavior = subject.getSubjectBaseBehavior();
            
            if (defaultBehavior.getBehaviorDescribingComponents().Any())
            {
                containsBehavior = true;
            }
            else
            {
                containsBehavior = false;
            }

            if (containsBehavior)
            {
                
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"{(string)stepContext.Values["subjectToBeEdited"]} does not specify any behavior yet. What does it do first?");

                doState = new DoState(defaultBehavior);

                await stepContext.Context.SendActivityAsync($"This was the first state. do you want to make it start state?");

                //TODO: make it start state (same way to make end state)
                doState.setIsStateType(IState.StateType.InitialStateOfBehavior);
            }

            //TODO: ask for state and inbound transition
            await stepContext.Context.SendActivityAsync($"which is the preceeding state?");

            //TODO: potential problems: (0) labels are not unique, but dictionary keys must be AND (1) getModelComponentLabels()[0] might be empty
            //(0) if labels.TryAdd label ; else labels.Add ID
            //(1) if Labels.size =0, use ComponentID
            IList<IState> stateList = new List<IState>(defaultBehavior.getBehaviorDescribingComponents().Values.OfType<IState>());
            IDictionary<string,IState> labels = new Dictionary<string,IState>();
            foreach (IState state in stateList) labels.Add(state.getModelComponentLabels()[0],state);

            if (doState is IDoState myDostate)
            {
                ITransition transition = new DoTransition(defaultBehavior);
                myDostate.addOutgoingTransition(transition);
            }

            
            return await stepContext.EndDialogAsync();
        }
    }
}
