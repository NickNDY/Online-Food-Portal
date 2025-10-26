namespace Online_Food_Portal.Services
{
    public interface IPasswordService
    {
        public string EncryptPassword(string password);

        public bool VerifyPassword(string password, string hashedPassword);

        public bool NeedsRehash(string hashedPassword);
    }
}
