using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Helper
    {
        public const string PathImageUser = "/Images/Users/";   // for show  (reading)
        public const string PathSaveImageUser = "Images/Users";  // for save  (writing)

        //msg type
        public const string Success = "success";
        public const string Error = "error";


        public const string MsgType = "MsgType";
        public const string Title = "title";
        public const string Msg = "msg";


        public enum eCurrentState{ Active= 1, Delete = 0 }

        // Auth
        public const string Admin = "Admin";
        // Actions
        public const string Save = "Save";
        public const string Update = "Update";
        public const string Delete = "Delete";


        //  Data DefaultUser => SuperAdmin
        public const string Email = "superadmin@domain.com";
        public const string UserName = "superadmin@domain.com";
        public const string Name = "SuperAdmin";
        public const string Password = "superadminP@ssw0rd";

        //  Data DefaultUser => Basic
        public const string EmailBasic = "basicuser@domain.com";
        public const string UserNameBasic = "basicuser@domain.com";
        public const string NameBasic = "BasicUser";
        public const string PasswordBasic = "basicuserP@ssw0rd";
        //Permission
        public const string Permission = "Permission";

        // Seeds data  DefaultRole
        public enum Roles{
            SuperAdmin,
            Admin,
            Basic
        }


        // Generate Permissions name
        public enum PermissionsModulesName
        {
            Home,
            Accounts,
            Roles,
            Registers,
            Categories
        }

    }
}
