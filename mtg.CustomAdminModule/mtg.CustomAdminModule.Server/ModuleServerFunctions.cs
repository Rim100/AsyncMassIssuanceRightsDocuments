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
        /// Получить все объекты IRecipient.
        /// </summary>
        /// <returns>Все объекты IRecipient в виде запроса.</returns>
        [Remote(IsPure = true)]
        public IQueryable<IRecipient> GetAllRecipients()
        {
            return Sungero.CoreEntities.Recipients.GetAll();
        }
        
        /// <summary>
        /// Получить все не системные папки.
        /// </summary>
        /// <returns>Все объекты IFolder в виде запроса.</returns>
        [Remote(IsPure = true)]
        public IQueryable<IFolder> GetAllNoSysFolders()
        {
            return Folders.GetAll(x => x.IsSpecial == false);
        }
        
        /// <summary>
        /// Направить уведомление Администраторам об резальтаты работы AsyncMassIssuanceRightsDocuments.
        /// </summary>
        /// <param name="processedInfo">AsyncIssuanceRightsInfo.</param>
        private static void SendNoteToAdministrators(Structures.Module.AsyncIssuanceRightsInfo processedInfo, string userID)
        {
           
        }
    }
}