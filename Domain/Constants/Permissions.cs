using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Entity.Helper;

namespace Domain.Constants
{
    public class Permissions
    {
        public static List<string> GeneratePermissionFromModule(string module)
        {
            return new List<string>
            {
                $"Permissions.{module}.View",
                $"Permissions.{module}.Create",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete"
            };
        } 
        
        public static List<string> PermissionList()
        {
            var allPermissions = new List<string>();
            foreach (var module in Enum.GetValues(typeof(PermissionsModulesName)))
                allPermissions.AddRange(GeneratePermissionFromModule(module.ToString()));
            return allPermissions;
        }
    }

}
