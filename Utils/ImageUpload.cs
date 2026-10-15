namespace VinylShop.Utils
{
    // Приймає лише справжні зображення: тип визначається за вмістом файлу,
    // а не за назвою, тож HTML чи скрипт не потрапить у wwwroot під виглядом картинки
    public static class ImageUpload
    {
        public const long MaxFileSize = 5 * 1024 * 1024;

        public const string AllowedFormatsText = "JPG, PNG, GIF або WEBP";

        public static string? Validate(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            if (file.Length > MaxFileSize)
            {
                return "Файл завеликий: максимум 5 МБ.";
            }

            return DetectExtension(file) == null
                ? $"Файл не є зображенням. Дозволені формати: {AllowedFormatsText}."
                : null;
        }

        public static async Task<string?> SaveAsync(IFormFile? file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            var extension = DetectExtension(file)
                ?? throw new InvalidOperationException("Uploaded file is not a supported image.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot/img/{folderName}");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/img/{folderName}/" + uniqueFileName;
        }

        private static string? DetectExtension(IFormFile file)
        {
            var header = new byte[12];
            int read;
            using (var stream = file.OpenReadStream())
            {
                read = stream.Read(header, 0, header.Length);
            }

            if (read >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                return ".jpg";

            if (read >= 8 && header.Take(8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
                return ".png";

            if (read >= 6 && (StartsWith(header, "GIF87a") || StartsWith(header, "GIF89a")))
                return ".gif";

            if (read >= 12 && StartsWith(header, "RIFF") && System.Text.Encoding.ASCII.GetString(header, 8, 4) == "WEBP")
                return ".webp";

            return null;
        }

        private static bool StartsWith(byte[] data, string signature)
        {
            return System.Text.Encoding.ASCII.GetString(data, 0, signature.Length) == signature;
        }
    }
}
