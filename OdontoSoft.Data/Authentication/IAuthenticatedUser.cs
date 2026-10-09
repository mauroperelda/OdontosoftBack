using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdontoSoft.Data.Authentication
{
    public interface IAuthenticatedUser
    {
        int GetId();
        bool GetAdministrador();
        bool GetSuperUsuario();
        string GetUserName();
        string GetEmail();
        bool HasPermission(string permissionCode);

    }
}
