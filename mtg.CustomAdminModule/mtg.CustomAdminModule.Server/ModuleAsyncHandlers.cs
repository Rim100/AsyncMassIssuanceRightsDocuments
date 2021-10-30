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
            var info = Structures.Module.AsyncIssuanceRightsInfo.Create();
            info.Guid = Guid.NewGuid();
            info.InitiatorID = args.InitiatorID;
            info.StartDateTime = Calendar.Now;
            info.ProcessedFoldersId = new List<int>();
            info.ProcessedDocsId = new List<int>();
            
            info.MainFolder = Sungero.CoreEntities.Folders.Get(args.Folder);
            info.RightType = Guid.Parse(args.RightTypeGuid);
            info.RightTypeName = args.RightTypeName;
            
            var recipientsId = args.SubjectsRights.Split(';').Select(x => int.Parse(x)).ToList();
            info.SubjectsOfRights = Sungero.CoreEntities.Recipients.GetAll(x => recipientsId.Contains(x.Id));
            
            info.GrantRightsFolders = args.GrantRightsFolders;
            info.GrantRightsDocuments = args.GrantRightsDocuments;
            info.ProcessingSubfolders = args.ProcessingSubfolders;
            info.LeaveMorePrivilegedRights = args.LeaveMorePrivilegedRights;
            
            Logger.DebugFormat("Start AsyncMassIssuanceRightsDocuments for entity, Id - {0}, GUID - {1}", args.Folder, info.Guid);
            
            
            // Выдать права на основную папку.
            AddRightToEntity(info.MainFolder, info);
            info.ProcessedFoldersId.Add(info.MainFolder.Id);
            
            AddRightToFolderEntities(info.MainFolder.Items, info);
            
            info.EndDateTime = Calendar.Now;
            
            Functions.Module.SendNoteToAdministrators(info);
            
            Logger.DebugFormat("End MassIssuanceRightDocuments for folder, Id - {0}, GUID - {1}", args.Folder, info.Guid);
        }
        
        /// <summary>
        /// Выдать права на коллекцию сущностей, только на папки и документы.
        /// </summary>
        /// <param name="entities">Содерживое папки.</param>
        /// <param name="info">Информация о выполнении и параметры обработки.</param>
        private static void AddRightToFolderEntities(ICollection<Sungero.Domain.Shared.IEntity> entities, Structures.Module.AsyncIssuanceRightsInfo info)
        {
            foreach (var entity in entities)
            {
                try
                {
                    var folder = Sungero.CoreEntities.Folders.As(entity);
                    var document = Sungero.Content.ElectronicDocuments.As(entity);
                    
                    if (folder == null && document == null)
                        continue;
                    
                    if ((folder != null && !info.GrantRightsFolders) || (document != null && !info.GrantRightsDocuments))
                        continue;
                    
                    // Защита от рекурсии + не обрабатываем entity повторно.
                    if (folder != null && info.ProcessedFoldersId.Contains(entity.Id) || document != null && info.ProcessedDocsId.Contains(entity.Id))
                        continue;
                    
                    AddRightToEntity(entity, info);
                    
                    if (folder != null)
                        info.ProcessedFoldersId.Add(entity.Id);
                    else
                        info.ProcessedDocsId.Add(entity.Id);
                    
                    if (folder != null && info.ProcessingSubfolders)
                        AddRightToFolderEntities(folder.Items, info);
                }
                catch (Exception ex)
                {
                    info.ErrorsCount++;
                    Logger.ErrorFormat("Error AsyncMassIssuanceRightsDocuments for entity, Id - {0}, GUID - {1}, message : {2}", entity.Id , info.Guid, ex.Message);
                }
            }
        }
        
        /// <summary>
        /// Выдать права субъекту прав.
        /// </summary>
        /// <param name="entity">Сущность.</param>
        /// <param name="info">Информация о выполнении и параметры обработки.</param>
        private static void AddRightToEntity(Sungero.Domain.Shared.IEntity entity, Structures.Module.AsyncIssuanceRightsInfo info)
        {
            foreach (var recipient in info.SubjectsOfRights)
            {
                try
                {
                    if (info.LeaveMorePrivilegedRights && entity.AccessRights.IsGrantedDirectly(info.RightType, recipient))
                        continue;
                    
                    if (!info.LeaveMorePrivilegedRights)
                        entity.AccessRights.RevokeAll(recipient);
                    
                    entity.AccessRights.Grant(recipient, info.RightType);
                    entity.AccessRights.Save();
                    
                }
                catch (Exception ex)
                {
                    info.ErrorsCount++;
                    Logger.ErrorFormat("Error AsyncMassIssuanceRightsDocuments for entity, Id - {0}, GUID - {1}, message : {2}", entity.Id , info.Guid, ex.Message);
                }
            }
        }
        
    }
}