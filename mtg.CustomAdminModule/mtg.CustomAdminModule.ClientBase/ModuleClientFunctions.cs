using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace mtg.CustomAdminModule.Client
{
    public class ModuleFunctions
    {
        /// <summary>
        /// Массовая выдача прав на документы.
        /// </summary>
        public virtual void MassIssuanceRightsDocuments()
        {
            if (Users.Current.IncludedIn(Roles.Administrators))
                MassIssuanceRights();
            else
                Dialogs.ShowMessage(mtg.CustomAdminModule.Resources.NotifyMessage);
        }
        
        /// <summary>
        /// Массовая выдача прав на папку и содержимое.
        /// </summary>
        [Public]
        public static void MassIssuanceRights()
        {
            var recipients = Functions.Module.Remote.GetAllRecipients();
            var folders = Functions.Module.Remote.GetAllNoSysFolders();
            var members = recipients.ShowSelectMany(mtg.CustomAdminModule.Resources.SelectSubjectsRights);
            
            var dialog = Dialogs.CreateInputDialog(mtg.CustomAdminModule.Resources.TitleGrantingFolder);
            
            var folder = dialog.AddSelect(mtg.CustomAdminModule.Resources.Folder, true, Sungero.CoreEntities.Folders.Null).From(folders);
            var subjectsRights = dialog.AddSelectMany(mtg.CustomAdminModule.Resources.SubjectsRights, true, members.ToArray());
            var typeRights = dialog.AddSelect(mtg.CustomAdminModule.Resources.TypeRights, true, string.Empty).From(Resources.View, Resources.Change, Resources.FullAccess);

            var grantRightsFolders = dialog.AddBoolean(mtg.CustomAdminModule.Resources.GrantRightsFolders, false);
            var grantRightsDocuments = dialog.AddBoolean(mtg.CustomAdminModule.Resources.GrantRightsDocuments, false);
            var processingSubfolders = dialog.AddBoolean(mtg.CustomAdminModule.Resources.HandleSubfolders, false);
            var leaveMorePrivilegedRights = dialog.AddBoolean(mtg.CustomAdminModule.Resources.LeaveMorePrivilegedRights, true);
            
            if (dialog.Show() == DialogButtons.Ok)
            {
                
                var subjectsRightsString = string.Join(";", subjectsRights.Value.Select(x => x.Id));
                var rightType = typeRights.Value == mtg.CustomAdminModule.Resources.Change ? DefaultAccessRightsTypes.Change :
                    (typeRights.Value == mtg.CustomAdminModule.Resources.FullAccess ? DefaultAccessRightsTypes.FullAccess : DefaultAccessRightsTypes.Read);
                
                var asyncMethod = mtg.CustomAdminModule.AsyncHandlers.AsyncMassIssuanceRightsDocuments.Create();
                
                
                asyncMethod.Folder = folder.Value.Id;
                asyncMethod.SubjectsRights = subjectsRightsString;
                asyncMethod.RightTypeGuid = rightType.ToString();
                asyncMethod.RightTypeName = typeRights.Value;
                
                asyncMethod.GrantRightsFolders = grantRightsFolders.Value.GetValueOrDefault();
                asyncMethod.GrantRightsDocuments = grantRightsDocuments.Value.GetValueOrDefault();
                asyncMethod.ProcessingSubfolders = processingSubfolders.Value.GetValueOrDefault();
                asyncMethod.LeaveMorePrivilegedRights = leaveMorePrivilegedRights.Value.GetValueOrDefault();
                asyncMethod.InitiatorID = Users.Current.Id;
                asyncMethod.ExecuteAsync();
                
                Dialogs.NotifyMessage(mtg.CustomAdminModule.Resources.InformMessageAfterStart);
            }
            
        }
    }
}