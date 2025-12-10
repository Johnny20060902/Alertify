namespace Alertify.Services.FileUpload
{
    public class FileUploadService
    {
        private readonly string _uploadsPath = Path.Combine("wwwroot", "uploads");
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly long _maxFileSize = 5 * 1024 * 1024;

        public async Task<(bool success, string message, string? filePath)> UploadImageAsync(
            IFormFile file,
            string subfolder,
            int? entityId = null)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return (false, "No se seleccionó ningún archivo", null);

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                    return (false, "Solo se permiten imágenes JPG y PNG", null);

                if (file.Length > _maxFileSize)
                    return (false, "El archivo no debe superar 5MB", null);

                var folderPath = Path.Combine(_uploadsPath, subfolder);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = entityId.HasValue
                    ? $"{entityId}_{Guid.NewGuid()}{extension}"
                    : $"{Guid.NewGuid()}{extension}";

                var fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/{subfolder}/{fileName}";
                return (true, "Archivo subido exitosamente", relativePath);
            }
            catch (Exception ex)
            {
                return (false, $"Error al subir archivo: {ex.Message}", null);
            }
        }

        public (bool success, string message) DeleteImage(string? relativePath)
        {
            try
            {
                if (string.IsNullOrEmpty(relativePath))
                    return (true, "No hay archivo para eliminar");

                var fullPath = Path.Combine("wwwroot", relativePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return (true, "Archivo eliminado exitosamente");
                }

                return (true, "El archivo no existe");
            }
            catch (Exception ex)
            {
                return (false, $"Error al eliminar archivo: {ex.Message}");
            }
        }
    }
}