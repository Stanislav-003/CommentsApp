using backend.Database;
using backend.Models;

namespace backend.Services;

public class AuthService : ServiceBase<User>
{
    private readonly IConfiguration _configuration;

    public AuthService(
        ApplicationDbContext context,
        IConfiguration configuration) : base(context)
    {
        _configuration = configuration;
    }
}
