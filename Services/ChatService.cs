using speect_to_text_Api.DTO.ResponseDTO;
using speect_to_text_Api.Helpers;
using System.Data;
using Dapper;
using System.Text.Json;
using System.Net.Http.Headers;

namespace speect_to_text_Api.Services
{
    public class ChatService : Ichat
    {
        private readonly IDbConnection _db;
        private readonly ILogger<ChatService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly CloudinaryHelper _cloudinaryHelper;

        public ChatService(IDbConnection db, ILogger<ChatService> logger, IHttpClientFactory httpClientFactory, CloudinaryHelper cloudinaryHelper)
        {
            _db = db;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _cloudinaryHelper = cloudinaryHelper;
        }

        public async Task<VoicePromptResponse> AddVoicePromptAsync(int chatId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new VoicePromptResponse
                    {
                        success = false,
                        message = "Audio file is required."
                    };
                }

                // 1. Check whether chat exists
                //string chatQuery = $" SELECT chat_id FROM chats WHERE chat_id = {chatId}";

                //var existingChat = await _db.QuerySingleOrDefaultAsync<int?>(chatQuery);

                //if (existingChat == null)
                //{
                //    return new VoicePromptResponse
                //    {
                //        success = false,
                //        message = "Chat not found."
                //    };
                //}

                // 2. Send audio to speech-to-text API
                using var content = new MultipartFormDataContent();

                await using var fileStream = file.OpenReadStream();

                using var streamContent = new StreamContent(fileStream);

                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");

                content.Add(streamContent, "file", file.FileName);

                //var httpClientFactory = _httpClientFactory;

                var client = _httpClientFactory.CreateClient("SpeechToText");

                var response = await client.PostAsync("/transcribe", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Speech-to-text API returned {response.StatusCode}");

                    return new VoicePromptResponse
                    {
                        success = false,
                        message = "Unable to transcribe audio."
                    };
                }

                // 3. Read transcription
                //var responseText = await response.Content.ReadAsStringAsync();

                //string transcript;

                //try
                //{
                //    transcript = JsonSerializer.Deserialize<string>(responseText) ?? responseText;
                //}
                //catch
                //{
                //    transcript = responseText;
                //}

                var transcript = await response.Content.ReadAsStringAsync();
                transcript = transcript.Trim();

                //Upload audio file to Cloudinary

                //var audioFileUrl = await _cloudinaryHelper.UploadImage(file);
                string audioFileUrl = "";
                // 4. Save transcription
                string insertQuery = @$" INSERT INTO voice_prompts (chat_id, audio_file_url, transcribed_text, created_at)
                                                   VALUES ({chatId}, '{audioFileUrl}', '{transcript}', CURRENT_TIMESTAMP)
                                RETURNING voice_prompt_id;
                ";

                var voicePromptId = await _db.ExecuteScalarAsync<int>(insertQuery);

                // 5. Return result
                return new VoicePromptResponse
                {
                    success = true,
                    message = "Audio transcribed successfully.",
                    VoicePromptId = voicePromptId,
                    ChatId = chatId,
                    Text = transcript
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing voice prompt.");

                return new VoicePromptResponse
                {
                    success = false,
                    message = "An error occurred while processing the audio."
                };
            }
        }
    }
}
