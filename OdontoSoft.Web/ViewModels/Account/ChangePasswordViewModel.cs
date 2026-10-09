namespace OdontoSoft.Web.ViewModels.Account
{
    public class ChangePasswordViewModel
    {
        public int? Id { get; set; }
        public string? Hash { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirm { get; set; }

    }

}
