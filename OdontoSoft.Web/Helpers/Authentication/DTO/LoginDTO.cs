using OdontoSoft.Web.ViewModels.Account;

namespace OdontoSoft.Web.Helpers.Authentication.DTO
{
    public class LoginDTO
    {
        public String Username { get; set; }
        public String Password { get; set; }
        public String CompanyCode { get; set; }
        public String ApplicationCode { get; set; }

        public static LoginDTO MapFromViewModel(LoginViewModel viewModel)
        {
            return new LoginDTO
            {
                Username = viewModel.Email,
                Password = viewModel.Password
                //CompanyCode = "ANDINA",
                //ApplicationCode = "EXTRACORE"
            };
        }
    }
}
