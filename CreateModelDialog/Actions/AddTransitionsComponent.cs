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
    public class AddTransitionsComponent : ComponentDialog
    {

        [JsonProperty("$kind")]
        public const string Kind = "AddTransitionscomponent";

        ModelManagement management = ModelManagement.getInstance();
        IDictionary<string, ISubject> subjectCollection;

        IState inboundState;
        IState outboundState;
        ITransition transition;

        [JsonConstructor]
        public AddTransitionsComponent(string specifics,[CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        : base(nameof(AddTransitionsComponent))
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);

            WaterfallStep[] waterfallSteps;
            switch (specifics)
            {
                case "Coming from BehaviorEditing":
                    //add transitions based on adding states context
                    waterfallSteps = new WaterfallStep[]
                    {
                        ProcessOptionsAsync,
                        TransitionLabelingAsync,
                        AddTransitionAsync,
                    };
                    break;
                case "Only transitions":
                    //add transitions without context
                    waterfallSteps = new WaterfallStep[]
                    {
                        OutboundStateSelectionAsync,
                        InboundStateSelectionAsync,
                        TransitionLabelingAsync,
                        AddTransitionAsync,
                    };
                    break;
                    //default only neccessary to prevent compiler errors
                default:
                    waterfallSteps = new WaterfallStep[]
                    {
                        OutboundStateSelectionAsync,
                        InboundStateSelectionAsync,
                        TransitionLabelingAsync,
                        AddTransitionAsync,
                    };
                    break;
            }
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            InitialDialogId = nameof(WaterfallDialog);

            subjectCollection = management.getSubjectCollection();

        }

        private async Task<DialogTurnResult> ProcessOptionsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var _wayToGo = stepContext.Options;

            string wayToGo;
            string stateID="";
            
            if (_wayToGo is string[] stringy)
            {
                wayToGo = stringy[0];
                stateID = stringy.ElementAtOrDefault(1);

                if (stateID != null)
                {
                    if (management.Model.getAllElements()[stateID] is IState state) { inboundState = state; }
                    else { /*add interactive state search*/}
                }
            }
            
            else
            {
                wayToGo = "exit";
            }

            switch (wayToGo)
            {
                case "exit":
                    return await stepContext.EndDialogAsync(null, cancellationToken);
      
                case "state selection":
                    break;
            }

            inboundState.getContainedBy(out ISubjectBehavior defaultBehavior);

            IList<IState> stateList = new List<IState>(defaultBehavior.getBehaviorDescribingComponents().Values.OfType<IState>());
            IDictionary<string, IState> labels = new Dictionary<string, IState>();
            foreach (IState state in stateList)
            {
                if (state.getModelComponentID()!=stateID)
                {
                    string stateName = state.getModelComponentLabels()[0];

                    stateName = stateName.Remove(stateName.Length - 3);

                    labels.Add(stateName, state);
                }
            }



            PromptOptions pO = new PromptOptions
            {
                Prompt = MessageFactory.Text("Please choose a state as origin of the transition"),
                Choices = ChoiceFactory.ToChoices(labels.Keys.ToList()),
                Style = ListStyle.HeroCard
            };
            return await stepContext.PromptAsync(nameof(ChoicePrompt), pO); 
        }

        private async Task<DialogTurnResult> InboundStateSelectionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //    //TODO: potential problems: (0) labels are not unique, but dictionary keys must be AND (1) getModelComponentLabels()[0] might be empty
            //    //(0) if labels.TryAdd label ; else labels.Add ID
            //    //(1) if Labels.size =0, use ComponentID

            ISubjectBehavior defaultBehavior = null;
            if (stepContext.Options != null)
            {
                if (stepContext.Options is IFullySpecifiedSubject subject)
                {
                    defaultBehavior = subject.getSubjectBaseBehavior();
                }
            }

            IList<IState> stateList = new List<IState>(defaultBehavior.getBehaviorDescribingComponents().Values.OfType<IState>());
            IDictionary<string, IState> labels = new Dictionary<string, IState>();
            foreach (IState state in stateList) labels.Add(state.getModelComponentLabels()[0], state);

            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please choose a state as destination of the transition"),
                Choices = ChoiceFactory.ToChoices(labels.Keys.ToList()),
                Style = ListStyle.HeroCard
            });
        }

        private async Task<DialogTurnResult> OutboundStateSelectionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //    //TODO: potential problems: (0) labels are not unique, but dictionary keys must be AND (1) getModelComponentLabels()[0] might be empty
            //    //(0) if labels.TryAdd label ; else labels.Add ID
            //    //(1) if Labels.size =0, use ComponentID

            inboundState.getContainedBy(out ISubjectBehavior defaultBehavior);

            IList<IState> stateList = new List<IState>(defaultBehavior.getBehaviorDescribingComponents().Values.OfType<IState>());
            IDictionary<string, IState> labels = new Dictionary<string, IState>();
            foreach (IState state in stateList) labels.Add(state.getModelComponentLabels()[0], state);

            outboundState = labels[(string)stepContext.Result];
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please choose a state as origin of the transition"),
                Choices = ChoiceFactory.ToChoices(labels.Keys.ToList()),

            });
        }

        private async Task<DialogTurnResult> TransitionLabelingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            inboundState.getContainedBy(out ISubjectBehavior defaultBehavior);
            string outboundStateLabel = ((FoundChoice)stepContext.Result).Value;

            IList<IState> stateList = new List<IState>(defaultBehavior.getBehaviorDescribingComponents().Values.OfType<IState>());
            IDictionary<string, IState> labels = new Dictionary<string, IState>();

            foreach (IState state in stateList)
            {
                string ID=state.getModelComponentLabels()[0];
                if (ID.Contains(outboundStateLabel))
                {
                    outboundState = state;
                }
            }

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("Please label the transition")
            });
        }
        private async Task<DialogTurnResult> AddTransitionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var transitionLabel= (string)stepContext.Result;

            if (outboundState is DoState)
            {
                transition = new DoTransition(outboundState, inboundState, null, null, ITransition.TransitionType.Standard, 0, null, transitionLabel);
            }
            else if (outboundState is IReceiveState)
            {
                transition = new ReceiveTransition(outboundState, inboundState, null, null, ITransition.TransitionType.Standard,null,0,null,transitionLabel);
            }
            else if (outboundState is ISendState)
            {
                transition = new SendTransition(outboundState,inboundState,null,null,ITransition.TransitionType.Standard,null,null,transitionLabel);
            }

            return await stepContext.EndDialogAsync(null,cancellationToken);
        }
    }
}