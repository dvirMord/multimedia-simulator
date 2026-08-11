using video_simulator.constans;
namespace video_simulator.Validators
{
    public static class MyValidators
    {
        public static void ValidateNotNullOrEmpty<T>(T? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            if (obj is string str && string.IsNullOrEmpty(str))
            {
                throw new ArgumentException("Value is empty.");
            }

            if (obj is IFormFile file && file.Length == 0)
            {
                throw new ArgumentException("File length is empty.");
            }
        }

        public static void ValidateFileExists(
            string fileName,
            string storagePath)
        {
            string fullPath = Path.Combine(
                storagePath,
                fileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException(
                    $"File not found: {fileName}");
            }
        }

        public static void ValidateFileExtension(string fileName)
        {
            string extension =
                Path.GetExtension(fileName).ToLowerInvariant();

            if (extension != Constants.TS_FILE_EXTENSION)
            {
                throw new InvalidOperationException(
                    "Only .ts media files are supported.");
            }
        }

        public static void ValidateFileDoesNotExist(
            string fileName,
            string storagePath)
        {
            string fullPath = Path.Combine(
                storagePath,
                fileName);

            if (File.Exists(fullPath))
            {
                throw new ArgumentException(
                    "File is already uploaded.",
                    nameof(fileName));
            }
        }
    }
}