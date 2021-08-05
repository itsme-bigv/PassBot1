using alps.net_api;
using System;
using System.Collections.Generic;
using System.Text;

namespace CreateModelDialog
{
    class ModelManagement
    {
        private static ModelManagement management;
        public PassProcessModel Model { get; set; }

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

    }
}
