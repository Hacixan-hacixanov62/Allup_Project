using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_DataAccess.Helpers
{
    public static class FileManager
    {
        public static string SaveImage(this IFormFile file, string path, string folder)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(path, folder, fileName);
            using FileStream fileStream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(fileStream);
            return fileName;

        }
        public static bool CheckSize(this IFormFile file, int maxSize)
        {
            return file.Length >= maxSize;

        }
        public static bool CheckType(this IFormFile file, string[] types)
        {
            return types.Contains(file.ContentType);
        }

        public static bool DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }
            return false;

        }
        public static void DeleteFile(this string fileName, params string[] roots)
        {
            string path = "";

            foreach (var root in roots)
            {
                path = Path.Combine(path, root);
            }

            path = Path.Combine(path, fileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        //=========================================================

        //public static string GenerateFilePath(this IWebHostEnvironment env, string folder, string fileName)
        //{
        //    return Path.Combine(env.WebRootPath, folder, fileName);
        //}

        public static async Task SaveFileToLocalAsync(this IFormFile file, string path)
        {
            using FileStream stream = new(path, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        public static void DeleteFileFromLocal(this string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

    }
}
