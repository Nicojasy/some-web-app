using System;

namespace SomeWebApp.Application.Auth
{
    public interface IPasswordHasher
    {
        string Hash(string password);

        ///<summary>Comparing of password and hash.</summary>
        ///<returns><strong>Verified</strong> returns true if the hash is equal to the password.
        ///<strong>NeedsUpgrade</strong> is true if the hash is outdated.</returns>
        ///<exception cref="FormatException">Unexpected hash format</exception>
        (bool Verified, bool NeedsUpgrade) Check(string hash, string password);
    }
}
