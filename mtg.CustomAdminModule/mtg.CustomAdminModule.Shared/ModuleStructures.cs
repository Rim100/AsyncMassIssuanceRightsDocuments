using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace mtg.CustomAdminModule.Structures.Module
{
    /// <summary>
    /// Информация о выполнении и параметры обработки.
    /// </summary>
    partial class AsyncIssuanceRightsInfo
    {
        public DateTime StartDateTime {get; set;}
        
        public DateTime EndDateTime {get; set;}
        
        public System.Guid Guid {get; set;}
        /// <summary>
        /// id инициатора.
        /// </summary>
        public int InitiatorID {get; set;}
        
        /// <summary>
        /// Корневая папка.
        /// </summary>
        public IFolder MainFolder {get; set;}
        
        /// <summary>
        /// id обработаных папок.
        /// </summary>
        public List<int> ProcessedFoldersId {get; set;}
        
        /// <summary>
        /// id обработаных документов.
        /// </summary>
        public List<int> ProcessedDocsId {get; set;}
        
        public string RightTypeName {get; set;}
        
        /// <summary>
        /// Тип прав.
        /// </summary>
        public System.Guid RightType {get; set;}
        
        /// <summary>
        /// Количество ошибок при обработке.
        /// </summary>
        public int ErrorsCount {get; set;}
        
        /// <summary>
        /// Выдать права на папки.
        /// </summary>
        public bool GrantRightsFolders {get; set;}
        
        /// <summary>
        /// Выдать права на документы.
        /// </summary>
        public bool GrantRightsDocuments {get; set;}
        
        /// <summary>
        /// Обработать подпапки.
        /// </summary>
        public bool ProcessingSubfolders {get; set;}
        
        /// <summary>
        /// Оставить более привилегированные права.
        /// </summary>
        public bool LeaveMorePrivilegedRights {get; set;}
        
        /// <summary>
        /// Субъекты прав.
        /// </summary>
        public IEnumerable<IRecipient> SubjectsOfRights {get; set;}
    }
    
}
