namespace Minimal.API.Endpoints;

public interface IEndpointGroup
{
    static abstract void Map(IEndpointRouteBuilder app);
}
