using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using mtg.CustomAdminModule.MassIssuanceRightDocument;

namespace mtg.CustomAdminModule
{
    partial class MassIssuanceRightDocumentFolderPropertyFilteringServerHandler<T>
    {

        public virtual IQueryable<T> FolderFiltering(IQueryable<T> query, Sungero.Domain.PropertyFilteringEventArgs e)
        {
            return query.Where(x => x.IsSpecial.HasValue && !x.IsSpecial.Value);
        }
    }

    partial class MassIssuanceRightDocumentFolderSearchPropertyFilteringServerHandler<T>
    {

        public virtual IQueryable<T> FolderSearchDialogFiltering(IQueryable<T> query, Sungero.Domain.PropertySearchDialogFilteringEventArgs e)
        {
            return query.Where(x => x.IsSpecial.HasValue && !x.IsSpecial.Value);
        }
    }

    partial class MassIssuanceRightDocumentServerHandlers
    {

        public override void Created(Sungero.Domain.CreatedEventArgs e)
        {
            _obj.GrantRightsDocuments = true;
            _obj.GrantRightsFolders = true;
            _obj.LeaveMorePrivilegedRights = true;
            _obj.ProcessingSubfolders = true;
            _obj.Status = CustomAdminModule.MassIssuanceRightDocument.Status.Creature;
        }
    }

}