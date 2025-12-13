using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Database;
using backend.Models;
using backend.Options;
using backend.Shared.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static backend.Utils.EncodeUtils;

namespace backend.Features.Users.Login;

public class LoginUserQueryHandler : IQueryHandler<LoginUserQuery, LoginResponse>
{
    private readonly JwtSettings _jwtSettings;
    private readonly ApplicationDbContext _context;

    public LoginUserQueryHandler(IOptions<JwtSettings> jwtOptions,
        ApplicationDbContext context)
    {
        _jwtSettings = jwtOptions.Value; 
        _context = context;
    }

    public async Task<LoginResponse> Handle(LoginUserQuery query, CancellationToken cancellationToken)
    {
        var securityKey = _jwtSettings.SecurityKey;

        if (securityKey == null)
            throw new AppException("FailToCreateAToken");

        var isUserByLoginExist = await _context.Credentials.Include(u => u.User).AnyAsync(u => u.Email == query.Email, cancellationToken);

        if (!isUserByLoginExist)
            throw new AppException("UserWithThisLoginDoesNotExist");

        var userCredentials = await GetCredentials(query.Email, query.Password, securityKey, cancellationToken);

        if (userCredentials == null)
            throw new AppException("UserCredentialsNotFound");

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userCredentials.UserId);

        if (user == null)
            throw new AppException("UserNotFound");
        
        if (user.Credential.IsBlocked)
            throw new AppException("UserAlreadyBlocked");

        var claims = new List<Claim>
        {
            new Claim("UserId", userCredentials.UserId.ToString()),
            new Claim(ClaimTypes.Name, userCredentials.User.FirstName)
        };

        switch (userCredentials.Role)
        {
            case CredentialRole.USER:
                claims.Add(new Claim(ClaimTypes.Role, UsersRoles.USER));
                break;
            case CredentialRole.ADMIN:
                claims.Add(new Claim(ClaimTypes.Role, UsersRoles.ADMIN));
                break;
            default:
                throw new AppException("UnsupportedRoleForVerifiedUser");
        }

        var jwt = new JwtSecurityToken(
            issuer: _jwtSettings.JwtIssuer,
            audience: _jwtSettings.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey)), SecurityAlgorithms.HmacSha256));

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new LoginResponse(token, DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds());
    }

    public async Task<Credential?> GetCredentials(string email, string password, string security, CancellationToken ct)
    {
        password += security;
        var cred = await _context.Credentials.Include(c => c.User).FirstOrDefaultAsync(u => u.Email == email, ct);
        if (cred == null || !EncodeUtilsRfc2898.Verify(password, cred.Password))
            return null;

        return cred;
    }
}
