using System.Text;
using Asoode.Application.Core.Primitives.Enums;

namespace Asoode.Application.Core.Helpers;

// ReSharper disable once InconsistentNaming
public static class IOHelper
{
    public static string[] ContactExtensions => new[] {".vcf"};
    public static string[] ImageExtensions => new[] {".png", ".jpg", ".jpeg", ".bmp", ".gif"};
    public static string[] VideoExtensions => new[] {".mp4"};

    public static string[] VideoAllExtensions =>
        new[] {".mp4", ".dat", ".mpeg", ".avi", ".mkv", ".ogg", ".ogv", ".webm"};

    public static void CopyFolder(string sourceFolder, string destFolder)
    {
        if (!Directory.Exists(destFolder))
            Directory.CreateDirectory(destFolder);
        var files = Directory.GetFiles(sourceFolder);
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            var dest = Path.Combine(destFolder, name);
            File.Copy(file, dest);
        }

        var folders = Directory.GetDirectories(sourceFolder);
        foreach (var folder in folders)
        {
            var name = Path.GetFileName(folder);
            var dest = Path.Combine(destFolder, name);
            CopyFolder(folder, dest);
        }
    }

    public static async Task<bool> Exists(string path)
    {
        return await Task.Run(() => File.Exists(path));
    }

    public static FileType GetFileType(string fileExt)
    {
        switch (fileExt.ToLower())
        {
            case ".jpg":
            case ".jpeg":
            case ".png":
            case ".gif":
            case ".tiff":
            case ".bmp":
                return FileType.Picture;

            case ".doc":
            case ".docx":
            case ".txt":
            case ".rtf":
                return FileType.Documents;

            case ".pdf":
                return FileType.Pdf;

            case ".xls":
            case ".xlsx":
                return FileType.Excel;

            case ".ppt":
            case ".pptx":
                return FileType.Powerpoint;

            case ".avi":
            case ".flv":
            case ".mpg":
            case ".mpeg":
            case ".mkv":
            case ".mp4":
            case ".3gp":
                return FileType.Video;

            case ".mp3":
            case ".aac":
            case ".wav":
            case ".amr":
                return FileType.Audio;

            case ".exe":
            case ".dll":
            case ".msi":
            case ".com":
            case ".asp":
            case ".aspx":
            case ".php":
            case ".py":
            case ".pl":
            case ".cgi":
                return FileType.Program;

            case ".rar":
            case ".7z":
            case ".zip":
            case ".tar":
            case ".tar.gz":
                return FileType.Archive;

            default:
                return FileType.General;
        }
    }

    public static string GetMimeType(string fileExt)
    {
        switch (fileExt.ToLower())
        {
            case ".jpg":
            case ".jpeg":
                return "image/jpeg";

            case ".png":
                return "image/png";

            case ".gif":
                return "image/gif";

            case ".tiff":
                return "image/tiff";

            case ".bmp":
                return "image/bmp";

            case ".doc":
            case ".docx":
                return "application/msword";

            case ".txt":
                return "text/plain";

            case ".rtf":
                return "application/rtf";

            case ".pdf":
                return "application/pdf";

            case ".xls":
            case ".xlsx":
                return "application/excel";

            case ".ppt":
            case ".pptx":
                return "application/mspowerpoint";

            case ".avi":
                return "video/avi";

            case ".flv":
                return "video/flv";

            case ".mpg":
            case ".mpeg":
                return "video/mpeg";

            case ".mkv":
                return "video/mkv";

            case ".mp4":
                return "video/mpeg";

            case ".mov":
                return "video/quicktime";

            case ".3gp":
                return "video/3gp";

            case ".mp3":
                return "audio/mpeg3";

            case ".aac":
                return "audio/aac";

            case ".wav":
                return "audio/wav";

            case ".amr":
                return "audio/amr";

            case ".exe":
            case ".dll":
            case ".msi":
            case ".com":
            case ".bak":
                return "application/octet-stream";

            case ".asp":
            case ".aspx":
                return "text/asp";

            case ".php":
                return "text/php";

            case ".py":
                return "text/py";

            case ".pl":
                return "text/pl";

            case ".rar":
            case ".7z":
            case ".zip":
            case ".tar":
            case ".tar.gz":
                return "application/x-compressed";

            default:
                return "text/plain";
        }
    }

    public static bool IsDirectory(string source)
    {
        var attr = File.GetAttributes(source);
        return attr.HasFlag(FileAttributes.Directory);
    }

    public static bool IsSpreadsheet(string extension)
    {
        switch (extension.Trim().ToLower())
        {
            case ".xls":
            case ".xlsx":
            case ".csv":
                return true;

            default:
                return false;
        }
    }

    public static bool IsPresentation(string extension)
    {
        switch (extension.Trim().ToLower())
        {
            case ".ppt":
            case ".pptx":
                return true;

            default:
                return false;
        }
    }

    public static bool IsImage(string extension)
    {
        return ImageExtensions.Contains(extension.ToLower());
    }

    public static bool IsPdf(string extension)
    {
        return extension.ToLower() == ".pdf";
    }

    public static bool IsAudio(string extension)
    {
        switch (extension.Trim().ToLower())
        {
            case ".mp3":
            case ".wav":
            case ".m4a":
                return true;

            default:
                return false;
        }
    }

    public static bool IsDocument(string extension)
    {
        switch (extension.Trim().ToLower())
        {
            case ".txt":
            case ".doc":
            case ".docx":
            // case ".pdf":
            case ".rtf":
                return true;

            default:
                return false;
        }
    }

    public static bool IsVideo(string extension, bool all = false)
    {
        if (all) return VideoAllExtensions.Contains(extension.ToLower());
        return VideoExtensions.Contains(extension.ToLower());
    }

    public static async Task<string> ReadText(string filePath)
    {
        using (var sourceStream = new FileStream(filePath,
                   FileMode.Open, FileAccess.Read, FileShare.Read,
                   4096, true))
        {
            var sb = new StringBuilder();

            var buffer = new byte[0x1000];
            int numRead;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                var text = Encoding.UTF8.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return sb.ToString();
        }
    }

    public static async Task WriteText(string filePath, string text)
    {
        var encodedText = Encoding.Unicode.GetBytes(text);

        using (var sourceStream = new FileStream(filePath,
                   FileMode.Append, FileAccess.Write, FileShare.None,
                   4096, true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        }
    }

    public static bool IsArchive(string extension)
    {
        switch (extension.Trim().ToLower())
        {
            case ".zip":
            case ".rar":
            case ".7z":
            case ".ace":
            case ".tar":
            case ".gz":
                return true;

            default:
                return false;
        }
    }

    public static bool IsExecutable(string extension)
    {
        return false;
    }

    public static bool IsCode(string extension)
    {
        return false;
    }

    public static bool IsOther(string extension)
    {
        return false;
    }
}