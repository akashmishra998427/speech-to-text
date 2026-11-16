using System.Net;
using speect_to_text_Api.DTO;
using speect_to_text_Api.DTO.ResponseDTO;

namespace speect_to_text_Api.Services
{
    public interface Iuser
    {
        Task<RegResponse> RegisterUserAsync(UserReg user);

        Task<LoginRes> LoginUserAsync(Login user);

        Task<bool> ForgotPasswordAsync(string email);

    }
}
