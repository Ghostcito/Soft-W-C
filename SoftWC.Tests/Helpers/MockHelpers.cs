using Microsoft.AspNetCore.Identity;
using Moq;
using SoftWC.Models;

namespace SoftWC.Tests.Helpers;

public static class MockHelpers
{
    public static Mock<UserManager<Usuario>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<Usuario>>();
        return new Mock<UserManager<Usuario>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);
    }
}

