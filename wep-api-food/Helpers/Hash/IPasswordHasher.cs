namespace wep_api_food.Helpers.Hash
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPasswod(string hashPassword, string inputPassword);
    }
}
