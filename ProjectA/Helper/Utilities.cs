using System.Text;
using System.Text.RegularExpressions;

namespace ProjectA.Helper
{
    public static class Utilities
    {
        public static int PAGE_SIZE = 20;
        public static void CreateIfMissing(string path)
        {
            bool folderExists = Directory.Exists(path);
            if (!folderExists)
                Directory.CreateDirectory(path);
        }
        public static bool IsInteger(string str)
        {
            Regex regex = new Regex(@"^[0-9]+$");
            if (String.IsNullOrWhiteSpace(str))
            {
                return false;
            }
            if (!regex.IsMatch(str))
            {
                return false;
            }
            return true;
        }
        public static string GetRandomKey(int length = 5)
        {
            string pattern = @"0123456789zxcvbnmasdfghjklqwertyuiop[]{}:~!@#$%^&*()+";
            Random rd = new Random();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }
            return sb.ToString();
        }
        public static string SEOUrl(string url)
        {
            url = url.ToLower().Trim();
            url = Regex.Replace(url, "áàạảãâấầậẩẫăắằặẳẵ", "a");
            url = Regex.Replace(url, "éèẹẻẽêếềệểễ", "e");
            url = Regex.Replace(url, "óòọỏõôốồộổỗơớờợởỡ", "o");
            url = Regex.Replace(url, "úùụủũưứừựửữ", "u");
            url = Regex.Replace(url, "íìịỉĩ", "i");
            url = Regex.Replace(url, "ýỳỵỷỹ", "y");
            url = Regex.Replace(url, "đ", "d");
            url = Regex.Replace(url, "[^a-z0-9-]", "");
            url = Regex.Replace(url, "(-)+", "-");

            return url;
        }
        public static string ToTitleCase(string str)
        {
            string result = str;
            if (!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for (int index = 0; index < words.Length; index++)
                {
                    var s = words[index];
                    if (s.Length > 0)
                    {
                        words[index] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }
        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newname = null)
        {
            try
            {
                if (newname == null) newname = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory);
                CreateIfMissing(path);
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory, newname);
                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif" };
                var fileExt = System.IO.Path.GetExtension(file.FileName).Substring(1);
                if (!supportedTypes.Contains(fileExt.ToLower()))
                {
                    return null;
                }
                else
                {
                    using (var stream = new FileStream(pathFile, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    return newname;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
