using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using static Repository.AuthRepository;

namespace Repository
{
    public interface IAuthRepository
    {
        IAuth SignIn(string username, string password);
        int? SignUp(string username, string password);
    }
    public class AuthRepository : IAuthRepository
    {
        private IConfiguration _configuration;
        public AuthRepository(IConfiguration configuration) => _configuration = configuration;

        public IAuth SignIn(string username, string password)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var query = "SELECT * FROM Users WHERE Username = @usr";
            IAuth user = connection.QueryFirstOrDefault<IAuth>(query, new { usr = username, pwd = password });
            if (user != null)
            {
                bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);
                return verified ? user : null;
            }
            else
            {
                return null;
            }
        }

        public int? SignUp(string username, string password)
        {
            var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var query = "IF NOT EXISTS (SELECT * FROM Users WHERE Username = @usr) INSERT INTO Users(Username, Password) VALUES(@usr, @pwd); SELECT CAST(SCOPE_IDENTITY() as int)";

            password = BCrypt.Net.BCrypt.HashPassword(password);

            return connection.QuerySingle<int>(query, new { usr = username, pwd = password });
        }

        public class IAuth
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

    }
}
