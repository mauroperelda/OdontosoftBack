using OdontoSoft.Data.Authentication;
//using ExtraCore.Entities.Constants;
//using Log2WebCoreLogger;
//using MedveCore.Common.Helpers;
using System.Security.Claims;
using System;
using OdontoSoft.Web.Helpers.Constants;

namespace ExtraCore.Web.Helpers.Authentication
{
    public class AuthenticatedUser : IAuthenticatedUser
    {

        private IHttpContextAccessor _httpContextAccessor;

        public AuthenticatedUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpContext GetContext()
        {
            return _httpContextAccessor.HttpContext;
        }

        private string GetClaimValue(string key)
        {
            if (GetContext() == null) return "";
            return GetContext().User.Claims.Any(r => r.Type == key) ? GetContext().User.Claims.First(r => r.Type == key).Value : null;
        }

        private bool GetClaimRole(string permissionCode)
        {
            if (GetContext() == null) return false;
            return GetContext().User.Claims.Any(r => r.Type == ClaimTypes.Role && r.Value.ToLower() == permissionCode.ToLower());
        }

        private bool GetClaimValueAsBoolean(string key)
        {
            var s = GetClaimValue(key);
            if (s == null) return false;
            s = s.ToUpper();
            return (s == "1" || s == "TRUE");
        }

        public int GetId()
        {
            return Convert.ToInt32(GetClaimValue("Id"));
        }

        //public int GetClienteId()
        //{
        //    return Converter.ToInt32(GetClaimValue("ClienteId"));
        //}

        //public bool IsCliente()
        //{
        //    return Converter.ToInt32(GetClaimValue("PerfilId")) == Constants.PERFIL_ID_CLIENTE;
        //}

        public string GetUserName()
        {
            return GetClaimValue(ClaimTypes.Name);
        }
        public string GetEmail()
        {
            return GetClaimValue(ClaimTypes.Email);
        }

        public bool GetAdministrador()
        {
            return GetClaimValueAsBoolean(PermissionCodes.ADMINISTRATOR_CODE);
        }

        public bool GetSuperUsuario()
        {
            return GetClaimValueAsBoolean(PermissionCodes.SUPERUSER_CODE);
        }

        public string GetUserIdentification()
        {
            return GetClaimValue(ClaimTypes.Email);
        }

        public bool HasPermission(string permissionCode)
        {
            return GetClaimRole(permissionCode);
        }
    }
}
