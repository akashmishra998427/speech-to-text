using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace speect_to_text_Api.Helpers
{
    public class CloudinaryHelper
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public CloudinaryHelper(IConfiguration config, ILogger<CloudinaryHelper> logger)
        {
            _config = config;
            _logger = logger;
            var account = new CloudinaryDotNet.Account(
                _config["CloudinarySettings:CloudName"],
                _config["CloudinarySettings:ApiKey"],
                _config["CloudinarySettings:ApiSecret"]
            );
            _cloudinary = new CloudinaryDotNet.Cloudinary(account);
        }

        public async Task<string> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return string.Empty;
                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                return uploadResult.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image to Cloudinary.");
                throw;
            }
        }
    }
}
