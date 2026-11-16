using speect_to_text_Api.DTO.ResponseDTO;
namespace speect_to_text_Api.Services
{
    public interface Ichat
    {
        Task<VoicePromptResponse> GetChatResponse(string authToken);
    }
}
