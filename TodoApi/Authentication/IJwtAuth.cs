namespace TodoApi.Authentication
{
    public interface IJwtAuth
    {
        /// <summary>
        /// Authenticate the user and return the JWT.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        string Authenticate(string username, string password);
    }
}