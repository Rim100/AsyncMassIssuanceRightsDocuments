using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace mtg.CustomAdminModule.Server
{
    public class ModuleFunctions
    {
        /// <summary>
        /// Создать запись выдачи прав.
        /// </summary>
        /// <returns></returns>
        [Public]
        public static IMassIssuanceRightDocument CreatMassIssuance()
        {
            return CustomAdminModule.MassIssuanceRightDocuments.Create();
        }
    }
}