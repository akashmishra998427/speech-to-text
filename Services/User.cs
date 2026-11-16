using Dapper;
using Npgsql;
using speect_to_text_Api.DTO;
using speect_to_text_Api.DTO.ResponseDTO;
using speect_to_text_Api.Helpers;
using System.Data;


namespace speect_to_text_Api.Services
{
    public class User : Iuser
    {
        private readonly IDbConnection _db;
        private readonly ILogger<User> _logger;
        private readonly JwtHelper _jwt;
   
        public User(IDbConnection db, ILogger<User> logger, JwtHelper jwt)
        {
            _db = db;
            _logger = logger;
            _jwt = jwt; 
        }

        public async Task<RegResponse> RegisterUserAsync(UserReg user)
        {
            try
            {
                var PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
                string Query = @$"INSERT INTO Users(Name, Phone, email, Gender, Profession, Address, password_hash, created_at)
                                VALUES('{user.Name}', '{user.Phone}', '{user.Email}', '{user.Gender}', '{user.Profession}', N'{user.Address}', '{PasswordHash}', CURRENT_TIMESTAMP)
                ";
                int res = await _db.ExecuteAsync(Query);
                return new RegResponse
                {
                    success = true,
                    message = "Account created successfully!"
                };

            }
            catch (PostgresException ngpEx) when(ngpEx.SqlState == "23505") 
            {
                _logger.LogWarning($"Duplicate email registration attempt: {user.Email}");

                return new RegResponse
                {
                    success = false,
                    message = "An account with this email already exists."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while registering user. Email: {user.Email}");

                return new RegResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
        }

        public async Task<LoginRes> LoginUserAsync(Login user)
        {
            try
            {
                string Query = $"SELECT password_hash FROM Users WHERE email = '{user.email}'";

                var passwordHash = await _db.QuerySingleOrDefaultAsync<string>(Query);

                if (passwordHash == null)
                {
                    _logger.LogWarning($"Login failed. User not found. Email: {user.email}");

                    return new LoginRes
                    {
                        success = false,
                        message = "Invalid email or password."
                    };
                }

                bool validPassword = BCrypt.Net.BCrypt.Verify(user.password, passwordHash);

                if (!validPassword)
                {
                    _logger.LogWarning($"Login failed. Invalid password. Email: {user.email}");

                    return new LoginRes
                    {
                        success = false,
                        message = "Invalid email or password."
                    };
                }

                var request = new Dictionary<string, object>
                {
                    { "Email", user.email }
                };

                var token = _jwt.GenerateJwtToken(request);

                _logger.LogInformation($"User login successful. Email: {user.email}");

                return new LoginRes
                {
                    success = true,
                    message = "Login successful.",
                    Token = token
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login. Email: {user.email}");

                return new LoginRes
                {
                    success = false,
                    message = "An error occurred while logging in."
                };
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            throw new NotImplementedException();
        }
         
    }
}
