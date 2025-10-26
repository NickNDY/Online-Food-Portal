using Microsoft.AspNetCore.Identity;
using Online_Food_Portal.Models;
using System.Security.Cryptography;
using System.Text;

namespace Online_Food_Portal.Services
{
    /// <summary>
    /// Custom password hashing for securing stored passwords with top of the line security
    /// We wouldn't want a bad actor taking control of someone's food ordering account, now would we?
    /// </summary>
    public class PasswordService : IPasswordService, IPasswordHasher<IdentityUserModel>
    {
        const int BCRYPT_WORKFACTOR = 11;

        /// <summary>
        /// Encrypt a password using a pre-hash with SHA512 to allow longer passwords before
        /// hashing with BCrypt for enhanced security
        /// </summary>
        /// <param name="password">The raw input password for hashing</param>
        /// <returns>The SHA512 + BCrypt hashed password</returns>
        public string EncryptPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(PreHashPassword(password), BCRYPT_WORKFACTOR);
        }

        /// <summary>
        /// Hashes a raw password for storage in a local database
        /// </summary>
        /// <param name="user">The user whose password is being hashed</param>
        /// <param name="password">The raw password being hashed</param>
        /// <returns>The hashed password</returns>
        public string HashPassword(IdentityUserModel user, string password)
        {
            return EncryptPassword(password);
        }

        /// <summary>
        /// Determines whether a stored password needs to be rehashed according to a higher
        /// BCrypt workload
        /// </summary>
        /// <param name="hashedPassword">The hashed password in storage</param>
        /// <returns>True if it needs to be rehashed</returns>
        public bool NeedsRehash(string hashedPassword)
        {
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, BCRYPT_WORKFACTOR);
        }

        /// <summary>
        /// Verifies a hashed password matches the password provided by the user
        /// </summary>
        /// <param name="user">The user attempting to log in</param>
        /// <param name="hashedPassword">The hashed password stored in a local database</param>
        /// <param name="providedPassword">The raw password provided by the user</param>
        /// <returns>The result of the verification:
        ///   Success: The password was correct
        ///   Success needs rehash: The password was correct but the work factor has increased
        ///   Failed: The password was incorrect
        /// </returns>
        public PasswordVerificationResult VerifyHashedPassword(IdentityUserModel user, string hashedPassword, string providedPassword)
        {
            if (VerifyPassword(providedPassword, hashedPassword))
            {
                return NeedsRehash(hashedPassword) ? PasswordVerificationResult.SuccessRehashNeeded : PasswordVerificationResult.Success;
            }
            return PasswordVerificationResult.Failed;
        }

        /// <summary>
        /// Verifies the input password against the stored hashed password
        /// </summary>
        /// <param name="password">The raw password entered by the user</param>
        /// <param name="hashedPassword">The hashed password stored in the database</param>
        /// <returns>True if the password is correct</returns>
        public bool VerifyPassword(string password, string hashedPassword)
        {

            string hashedData = PreHashPassword(password);

            return BCrypt.Net.BCrypt.Verify(hashedData, hashedPassword);
        }

        /// <summary>
        /// Hashes a password with SHA512
        /// </summary>
        /// <param name="password">The raw input password for hashing</param>
        /// <returns>The SHA512 hashed password</returns>
        private string PreHashPassword(string password)
        {
            byte[] hashedData;

            using (SHA512 sHA512 = SHA512.Create())
            {
                hashedData = sHA512.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return Encoding.UTF8.GetString(hashedData);
        }
    }
}
