using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace mtg.CustomAdminModule.Client
{
    public class ModuleFunctions
    {
        /// <summary>
        /// Массовая выдача прав на документы.
        /// </summary>
        public virtual void MassIssuanceRightsDocuments()
        {
            CustomAdminModule.PublicFunctions.Module.CreatMassIssuance().Show();
        }
    }
}