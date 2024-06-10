using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Html;
using System.Web;

namespace Evarosa.Utils
{
    public static class HtmlHelpers
    {
        public static string ConvertToUnSign(string text, bool isUrl = true)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            text = string.Concat(from c in text.ToLower().Normalize(NormalizationForm.FormD).Replace('đ', 'd')
                    .Replace('Đ', 'D')
                                 where CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark
                                 select c);
            text = Regex.Replace(text, "[^a-zA-Z0-9]", " ");
            text = new Regex("[\\s\\r\\n]+", RegexOptions.Compiled).Replace(text, " ");
            if (isUrl)
            {
                text = text.Trim().Replace(" ", "-");
            }

            return text;
        }

        public static string DateCountDown(DateTime? sdate)
        {
            if (!sdate.HasValue)
            {
                return null;
            }

            TimeSpan timeSpan = sdate.Value - DateTime.Now;
            if (timeSpan.Days >= 1)
            {
                return $"{timeSpan.Days} days";
            }

            if (timeSpan.Days < 1 && timeSpan.Hours > 0)
            {
                return $"{timeSpan.Hours} hours {timeSpan.Minutes} minutes";
            }
            return $"{timeSpan.Minutes} minutes";
        }

        public static string DateVn(DateTime? sdate)
        {
            if (!sdate.HasValue)
            {
                return null;
            }
            DateTime value = sdate.Value;
            DateTime now = DateTime.Now;
            if (now.Day - value.Day == 1 && value.Month == now.Month && value.Year == now.Year)
            {
                return $"Yesterday, at {value:HH:mm}";
            }

            if (value.Day != now.Day || value.Month != now.Month || value.Year != now.Year)
            {
                return value.ToString("dd/MM/yyyy");
            }
            TimeSpan timeSpan = now - value;
            int minutes = timeSpan.Minutes;
            int hours = timeSpan.Hours;
            if (hours < 1)
            {
                return $"{minutes} minutes ago";
            }

            return $"{hours} hours {minutes} minutes ago";
        }


        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        public static string ComputeHash(string password, string pepper, int iteration)
        {
            if (iteration <= 0) return password;

            using var sha256 = SHA256.Create();
            var salt = "vico@123";
            var passwordSaltPepper = $"{password}{salt}{pepper}";
            var byteValue = Encoding.UTF8.GetBytes(passwordSaltPepper);
            var byteHash = sha256.ComputeHash(byteValue);
            var hash = Convert.ToBase64String(byteHash);
            return ComputeHash(hash, pepper, iteration - 1);
        }

        public static string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var byteSalt = new byte[16];
            rng.GetBytes(byteSalt);
            var salt = Convert.ToBase64String(byteSalt);
            return salt;
        }

        public static string JsonUploader(this string imgStr, HttpRequest request, string folderName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderName);
            var url = $"{request.Scheme}://{request.Host}";
            var result = "";

            var fineObjs = new List<object>();
            string[] arrStrings = imgStr.Split(',');

            if (!string.IsNullOrEmpty(imgStr) && arrStrings.Count() > 0)
            {
                int i = 0;
                foreach (var fileName in arrStrings)
                {
                    var filePath = Path.Combine(path, fileName);

                    if (!File.Exists(filePath))
                    {
                        continue;
                    }

                    var fileSize = new FileInfo(filePath).Length;
                    var obj = new
                    {
                        id = i,
                        uuid = Guid.NewGuid(),
                        name = fileName,
                        thumbnailUrl = $"{url}/contents/{folderName}/{fileName}",
                        size = fileSize,
                    };

                    fineObjs.Add(obj);
                    i++;
                }

                result = JsonConvert.SerializeObject(fineObjs);
            }
            return result;
        }

        public static int GenerateRandomNumber(int minValue, int maxValue)
        {
            Random random = new Random();
            return random.Next(minValue, maxValue);
        }

        public static string _pegarIDYoutube(string urlYoutube)
        {
            string pattern = @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})";
            Match match = Regex.Match(urlYoutube, pattern, RegexOptions.IgnoreCase);
            if (!match.Success)
            {
                return null;
            }

            return match.Groups[1].Value;
        }

        public static HtmlString ShowYoutubeLazy(this string youtubeLink, string w = "auto", string h = "auto", string title = "")
        {
            if (string.IsNullOrEmpty(youtubeLink))
            {
                return null;
            }

            string videoId = _pegarIDYoutube(youtubeLink);

            if (videoId == null)
            {
                return null;
            }

            string escapedVideoId = HttpUtility.HtmlEncode(videoId);

            string iframeHtml = $"<iframe width='{w}' height={h} src='https://www.youtube.com/embed/{escapedVideoId}' title='{title}' frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share' referrerpolicy='strict-origin-when-cross-origin' allowfullscreen></iframe>";

            return new HtmlString(iframeHtml);
        }
    }
}
