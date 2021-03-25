using SomeWebApp.Application.Checker;
using System.Text.RegularExpressions;

namespace SomeWebApp.Infrastructure.Checker
{
    public class EnteredDataChecker : IEnteredDataChecker
    {
        public bool CheckNicknameAndEmailAndPassword(string nickname, string email, string password)
        {
            if (CheckNickname(nickname) &&
                CheckEmail(email) &&
                CheckPassword(password) &&
                password != nickname &&
                password != email)
                return true;
            else
                return false;
        }

        public bool CheckNickname(string nickname)
        {
            if (string.IsNullOrEmpty(nickname) &&
                nickname.Length >= 4 &&
                nickname.Length <= 32 &&
                MatchingStringToPattern(nickname, "^[a-zA-Z0-9]+$"))
                return true;
            else
                return false;
        }

        public bool CheckEmail(string email)
        {
            if (string.IsNullOrEmpty(email) &&
                MatchingStringToPattern(email, "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}"))
                return true;
            else
                return false;
        }

        public bool CheckPassword(string password)
        {
            if (string.IsNullOrEmpty(password) &&
                password.Length >= 8 &&
                password.Length <= 32)
                return true;
            else
                return false;
        }

        private bool MatchingStringToPattern(string str, string pattern)
        {
            return Regex.Match(str, pattern, RegexOptions.IgnoreCase).Success;
        }
    }
}
