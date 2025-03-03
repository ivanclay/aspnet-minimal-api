
using RangoAgil.API.Entities;

namespace RangoAgil.API.EndpointFilters;

public class RangoIsLockedFilter : IEndpointFilter
{
    public readonly int _lockedFilterId;

    public RangoIsLockedFilter(int lockedFilterId)
    {
        _lockedFilterId = lockedFilterId;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        //this is just an exercise
        int rangoId;

        if (context.HttpContext.Request.Method == "PUT")
        {
            rangoId = context.GetArgument<int>(2);
        }
        else if (context.HttpContext.Request.Method == "DELETE")
        {
            rangoId = context.GetArgument<int>(1);
        }
        else
        {
            throw new NotSupportedException("This filter is not supported for this scenario.");
        }

        if (rangoId == _lockedFilterId)
        {
            return TypedResults.Problem(new()
            {
                Status = 400,
                Title = "Rango is perfect!",
                Detail = "You can't change or delete this."
            });
        }

        var result = await next.Invoke(context);
        return result;
    }
}
