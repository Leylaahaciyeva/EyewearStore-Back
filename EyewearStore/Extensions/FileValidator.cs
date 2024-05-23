namespace EyewearStore.Extensions;

public static class FileValidator
{
    public static bool ValidateSize(this IFormFile file, int mb)
    {
        return file.Length < mb * 1024 * 1024;
    }

    public static bool ValidateType(this IFormFile file, string type)
    {
        return file.ContentType.Contains(type);
    }

    public static async Task<string> FileCreateAsync(this IFormFile file, params string[] roots)
    {
        string path = String.Empty;

        foreach (string p in roots)
        {
            path = Path.Combine(path, p);
        }
        string fileName = Guid.NewGuid() + file.FileName;

        path = Path.Combine(path, fileName);

        using (FileStream stream = new(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }


        return fileName;
    }

    public static void FileDelete(this string filename, params string[] paths)
    {
        string path = "";


        foreach (var p in paths)
        {
            path = Path.Combine(path, p);
        }

        path = Path.Combine(path, filename);

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
