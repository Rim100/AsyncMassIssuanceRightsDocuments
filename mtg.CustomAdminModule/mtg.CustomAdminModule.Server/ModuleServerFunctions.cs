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
        /// Направить уведомление Администраторам о результах работы AsyncMassIssuanceRightsDocuments.
        /// </summary>
        /// <param name="info">Информация о выполнении и параметры обработки.</param>
        public static void SendNoteToAdministrators(Structures.Module.AsyncIssuanceRightsInfo info)
        {
            try
            {
                var user = Users.Get(info.InitiatorID);
                
                var subject = mtg.CustomAdminModule.Resources.IssuanceRightsResultsSubject;
                var result = string.Empty;
                
                var notice = Sungero.Workflow.SimpleTasks.CreateWithNotices(subject, user);
                
                result += mtg.CustomAdminModule.Resources.TimeStartEndFormat(info.StartDateTime.ToString("G"), info.EndDateTime.ToString("G"));
                result += Environment.NewLine + mtg.CustomAdminModule.Resources.Settings + Environment.NewLine;
                result += mtg.CustomAdminModule.Resources.RootFolderFormat(Hyperlinks.Get(info.MainFolder)) + Environment.NewLine;
                result +=  mtg.CustomAdminModule.Resources.SubjectsOfRightsFormat(string.Join(";", info.SubjectsOfRights.Select(x => Hyperlinks.Get(x)))) + Environment.NewLine;
                result += string.Format("{0}: {1}", mtg.CustomAdminModule.Resources.TypeRights, info.RightTypeName) + Environment.NewLine;
                result += string.Format("{0}: {1}",  mtg.CustomAdminModule.Resources.GrantRightsFolders, info.GrantRightsFolders) + Environment.NewLine;
                result += string.Format("{0}: {1}",  mtg.CustomAdminModule.Resources.GrantRightsDocuments, info.GrantRightsDocuments) + Environment.NewLine;
                result += string.Format("{0}: {1}",  mtg.CustomAdminModule.Resources.HandleSubfolders, info.ProcessingSubfolders) + Environment.NewLine;
                result += string.Format("{0}: {1}",  mtg.CustomAdminModule.Resources.LeaveMorePrivilegedRights, info.LeaveMorePrivilegedRights) + Environment.NewLine;
                result += Environment.NewLine + mtg.CustomAdminModule.Resources.Results + Environment.NewLine;
                result += mtg.CustomAdminModule.Resources.FoldersProcessedFormat(info.ProcessedFoldersId.Count) + Environment.NewLine;
                result += mtg.CustomAdminModule.Resources.DocumentsProcessedFormat(info.ProcessedDocsId.Count) + Environment.NewLine;
                result += mtg.CustomAdminModule.Resources.ErrorsCountFormat(info.ErrorsCount);
                
                notice.ActiveText = result;
                notice.Start();
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error AsyncMassIssuanceRightsDocuments error sending notification, GUID - {0}, Message: {1}", info.Guid, ex.Message);
            }
        }
    }
}