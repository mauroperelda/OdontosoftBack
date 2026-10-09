namespace OdontoSoft.Web.Helpers.Authentication.DTO
{
    public class UserDTO
    {
        public String LastName { get; set; }
        public String Name { get; set; }
        public String Username { get; set; }
        public String Email { get; set; }
        public Boolean Disabled { get; set; }
        public Boolean Activated { get; set; }
        public DateTime? ActivatedDate { get; set; }
        public int Id { get; set; }
        public String ActivationHash { get; set; }
        public List<String> Permissions { get; set; }
        public ProfileDTO Profile { get; set; }
        public bool ChangePasswordRequired { get; set; }
        public string Token { get; set; }
    }
}
