using MediatR;
using Minimal.Application.Auth.ChangePassword;
using Minimal.Application.Auth.Login;
using Minimal.Application.Auth.SignIn;

namespace Minimal.API.Endpoints;

public class AuthEndpoints : IEndpointGroup
{
    public static void Map(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        group.MapPost("/login", async (ISender mediator, 
            LoginCommand command) =>
            {
                return await mediator.Send(command);
            });

        group.MapPost("/signin", async (ISender mediator, 
            SignInCommand command) =>
            {
                return await mediator.Send(command);
            });

        group.MapPost("/change-password", async (ISender mediator,
            UpdatePlayerCommand command) =>
        {
            return await mediator.Send(command);
        });
    }
}
