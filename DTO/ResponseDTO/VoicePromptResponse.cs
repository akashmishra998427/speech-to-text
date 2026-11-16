namespace speect_to_text_Api.DTO.ResponseDTO
{
    public class VoicePromptResponse
    {
        public bool success { get; set; }

        public string message { get; set; }

        public int VoicePromptId { get; set; }

        public int ChatId { get; set; }

        public string Text { get; set; }
    }
}
