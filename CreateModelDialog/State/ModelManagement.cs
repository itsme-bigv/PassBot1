using System.Collections.Generic;
using alps.net_api.StandardPASS;
using alps.net_api.StandardPASS.InteractionDescribingComponents;

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

    }
}
