using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using speect_to_text_Api.Services;

namespace speect_to_text_Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly Ichat _chat;

        public ChatController(Ichat chat)
        {
            _chat = chat;
        }

        [HttpPost("chats/{chatId}/voice-prompts")]
        public async Task<IActionResult> AddVoicePrompt(int chatId, IFormFile file)
        {
            var result = await _chat.AddVoicePromptAsync(chatId, file);

            if (!result.success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
