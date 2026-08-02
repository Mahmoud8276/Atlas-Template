using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;

namespace Atlas.Template.Services.Helpers
{
    public static class FileHelper
    {

        /// <summary>
        /// Used to upload a specific file.
        /// </summary>
        /// <param name="file">The file you want to upload.</param>
        /// <param name="folderName">The folder name in which you want the file to be stored.</param>
        /// <returns></returns>
        public async static Task<string> UploadFile(IFormFile file, string folderName)
        {
            if (file == null)
                return String.Empty;

            string folderpath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName);

            string filename = $"{Guid.NewGuid()}{file.FileName}";
            string filepath = Path.Combine(folderpath, filename);
            using var fs = new FileStream(filepath, FileMode.Create);
            await file.CopyToAsync(fs);
            return filename;
        }

        /// <summary>
        /// Used to delete a specific file.
        /// </summary>
        /// <param name="fileName">The file name you want to delete.</param>
        /// <param name="folderName">The folder name inside 'files' folder, where your file exists.</param>
        /// <returns></returns>
        public async static Task DeleteFile(string fileName, string folderName)
        {
            if (fileName is not null && folderName is not null)
            {
                string filepath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\files", folderName, fileName);
                if (File.Exists(filepath))
                    File.Delete(filepath);
            }
        }
    }
}
