using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using speect_to_text_Api.DTO;
using speect_to_text_Api.Services;

namespace speect_to_text_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Iuser _user;
        private readonly ILogger<AuthController> _logger;
        private readonly IValidator<UserReg> _validatorReg;
        private readonly IValidator<Login> _validatorLogin;

        public AuthController(Iuser user, ILogger<AuthController> logger, IValidator<UserReg> validatorReg, IValidator<Login> validatorLogin)
        {
            _user = user;
            _logger = logger;
            _validatorReg = validatorReg;
            _validatorLogin = validatorLogin;
        }


        [HttpPost("register")]
        public async Task<IActionResult> CreateUser([FromBody] UserReg user)
        {
            try
            {
                _logger.LogInformation($"Registration attempt for email: {user.Email}");

                var validationResult = await _validatorReg.ValidateAsync(user);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(e => e.ErrorMessage).ToArray());

                    _logger.LogWarning($"Registration validation failed for email: {user.Email}");

                    return BadRequest(new
                    {
                        success = false,
                        message = "Please correct the validation errors.",
                        errors = errors
                    });
                }

                var result = await _user.RegisterUserAsync(user);

                if (result.success)
                {
                    _logger.LogInformation($"User created successfully. Email: {user.Email}");

                    return Ok(new
                    {
                        success = true,
                        message = result.message
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = result.message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while creating user. Email: {user?.Email}");

                return StatusCode(500, new
                {
                    success = false,
                    message = "An internal server error occurred."
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            try
            {
                _logger.LogInformation($"Login attempt for email: {request.email}");

                var validationResult = await _validatorLogin.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(e => e.ErrorMessage).ToArray());

                    _logger.LogWarning($"Login validation failed for email: {request.email}");

                    return BadRequest(new
                    {
                        success = false,
                        message = "Please correct the validation errors.",
                        errors = errors
                    });
                }

                var result = await _user.LoginUserAsync(request);

                if (result.success)
                {
                    _logger.LogInformation($"User login successfully. Email: {request.email}");

                    return Ok(new
                    {
                        success = true,
                        message = result.message,
                        Token = result.Token
                    });
                }

                return BadRequest(new
                {
                    success = false,
                    message = result.message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Login faild user. Email: {request.email}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An internal server error occurred."
                });
            }
        }
    }
}
