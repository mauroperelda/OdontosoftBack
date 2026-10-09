using OdontoSoft.Web.Helpers.Authentication.DTO;

namespace OdontoSoft.Web.IServices
{
    public interface IAccountService
    {
        Task<UserDTO> LoginAsync(LoginDTO login);
        UserDTO Login(LoginDTO login);
    }
}
