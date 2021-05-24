using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Content;

namespace mtg.CustomAdminModule.Server
{
    public class ModuleAsyncHandlers
    {

        public virtual void AsyncMassIssuanceRightsDocuments(mtg.CustomAdminModule.Server.AsyncHandlerInvokeArgs.AsyncMassIssuanceRightsDocumentsInvokeArgs args)
        {
            // Сбор информация о выполнении.
            var issuanceRightsInfo = Structures.Module.AsyncIssuanceRightsInfo.Create();
            
            var settings = MassIssuanceRightDocuments.Get(args.MassIssuRightBookId);
            
            var folder = settings.Folder;
            
            var recipients = settings.RecipientsCollection.Select(r => r.RecipientChildProp);
            
            var typeRight = settings.RightType == CustomAdminModule.MassIssuanceRightDocument.RightType.Change ? DefaultAccessRightsTypes.Change :
                (settings.RightType == CustomAdminModule.MassIssuanceRightDocument.RightType.FullAccess ? DefaultAccessRightsTypes.FullAccess : DefaultAccessRightsTypes.Read);
            
            Logger.DebugFormat("Start MassIssuanceRightDocuments Id - {0}", settings.Id);
            
            foreach(var recipient in recipients)
            {
                // Выдать права на основную папку
                SafeExecute(() => AddRightToEntity(folder, settings.LeaveMorePrivilegedRights.Value, recipient, typeRight), issuanceRightsInfo);
                issuanceRightsInfo.ProcessedEntitiesId = new List<int>();
                issuanceRightsInfo.ProcessedEntitiesId.Add(folder.Id);
                issuanceRightsInfo.FoldersCount++;
            }
            
            
            SafeExecute(() => AddRightToFolderEntities(folder.Items, typeRight, recipients, settings.GrantRightsFolders.Value, settings.GrantRightsDocuments.Value, settings.ProcessingSubfolders.Value, settings.LeaveMorePrivilegedRights.Value, issuanceRightsInfo), issuanceRightsInfo);
            
            Logger.DebugFormat("End MassIssuanceRightDocuments Id - {0}", settings.Id);
            
            settings.Status = CustomAdminModule.MassIssuanceRightDocument.Status.Processed;
            settings.Result = mtg.CustomAdminModule.Resources.MessageResultFormat(issuanceRightsInfo.DocsCount, issuanceRightsInfo.FoldersCount, issuanceRightsInfo.ErrorsCount);
            
            settings.Save();
        }
        
        /// <summary>
        /// Безопасное выполнение.
        /// </summary>
        /// <param name="action">Action</param>
        private static void SafeExecute(Action action, Structures.Module.AsyncIssuanceRightsInfo info)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("AsyncMassRightsError {0}", ex);
                info.ErrorsCount++;
            }
        }
        
        
        /// <summary>
        /// Выдать права на коллекцию сущностей, только на папки и документы.
        /// </summary>
        /// <param name="entities">Содерживое папки</param>
        /// <param name="rightType">Тип Прав</param>
        /// <param name="grantRightsFolders">true - выдать права на вложенные папки, false - нет</param>
        /// <param name="grantRightsDocuments">true - выдать права на вложенные документы, false - нет</param>
        /// <param name="processingSubFolders">true - обработать вложенные папки, false - нет</param>
        /// <param name="leaveMorePrivilegedRights">true - оставить более привелигированные права</param>
        /// <param name="info">AsyncIssuanceRightsInfo</param>
        private static void AddRightToFolderEntities(ICollection<Sungero.Domain.Shared.IEntity> entities, System.Guid rightType, IEnumerable<IRecipient> recipients, bool grantRightsFolders, bool grantRightsDocuments, bool processingSubFolders , bool leaveMorePrivilegedRights, Structures.Module.AsyncIssuanceRightsInfo info)
        {
            foreach (var entity in entities)
            {
                // Защита от рекурсии + не обрабатываем entity повторно
                if (info.ProcessedEntitiesId.Contains(entity.Id))
                {
                    continue;
                }
                
                var folder = Sungero.CoreEntities.Folders.As(entity);
                var document = Sungero.Content.ElectronicDocuments.As(entity);
                
                if (folder == null && document == null)
                    continue;
                
                if ((folder != null && !grantRightsFolders) || (document != null && !grantRightsDocuments))
                    continue;
                
                foreach (var recipient in recipients)
                {
                    SafeExecute(()=> AddRightToEntity(entity, leaveMorePrivilegedRights, recipient, rightType), info);
                }
                
                if (folder != null)
                {
                    info.FoldersCount++;
                }
                else
                {
                    info.DocsCount++;
                }
                
                info.ProcessedEntitiesId.Add(entity.Id);
                
                if (folder != null && processingSubFolders)
                {
                    SafeExecute(()=> AddRightToFolderEntities(folder.Items, rightType, recipients, grantRightsFolders, grantRightsDocuments, processingSubFolders, leaveMorePrivilegedRights, info), info);
                }
            }
        }
        
        /// <summary>
        /// Выдать права субъекту прав.
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <param name="removeOldRights">true - удалить, false - оставить</param>
        /// <param name="recipient">Субъект прав</param>
        /// <param name="rightType">Тип прав</param>
        private static void AddRightToEntity(Sungero.Domain.Shared.IEntity entity, bool leaveMorePrivilegedRights, IRecipient recipient, System.Guid rightType)
        {
            if(!leaveMorePrivilegedRights)
            {
                entity.AccessRights.RevokeAll(recipient);
                entity.AccessRights.Save();
            }
            
            entity.AccessRights.Grant(recipient, rightType);
            entity.AccessRights.Save();
            
        }
        
        /// <summary>
        /// Направить уведомление Администраторам об резальтаты работы AsyncMassIssuanceRightsDocuments.
        /// </summary>
        /// <param name="processedInfo">AsyncIssuanceRightsInfo</param>
        private static void SendNoteToAdministrators(Structures.Module.AsyncIssuanceRightsInfo processedInfo)
        {
            
        }
    }
}