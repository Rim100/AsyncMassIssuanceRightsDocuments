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
            issuanceRightsInfo.ProcessedFoldersId = new List<int>();
            issuanceRightsInfo.ProcessedDocsId = new List<int>();
            
            var folder = Sungero.CoreEntities.Folders.Get(args.Folder);
            
            var recipientsId = args.SubjectsRights.Split(';').Select(x => int.Parse(x)).ToList();
            
            var recipients = Sungero.CoreEntities.Recipients.GetAll(x => recipientsId.Contains(x.Id));
            
            var typeRight = Guid.Parse(args.RightTypeGuid);
            
            Logger.DebugFormat("Start MassIssuanceRightDocuments for folder, Id - {0}", args.Folder);
            
            foreach(var recipient in recipients)
            {
                // Выдать права на основную папку.
                AddRightToEntity(folder, args.LeaveMorePrivilegedRights, recipient, typeRight);
                issuanceRightsInfo.ProcessedFoldersId.Add(folder.Id);
                issuanceRightsInfo.FoldersCount++;
            }
            
            
            AddRightToFolderEntities(folder.Items, typeRight, recipients, args.GrantRightsFolders, args.GrantRightsDocuments, args.ProcessingSubfolders, args.LeaveMorePrivilegedRights, issuanceRightsInfo);
            
            Logger.DebugFormat("End MassIssuanceRightDocuments for folder, Id - {0}", args.Folder);
            

        }
        
        
        
        
        /// <summary>
        /// Выдать права на коллекцию сущностей, только на папки и документы.
        /// </summary>
        /// <param name="entities">Содерживое папки.</param>
        /// <param name="rightType">Тип Прав.</param>
        /// <param name="grantRightsFolders">true - выдать права на вложенные папки, false - нет.</param>
        /// <param name="grantRightsDocuments">true - выдать права на вложенные документы, false - нет.</param>
        /// <param name="processingSubFolders">true - обработать вложенные папки, false - нет.</param>
        /// <param name="leaveMorePrivilegedRights">true - оставить более привелигированные права.</param>
        /// <param name="info">AsyncIssuanceRightsInfo</param>
        private static void AddRightToFolderEntities(ICollection<Sungero.Domain.Shared.IEntity> entities, System.Guid rightType, IEnumerable<IRecipient> recipients, bool grantRightsFolders, bool grantRightsDocuments, bool processingSubFolders , bool leaveMorePrivilegedRights, Structures.Module.AsyncIssuanceRightsInfo info)
        {
            foreach (var entity in entities)
            {
                var folder = Sungero.CoreEntities.Folders.As(entity);
                var document = Sungero.Content.ElectronicDocuments.As(entity);
                
                if (folder == null && document == null)
                    continue;
                
                if ((folder != null && !grantRightsFolders) || (document != null && !grantRightsDocuments))
                    continue;
                
                // Защита от рекурсии + не обрабатываем entity повторно.
                if (folder != null && info.ProcessedFoldersId.Contains(entity.Id))
                    continue;
                
                if (document != null && info.ProcessedDocsId.Contains(entity.Id))
                    continue;
                
                foreach (var recipient in recipients)
                {
                    AddRightToEntity(entity, leaveMorePrivilegedRights, recipient, rightType);
                }
                
                if (folder != null)
                {
                    info.FoldersCount++;
                    info.ProcessedFoldersId.Add(entity.Id);
                }
                else
                {
                    info.DocsCount++;
                    info.ProcessedDocsId.Add(entity.Id);
                }
                
                if (folder != null && processingSubFolders)
                {
                    AddRightToFolderEntities(folder.Items, rightType, recipients, grantRightsFolders, grantRightsDocuments, processingSubFolders, leaveMorePrivilegedRights, info);
                }
            }
        }
        
        /// <summary>
        /// Выдать права субъекту прав.
        /// </summary>
        /// <param name="entity">Сущность.</param>
        /// <param name="removeOldRights">true - удалить, false - оставить.</param>
        /// <param name="recipient">Субъект прав.</param>
        /// <param name="rightType">Тип прав.</param>
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
        
        
    }
}