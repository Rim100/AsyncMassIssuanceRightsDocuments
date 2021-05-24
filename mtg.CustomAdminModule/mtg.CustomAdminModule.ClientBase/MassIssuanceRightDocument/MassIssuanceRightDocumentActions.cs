using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using mtg.CustomAdminModule.MassIssuanceRightDocument;

namespace mtg.CustomAdminModule.Client
{
    partial class MassIssuanceRightDocumentActions
    {
        public virtual void StartProcessing(Sungero.Domain.Client.ExecuteActionArgs e)
        {
            _obj.Save();
            
            _obj.Status = CustomAdminModule.MassIssuanceRightDocument.Status.Processing;
            _obj.Save();
            
            var asyncMethod = mtg.CustomAdminModule.AsyncHandlers.AsyncMassIssuanceRightsDocuments.Create();
            asyncMethod.MassIssuRightBookId = _obj.Id;
            asyncMethod.ExecuteAsync();
            
            Dialogs.NotifyMessage(mtg.CustomAdminModule.Resources.InformMessageAfterStart);
        }

        public virtual bool CanStartProcessing(Sungero.Domain.Client.CanExecuteActionArgs e)
        {
            return _obj.Status != CustomAdminModule.MassIssuanceRightDocument.Status.Processing;
        }

    }

}