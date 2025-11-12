using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Service;

namespace SoftWC.Tests.Helpers;

/// <summary>
/// Clase de prueba que envuelve UserService para permitir mockear GetCurrentUserAsync
/// </summary>
public class TestUserService : UserService
{
    private readonly ApplicationDbContext _testContext;
    private readonly string _testUserId;
    private readonly Func<Task<Usuario>> _getCurrentUserFunc;

    public TestUserService(
        ApplicationDbContext context, 
        UserManager<Usuario> userManager, 
        IHttpContextAccessor httpContextAccessor,
        string testUserId) 
        : base(context, userManager, httpContextAccessor)
    {
        _testContext = context;
        _testUserId = testUserId;
        _getCurrentUserFunc = async () => 
        {
            var user = await _testContext.Users.FindAsync(_testUserId);
            return user ?? throw new InvalidOperationException($"Usuario con ID {_testUserId} no encontrado");
        };
    }

    // No podemos hacer override porque no es virtual, pero podemos crear un método nuevo
    // y usar composición en lugar de herencia para las pruebas
}

