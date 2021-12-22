using System.Collections.Generic;
using System.Linq;
using alps.net_api.StandardPASS;
using alps.net_api.StandardPASS.InteractionDescribingComponents;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace CreateModelDialog
{
    class ModelManagement
    {
        private static ModelManagement management;
        public PASSProcessModel Model { get; set; }
        public Dictionary<string, ISubject> subjectCollection { get; set; }


        private ModelManagement()
        {

        }

        public static ModelManagement getInstance()
        {
            if (management is null)
            {
                management = new ModelManagement();
            }
            return management;
        }

        public Dictionary<string,ISubject> getSubjectCollection()
        {
            if (subjectCollection is null)
            {
                subjectCollection = new Dictionary<string, ISubject>();
            }

            return subjectCollection;
        }

        public List<string> GetListOfAvailableSubjectNames()
        {
            List<string> names = new List<string>();
            IList<IPASSProcessModelElement> elements = new List<IPASSProcessModelElement>(management.Model.getBaseLayer().getElements().Values);
            foreach (IPASSProcessModelElement element in elements)
            {
                string splitName = element.getModelComponentID();
                names.Add(splitName);
            }
            return names;
        }

        public Dictionary<string, ISubject> GetDictionaryOfAvailableSubjects()
        {
            Dictionary<string, ISubject> subjects = new Dictionary<string, ISubject>();
            if (Model.getAllElements().Values.OfType<ISubject>() != null)
            {
                foreach (ISubject sub in management.Model.getAllElements().Values.OfType<ISubject>())
                {
                    subjects.Add(sub.getModelComponentID(), sub);
                }
            }
            else
            {
                PromptOptions pO = new PromptOptions
                {
                    Prompt = MessageFactory.Text("There are no subjects in your model at the moment. Please add some first and then come back")
                };
            }

            return subjects;
        }

        public List<ISubjectBehavior> GetSubjectBehaviors()
        {
            List<ISubjectBehavior> subjectBehaviors = new List<ISubjectBehavior>();
            foreach (ISubjectBehavior bhv in Model.getAllElements().Values.OfType<ISubjectBehavior>())
            {
                subjectBehaviors.Add(bhv);
            }
            return subjectBehaviors;
        }

        public bool HasStates(ISubject sub)
        { 
            bool hasState = false;
            //Dictionary<ISubject, string> invertedSubjectCollection = new Dictionary<ISubject, string>();
            //foreach (string k in subjectCollection.Keys)
            //{
            //    invertedSubjectCollection.Add(subjectCollection[k], k);
            //}
            //string subjectName = invertedSubjectCollection[sub];

            //foreach (ISubjectBehavior bhv in GetSubjectBehaviors())
            //{
            //    if ((GetSubjectBehaviors().Any(invertedSubjectCollection[sub] => invertedSubjectCollection[sub].Contains(substring, System.StringComparison.InvariantCultureIgnoreCase)))
            //    {

            //    }
            //    if (bhv.getBehaviorDescribingComponents().Any())
            //    {
            //        hasState = true;
            //    }
            //}

            return hasState;
        }

    }
}
