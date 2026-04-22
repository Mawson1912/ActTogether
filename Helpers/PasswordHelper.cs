namespace ActTogether.Helpers
{
    // Matches RazorApp Helper pattern for password hashing
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool Verify(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                // Hash is not valid BCrypt (e.g. legacy plain-text password)
                return false;
            }
        }
    }
}
