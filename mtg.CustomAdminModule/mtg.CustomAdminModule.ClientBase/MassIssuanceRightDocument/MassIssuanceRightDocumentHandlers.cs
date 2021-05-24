using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using mtg.CustomAdminModule.MassIssuanceRightDocument;

namespace mtg.CustomAdminModule
{
    partial class MassIssuanceRightDocumentClientHandlers
    {

        public override void Refresh(Sungero.Presentation.FormRefreshEventArgs e)
        {
            if (_obj.Status == CustomAdminModule.MassIssuanceRightDocument.Status.Processing)
            {
                _obj.State.IsEnabled = false;
                foreach (var p in _obj.State.Properties)
                {
                    p.IsEnabled = false;
                }
            }
        }
    }

}