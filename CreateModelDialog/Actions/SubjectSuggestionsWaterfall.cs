using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using alps.net_api.StandardPASS.InteractionDescribingComponents;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace CreateModelDialog.Actions
{
    public class SubjectSuggestionsWaterfall : WaterfallDialog
    {
        //        [JsonProperty("$kind")]
        //        public const string Kind = "SubjectSuggestionsWaterfall";

        //        private static IEnumerable<WaterfallStep> steps;

        //        /*public SubjectSuggestionsWaterfall([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        //: base()*/

        //        [JsonConstructor]
        //        public SubjectSuggestionsWaterfall(string dialogId, IEnumerable<WaterfallStep> steps = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        //    : base(dialogId,steps)
        //        {
        //            // enable instances of this command as debug break point
        //            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        //            //}
        //            //public SubjectSuggestionsWaterfall(string dialogID, IEnumerable<WaterfallStep> steps = null) : base(dialogID, steps)
        //            //{

        //            AddStep(async (stepContext, cancellationToken) =>
        //            {
        //                SubjectLoader subjectLoader = new SubjectLoader();
        //                Dictionary<string, ISubject> subjects = subjectLoader.GetDictionaryOfAvailableSubjects();

        //                new PromptOptions();
        //                List<Choice> choiceList = new List<Choice>();

        //                foreach (string sub in subjects.Keys)
        //                {
        //                    Choice myChoice = new Choice
        //                    {
        //                        Value = sub,
        //                        Action = ActionTypes.ImBack
        //                    };
        //                    try
        //                    {
        //                        choiceList.Add(myChoice);
        //                    }
        //                    catch (ArgumentException)
        //                    {
        //                        await stepContext.Context.SendActivityAsync($"Cannot add duplicate subjects with name {sub} to this dictionary");
        //                        //PromptOptions pO = new PromptOptions
        //                        //{
        //                        //    Prompt = MessageFactory.Text($"Cannot add duplicate subjects with name {sub} to this dictionary")
        //                        //};

        //                    }

        //                }
        //                await stepContext.Context.SendActivityAsync("I created a list of available subjects");

        //                return await stepContext.PromptAsync("choicePrompt",
        //                    new PromptOptions
        //                    {
        //                        Prompt = stepContext.Context.Activity.CreateReply("These subjects currently exist in your model. Please select one"),
        //                        Choices = choiceList
        //                    });
        //            });

        //            AddStep(async (stepContext, cancellationToken) =>
        //            {
        //                var response = (stepContext.Result as FoundChoice)?.Value;
        //                if (response != null)
        //                {
        //                    SubjectLoader subjectLoader = new SubjectLoader();
        //                    Dictionary<string, ISubject> subjects = subjectLoader.GetDictionaryOfAvailableSubjects();

        //                    ISubject selectedSubject = subjects[response];

        //                    await stepContext.Context.SendActivityAsync($"you selected {selectedSubject.getModelComponentID()}");
        //                        //TODO: Find a way to save selection to state
        //            // await ConversationState.SaveChangesAsync(context, false, cancellationToken);
        //            //}, cancellationToken);

        //                }
        //                else
        //                {
        //                    await stepContext.Context.SendActivityAsync("Sorry, that didn't work");
        //                }
        //                return await stepContext.NextAsync();
        //            });

        //        }
        public SubjectSuggestionsWaterfall(string dialogId, IEnumerable<WaterfallStep> steps=null) : base (dialogId, steps)
        {
            WaterfallDialog SubjectSuggestionsWaterfall = new WaterfallDialog("waterfall");
            SubjectSuggestionsWaterfall.AddStep(async (stepContext, cancellationToken) =>
                {
                    SubjectLoader subjectLoader = new SubjectLoader();
                    Dictionary<string, ISubject> subjects = subjectLoader.GetDictionaryOfAvailableSubjects();

                    new PromptOptions();
                    List<Choice> choiceList = new List<Choice>();

                    foreach (string sub in subjects.Keys)
                    {
                        Choice myChoice = new Choice
                        {
                            Value = sub,
                            Action = ActionTypes.ImBack
                        };
                        try
                        {
                            choiceList.Add(myChoice);
                        }
                        catch (ArgumentException)
                        {
                            await stepContext.Context.SendActivityAsync($"Cannot add duplicate subjects with name {sub} to this dictionary");
                            //PromptOptions pO = new PromptOptions
                            //{
                            //    Prompt = MessageFactory.Text($"Cannot add duplicate subjects with name {sub} to this dictionary")
                            //};

                        }

                    }
                    await stepContext.Context.SendActivityAsync("I created a list of available subjects");

                    return await stepContext.PromptAsync("choicePrompt",
                    new PromptOptions
                    {
                        Prompt = stepContext.Context.Activity.CreateReply("These subjects currently exist in your model. Please select one"),
                        Choices = choiceList
                    });
                });

            SubjectSuggestionsWaterfall.AddStep(async (stepContext, cancellationToken) =>
            {
                var response = (stepContext.Result as FoundChoice)?.Value;
                if (response != null)
                {
                    SubjectLoader subjectLoader = new SubjectLoader();
                    Dictionary<string, ISubject> subjects = subjectLoader.GetDictionaryOfAvailableSubjects();

                    ISubject selectedSubject = subjects[response];

                    await stepContext.Context.SendActivityAsync($"you selected {selectedSubject.getModelComponentID()}");
                    //TODO: Find a way to save selection to state
                    // await ConversationState.SaveChangesAsync(context, false, cancellationToken);
                    //}, cancellationToken);

                }
                else
                {
                    await stepContext.Context.SendActivityAsync("Sorry, that didn't work");
                }
                return await stepContext.NextAsync();
            });
        }
        private static IEnumerable<WaterfallStep> step;
        private static new string Id => "dialogId";
        public static SubjectSuggestionsWaterfall Instance { get; } = new SubjectSuggestionsWaterfall(Id, step);
    }
}



