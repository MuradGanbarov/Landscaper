using Landscaper.Areas.Admin.Models.Utilities.Enums;

namespace Landscaper.Areas.Admin.Models.Utilities.Extentions
{
    public static class FileValidator
    {
        public static bool IsValidType(this IFormFile file,FileType type)
        {
            switch(type)
            {
                case FileType.Image:
                    if (file.ContentType.Contains("image/")) return true;
                    break;

                case FileType.Audio:
                    if (file.ContentType.Contains("audio/")) return true;
                    break;

                case FileType.Video:
                    if (file.ContentType.Contains("video/")) return true;
                    break;
            }
            return false;
        }
        public static bool IsValidSize(this IFormFile file,int MaxSize,FileSize size)
        {
            switch(size)
            {
                case FileSize.Kilobyte:
                    if (file.Length <= MaxSize * 1024) return true;
                    break;
                case FileSize.Megabyte:
                    if (file.Length <= MaxSize * 1024*1024) return true;
                    break;
                case FileSize.Gigabyte:
                    if (file.Length <= MaxSize * 1024*1024*1024) return true;
                    break;
            }
            return false;
        }

        public static async Task<string> CreateAsync(this IFormFile file,string rootpath,params string[] folders)
        {
            string fileName = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf('.'));
            string path = rootpath;
            for(int i=0;i<folders.Length;i++)
            {
                path = Path.Combine(path, folders[i]);
            }
            path = Path.Combine(path, fileName);

            using(FileStream newFile = new FileStream(path,FileMode.Create))
            {
                await file.CopyToAsync(newFile);
            }
            return fileName;
        }

        public static void Delete(this string fileName,string rootpath,params string[] folders)
        {
            string path = rootpath;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }
            path = Path.Combine(path, fileName);
            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }

    }
}
