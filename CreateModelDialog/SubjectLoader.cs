using alps.net_api;
using System;
using System.Collections.Generic;
using System.Text;

namespace CreateModelDialog
{
    class SubjectLoader
    {
        public List<string> GetListOfAvailableSubjects()
        {
            List<string> names = new List<string>();
            ModelManagement management = ModelManagement.getInstance();
            Dictionary<string, IPASSProcessModelElement> elements = management.Model.getModelLayer(0).getElements();
            foreach (IPASSProcessModelElement element in elements.Values)
            {
                string[] splitName = element.getModelComponentID().Split("#");
                if (splitName.Length > 1)
                {
                    names.Add(splitName[0]);
                }
            }
            return names;
        }
    }
}
