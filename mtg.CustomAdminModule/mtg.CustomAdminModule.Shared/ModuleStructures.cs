using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace mtg.CustomAdminModule.Structures.Module
{
    /// <summary>
    /// Информация о выполнении AsyncMassIssuanceRightsDocuments
    /// </summary>
    partial class AsyncIssuanceRightsInfo
    {
        public IFolder MainFolder {get; set;}
        
        public int FoldersCount {get; set;}
        
        public int DocsCount {get; set;}
        
        public List<int> ProcessedFoldersId {get; set;}
        
        public List<int> ProcessedDocsId {get; set;} 
        
        public Sungero.CoreEntities.IAccessRights RightType {get; set;}
        
        public int ErrorsCount {get; set;}
    }     
    
}
