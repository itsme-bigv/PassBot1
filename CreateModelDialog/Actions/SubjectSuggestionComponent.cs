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

namespace CreateModelDialog.Actions
{
    public class SubjectSuggestionComponent : ComponentDialog
    {

        [JsonProperty("$kind")]
        public const string Kind = "SubjectSuggestions";
        
        [JsonConstructor]
        public SubjectSuggestionComponent([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        : base(nameof(SubjectSuggestionComponent))
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);

            WaterfallStep[] waterfallSteps = new WaterfallStep[]
            {
                SubjectChoiceAsync,
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));


            InitialDialogId = nameof(WaterfallDialog);

            

        }

        [JsonConstructor]
        public SubjectSuggestionComponent(string specifics,[CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
: base(nameof(SubjectSuggestionComponent))
        {
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
            WaterfallStep[] waterfallSteps;
            switch (specifics)
            {
                case "Interface subjects":
                    waterfallSteps = new WaterfallStep[]
                    {
                    InterfaceSubjectChoiceAsync,
                    };
                    break;
                case "Fully specified subjects":
                    waterfallSteps = new WaterfallStep[]
                    {
                    FullySpecifiedSubjectChoiceAsync,
                    };
                    break;
                case "Multi subjects":
                    waterfallSteps = new WaterfallStep[]
                    {
                    MultiSubjectChoiceAsync,
                    };
                    break;
                default:
                    waterfallSteps = new WaterfallStep[]
                    {
                    SubjectChoiceAsync, 
                    };
                    break;
            }

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));


            InitialDialogId = nameof(WaterfallDialog);
        }
        private static async Task<DialogTurnResult> SubjectChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            {
                ModelManagement management = ModelManagement.getInstance();
                List<Choice> choiceList = new List<Choice>();

                foreach (string sub in management.subjectCollection.Keys)
                {
                    Choice myChoice = new Choice
                    {
                        Value = sub,
                    };
                    try
                    {
                        choiceList.Add(myChoice);
                    }
                    catch (ArgumentException)
                    {
                        await stepContext.Context.SendActivityAsync($"Cannot add duplicate subjects with name {sub} to this dictionary");
                    }

                }

                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                    Prompt = stepContext.Context.Activity.CreateReply("These subjects currently exist in your model. Please select one"),
                    Choices = choiceList,
                        Style = ListStyle.HeroCard
                    }, cancellationToken);
            };
        }

        private static async Task<DialogTurnResult> FullySpecifiedSubjectChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            {
                ModelManagement management = ModelManagement.getInstance();
                List<Choice> choiceList = new List<Choice>();

                IDictionary<string, ISubject> fullySubjects = new Dictionary<string, ISubject>();

                foreach (IFullySpecifiedSubject sub in management.subjectCollection.Values.OfType<IFullySpecifiedSubject>())
                {
                    string subName = sub.getModelComponentLabels()[0];

                    subName = subName.Remove(subName.Length - 3);

                    fullySubjects.Add(subName, sub);
                }

                foreach (string sub in fullySubjects.Keys)
                {
                    Choice myChoice = new Choice
                    {
                        Value = sub,
                    };
                    try
                    {
                        choiceList.Add(myChoice);
                    }
                    catch (ArgumentException)
                    {
                        await stepContext.Context.SendActivityAsync($"Cannot add duplicate subjects with name {sub} to this dictionary");
                    }

                }

                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = stepContext.Context.Activity.CreateReply("These subjects currently exist in your model. Please select one"),
                        Choices = choiceList,
                        Style = ListStyle.HeroCard
                    }, cancellationToken);
            };
        }
        private static async Task<DialogTurnResult> InterfaceSubjectChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            {
                ModelManagement management = ModelManagement.getInstance();
                List<Choice> choiceList = new List<Choice>();

                IDictionary<string, ISubject> interfaceSubjects = new Dictionary<string, ISubject>();

                foreach (IInterfaceSubject sub in management.subjectCollection.Values.OfType<IInterfaceSubject>())
                {
                    string subName = sub.getModelComponentLabels()[0];

                    subName = subName.Remove(subName.Length - 3);

                    interfaceSubjects.Add(subName, sub);
                }

                foreach (string sub in interfaceSubjects.Keys)
                {
                    Choice myChoice = new Choice
                    {
                        Value = sub,
                    };
                    try
                    {
                        choiceList.Add(myChoice);
                    }
                    catch (ArgumentException)
                    {
                        await stepContext.Context.SendActivityAsync($"Cannot add duplicate subjects with name {sub} to this dictionary");
                    }

                }

                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = stepContext.Context.Activity.CreateReply("These subjects currently exist in your model. Please select one"),
                        Choices = choiceList, Style = ListStyle.HeroCard
                    }, cancellationToken);
            };
        }
        private static async Task<DialogTurnResult> MultiSubjectChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            {
                ModelManagement management = ModelManagement.getInstance();
                List<Choice> choiceList = new List<Choice>();

                IDictionary<string, ISubject> multiSubjects = new Dictionary<string, ISubject>();

                foreach (IMultiSubject sub in management.subjectCollection.Values.OfType<IMultiSubject>())
                {
                    string subName = sub.getModelComponentLabels()[0];

                    subName = subName.Remove(subName.Length - 3);

                    multiSubjects.Add(subName, sub);
                }

                foreach (string sub in multiSubjects.Keys)
                {
                    Choice myChoice = new Choice
                    {
                        Value = sub,
                        
                    };
                    try
                    {
                        choiceList.Add(myChoice);
                    }
                    //TODO: solve this problem with the .TryAdd method instead of just throwing an exception
                    catch (ArgumentException)
                    {
                        await stepContext.Context.SendActivityAsync($"Cannot add duplicate subjects with name {sub} to this dictionary");
                    }

                }

                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                    new PromptOptions
                    {
                        Prompt = stepContext.Context.Activity.CreateReply("These subjects currently exist in your model. Please select one"),
                        Choices = choiceList, Style=ListStyle.HeroCard
                    }, cancellationToken);
            };
        }
    }
}

