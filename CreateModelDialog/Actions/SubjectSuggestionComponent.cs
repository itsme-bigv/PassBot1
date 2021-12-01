using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
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
                SummaryAsync,
            };

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
                await stepContext.Context.SendActivityAsync("I created a list of available subjects");

                return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = stepContext.Context.Activity.CreateReply("These subjects currently exist in your model. Please select one"),
                    Choices = choiceList
                }, cancellationToken);
            };

        }

        private static async Task<DialogTurnResult> SummaryAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            ModelManagement management = ModelManagement.getInstance();

            stepContext.Values["subject"] = ((FoundChoice)stepContext.Result).Value;

            if (management.subjectCollection[(string)stepContext.Values["subject"]] != null)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text($"{(string)stepContext.Values["subject"]} is the selected subject") }, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync("cannot find a subject with this name");
            }
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }




    }

}

