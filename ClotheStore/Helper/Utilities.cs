using System.Text;
using System.Text.RegularExpressions;

namespace ClotheStore.Helper
{
    public static class Utilities
    {

        public static string ToTitleCase(string str)
        {
            string result = str;
            if (!string.IsNullOrEmpty(str))
            {
                var words = str.Split(' ');
                for (int i = 0; i < words.Length; i++) {
                    var s = words[i];
                    if (s.Length > 0)
                    {
                        words[i] = s[0].ToString().ToUpper() + s.Substring(1);
                    }
                }
                result = string.Join(" ", words);
            }
            return result;
        }
        public static string StripHTML(string input)
        {
            try
            {
                if (!string.IsNullOrEmpty(input))
                {
                    return Regex.Replace(input, "< *?>", String.Empty);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        public static bool IsValidateEmail(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static void CreateIfMissing(string path)
        {
            bool folderExits = Directory.Exists(path);
            if (!folderExits)
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string RandomKey(int length = 5)
        {
            string pattern = @"0123456789zxcvbnmasdfghjklqwertyuiop[]{}:~!@#$%^&*()+";
            Random rd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0,pattern.Length)]);
            }
            return sb.ToString();
        }

        public static string SEOUrl(string url)
        {
            url = url.ToLower();
            url = Regex.Replace(url, @"[áàạảãẳẵăắặằẩâấẫậ]", "a");
            url = Regex.Replace(url, @"[éèẹẽêếềễệẻể]", "e");
            url = Regex.Replace(url, @"[òóọõỏổởôốồỗộơớờỡợ]", "o");
            url = Regex.Replace(url, @"[íìịĩỉ]", "i");
            url = Regex.Replace(url, @"[ýỳỹỵỷ]", "y");
            url = Regex.Replace(url, @"[ũủùúụưửừứựữ]", "u");
            url = Regex.Replace(url, @"[đ]", "d");

            //Chỉ cho phép nhận: [^0-9a-z-\s]
            url = Regex.Replace(url.Trim(), @"[^0-9a-z-\s]", "").Trim();
            //Xử lý nhiều hơn 1 khoảng trắng --> 1 kt
            url = Regex.Replace(url.Trim(), @"\s+", "-");
            //Thay khoảng trắng bằng
            url = Regex.Replace(url, @"\s", "-");

            while (true)
            {
                if (url.IndexOf("--") != -1)
                {
                    url = url.Replace("--", "-");
                }
                else
                {
                    break;
                }
            }
            return url;
        }

        public static async Task<string> UploadFile(Microsoft.AspNetCore.Http.IFormFile file, string sDirectory, string newName)
        {
            try
            {
                if (newName == null) newName = file.FileName;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory);
                CreateIfMissing(path);
                string pathFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", sDirectory, newName);
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
                    return newName;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
