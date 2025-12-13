using backend.Database;
using backend.Models;

namespace backend.Services;

public class UserService : ServiceBase<User>
{
    public UserService(ApplicationDbContext context)
        : base(context)
    {
        
    }
}
//private readonly byte[] _hmacKey = Encoding.UTF8.GetBytes("super-secret-key-change-me");
//string fontPath = Path.Combine(AppContext.BaseDirectory, "Fonts", "OpenSans_Condensed-Regular.ttf");
//const string CHARS = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

//public CaptchaResponse Generate()
//{
//    var code = GenerateCode(5);
//    var imageBytes = GenerateImage(code);
//    var token = CreateToken(code);

//    return new CaptchaResponse
//    {
//        Token = token,
//        ImageBase64 = Convert.ToBase64String(imageBytes)
//    };
//}

//public bool Validate(string token, string userAnswer)
//{
//    var json = ValidateToken(token);
//    var payload = JsonConvert.DeserializeObject<CaptchaPayload>(json)!;

//    return payload.Answer.Equals(userAnswer, StringComparison.OrdinalIgnoreCase);
//}

//private string GenerateCode(int length)
//{
//    var rnd = RandomNumberGenerator.Create();
//    var bytes = new byte[length];
//    rnd.GetBytes(bytes);

//    var result = new StringBuilder();

//    foreach (var b in bytes)
//        result.Append(CHARS[b % CHARS.Length]);

//    return result.ToString();
//}

//private byte[] GenerateImage(string code)
//{
//    var collection = new FontCollection();
//    var fontFamily = collection.Add(fontPath);
//    var font = fontFamily.CreateFont(32, FontStyle.Bold);

//    using var img = new Image<Rgba32>(200, 70);
//    img.Mutate(ctx =>
//    {
//        ctx.Fill(Color.White);
//        ctx.DrawText(code, font, Color.Black, new PointF(20, 15));
//    });

//    using var ms = new MemoryStream();
//    img.Save(ms, new PngEncoder());
//    return ms.ToArray();
//}

//private string CreateToken(string answer)
//{
//    var payload = new CaptchaPayload { Answer = answer };
//    var json = JsonConvert.SerializeObject(payload);
//    var payloadBytes = Encoding.UTF8.GetBytes(json);

//    using var hmac = new HMACSHA256(_hmacKey);
//    var signature = hmac.ComputeHash(payloadBytes);

//    return Convert.ToBase64String(payloadBytes) + "." + Convert.ToBase64String(signature);
//}

//private string ValidateToken(string token)
//{
//    var parts = token.Split('.');
//    if (parts.Length != 2)
//        throw new Exception("Invalid token");

//    var payloadBytes = Convert.FromBase64String(parts[0]);
//    var signatureBytes = Convert.FromBase64String(parts[1]);

//    using var hmac = new HMACSHA256(_hmacKey);
//    var expectedSig = hmac.ComputeHash(payloadBytes);

//    if (!CryptographicOperations.FixedTimeEquals(signatureBytes, expectedSig))
//        throw new Exception("Invalid signature");

//    return Encoding.UTF8.GetString(payloadBytes);
//}