using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;

namespace backend.Helpers;

public static class CaptchaHelper
{
    private static readonly char[] Alphabet =
        "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789".ToCharArray();

    public static string GenerateCaptchaCode(int length = 6)
    {
        var code = new char[length];

        for (int i = 0; i < length; i++)
            code[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];

        return new string(code);
    }

    public static byte[] GenerateCaptchaImage(string captchaCode)
    {
        int width = 150;
        int height = 60;

        using var image = new Image<Rgba32>(width, height, Color.White);
        var random = new Random();
        var collection = new FontCollection();
        var family = collection.Add("Fonts/OpenSans_Condensed-Regular.ttf");
        var font = family.CreateFont(32, FontStyle.Bold);
        image.Mutate(ctx =>
        {
            float x = 8f;

            foreach (char c in captchaCode)
            {
                var angle = random.Next(-15, 15);
                var color = Color.FromRgb(
                    (byte)random.Next(50, 200),
                    (byte)random.Next(50, 200),
                    (byte)random.Next(50, 200));

                float y = random.Next(5, 25);
                ctx.DrawText(c.ToString(), font, color, new PointF(x, y));
                x += 22;
            }
            for (int i = 0; i < 4; i++)
            {
                var p1 = new PointF(random.Next(width), random.Next(height));
                var p2 = new PointF(random.Next(width), random.Next(height));
                var lineColor = Color.FromRgb(
                    (byte)random.Next(150, 255),
                    (byte)random.Next(150, 255),
                    (byte)random.Next(150, 255));
                ctx.DrawLine(lineColor, 1f, new[] { p1, p2 });
            }
        });

        using var ms = new MemoryStream();
        image.Save(ms, new PngEncoder());
        return ms.ToArray();
    }
}
