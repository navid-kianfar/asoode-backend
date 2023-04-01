using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Asoode.Core.Contracts.General;
using Asoode.Core.Extensions;
using Asoode.Core.Helpers;
using Asoode.Core.Primitives;
using Asoode.Core.ViewModels.General;
using Color = System.Drawing.Color;

namespace Asoode.Backend.Services
{
    public class CaptchaBiz : ICaptchaBiz
    {
        private readonly int ChallengeLength = 5;
        private readonly int width = 150;
        private readonly int height = 42;
        

        private string GenerateChallenge()
        {
            var from = (int) Math.Pow(10, ChallengeLength - 1);
            var to = (int) Math.Pow(10, ChallengeLength);
            return new Random(DateTime.UtcNow.Millisecond).Next(from, to).ToString();
        }

        private Bitmap GenerateNoise()
        {
            Color whiteColor = Color.White;
            Color noiseColor = Color.FromArgb(85, 193, 214);
            
            var r = new Random();
            var bmp = new Bitmap(width, height);
            using (var gfx = Graphics.FromImage(bmp))
            using (var brush = new SolidBrush(noiseColor))
            {
                gfx.FillRectangle(brush, 0, 0, width, height);
            }

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
            {
                var num = r.Next(0, 100);
                if (num % 2 == 0) continue;
                bmp.SetPixel(x, y, whiteColor);
            }

            return bmp;
        }

        private Image DrawText(string text)
        {
            FontFamily fontFamily = new FontFamily("Arial");
            Font font = new Font(fontFamily, 30, FontStyle.Italic, GraphicsUnit.Pixel);
            Color textColor = Color.FromArgb(107, 61, 141);
            
            Image img = new Bitmap(1, 1);
            var drawing = Graphics.FromImage(img);
            img.Dispose();
            drawing.Dispose();
            img = GenerateNoise();
            drawing = Graphics.FromImage(img);
            Brush textBrush = new SolidBrush(textColor);
            drawing.DrawString(text, font, textBrush, 20, 2);
            drawing.Save();
            textBrush.Dispose();
            drawing.Dispose();
            return img;
        }

        private string GenerateExpireToken()
        {
            var date = DateHelper.ToUnixTime(DateTime.UtcNow.AddMinutes(5)).ToString();
            return CryptoHelper.EncryptRijndael(date);
        }

        private string GenerateImage(string challenge)
        {
            using (var img = DrawText(challenge))
            {
                using (var ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Jpeg);
                    var imageBytes = ms.ToArray();
                    return $"data:image/jpeg;base64,{Convert.ToBase64String(imageBytes)}";
                }
            }
        }

        private string GenerateToken(string challenge)
        {
            var step_1 = CryptoHelper.Base64Encode(challenge);
            var step_2 = CryptoHelper.Md5(step_1);
            var step_3 = CryptoHelper.Sha1(step_2);
            return CryptoHelper.Md5(step_3);
        }

        public OperationResult<CaptchaRequestViewModel> Generate()
        {
            try
            {
                var challenge = GenerateChallenge();
                var image = GenerateImage(challenge);
                var token = GenerateToken(challenge);
                var expire = GenerateExpireToken();
                return OperationResult<CaptchaRequestViewModel>.Success(new CaptchaRequestViewModel
                {
                    Expire = expire, Image = image, Token = token
                });
            }
            catch
            {
                return OperationResult<CaptchaRequestViewModel>.Failed();
            }
        }

        public bool Ignore { get; set; }

        private bool Validate(string model, string token, string expire)
        {
            try
            {
                model = model.ConvertDigitsToLatin();
                if (Ignore) return true;
                if (token != GenerateToken(model)) return false;
                var expireDate = long.Parse(CryptoHelper.DecryptRijndael(expire));
                var now = DateHelper.ToUnixTime(DateTime.UtcNow);
                return now < expireDate;
            }
            catch
            {
                return false;
            }
        }

        public bool Validate(ICaptchaChallenge model)
        {
            return Validate(model.Captcha.Code, model.Captcha.Token, model.Captcha.Expire);
        }

        public bool Validate(string captcha)
        {
            var parts = (captcha ?? "").Split(';');
            if (parts.Length != 3) return false;
            return Validate(parts[0], parts[1], parts[2]);
        }
    }
}