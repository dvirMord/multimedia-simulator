using multimedia_simulator.constants;

namespace multimedia_simulator.Validators
{
    public static class MyValidators
    {
        public static void ValidateNotNullOrEmpty<T>(T? obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            if (obj is string str && string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(ValidatorsExceptions.ValueIsEmpty);
            }

            if (obj is IFormFile file && file.Length == 0)
            {
                throw new ArgumentException(ValidatorsExceptions.FileIsEmpty);
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
                    string.Format(ValidatorsExceptions.FileNotFoundTemplate, fileName));
            }
        }

        public static void ValidateFileExtension(string fileName)
        {
            string extension =
                Path.GetExtension(fileName).ToLowerInvariant();

            if (extension != Constants.TS_FILE_EXTENSION)
            {
                throw new InvalidOperationException(
                    ValidatorsExceptions.OnlyTsFilesSupported);
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
                    ValidatorsExceptions.FileAlreadyUploaded,
                    nameof(fileName));
            }
        }
    }
}