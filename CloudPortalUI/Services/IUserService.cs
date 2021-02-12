using CloudPortal.Models;
using CloudPortal.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace CloudPortal.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(User user);

        UserProfile GetById(string userid);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        //private List<User> _users = new List<User>
        //{
        //    new User { Id = 1, FirstName = "Test", LastName = "User", Username = "test", Password = "test" }
        //};

        private readonly AppSettings _appSettings;

        private readonly IConfiguration configuration;

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(configuration.GetConnectionString("Default"));
        }
        public UserService(IOptions<AppSettings> appSettings, IConfiguration config)
        {
            _appSettings = appSettings.Value;
            configuration = config;

        }


        public AuthenticateResponse Authenticate(User user)
        {
           
          //  var user = _users.SingleOrDefault(x => x.Username == model.Username && x.Password == model.Password);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt token
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

      
        public UserProfile GetById(string userid)
        {
            UserProfile usrpro = new UserProfile();
            using (MySqlConnector.MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("select * from userprofile where userid='" + userid + "'", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        usrpro.userid = Convert.ToString(reader["userid"]);
                        usrpro.fullname = Convert.ToString(reader["fullname"]);
                        usrpro.email = Convert.ToString(reader["email"]);
                        usrpro.lastlogin = Convert.ToDateTime(reader["lastlogin"]);
                        usrpro.phone = Convert.ToString(reader["phone"]);


                    }
                }
            }
            return usrpro;
        }

        // helper methods

        private string generateJwtToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Username.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
