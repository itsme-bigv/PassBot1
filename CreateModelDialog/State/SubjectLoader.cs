using System.Collections.Generic;
using System.Linq;
using alps.net_api.StandardPASS;
using alps.net_api.StandardPASS.InteractionDescribingComponents;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace CreateModelDialog
{
    class SubjectLoader
    {
        public List<string> GetListOfAvailableSubjectNames()
        {
            List<string> names = new List<string>();
            ModelManagement management = ModelManagement.getInstance();
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
            ModelManagement management = ModelManagement.getInstance();
            Dictionary<string, ISubject> subjects = new Dictionary<string, ISubject>();
            if (management.Model.getAllElements().Values.OfType<ISubject>()!=null)
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


    }
}
