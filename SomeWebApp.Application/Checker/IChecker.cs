namespace SomeWebApp.Application.Checker
{
    public interface IChecker
    {
        bool CheckNicknameAndEmailAndPassword(string nickname, string email, string password);
        bool CheckNickname(string nickname);
        bool CheckEmail(string email);
        bool CheckPassword(string password);
    }
}
