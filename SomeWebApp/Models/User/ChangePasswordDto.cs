namespace SomeWebApp.Models.User
{
    public class ChangePasswordDto
    {
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
}
