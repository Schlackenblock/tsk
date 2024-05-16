using System.Linq.Expressions;
using System.Net.Mime;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Tsk.Auth.HttpApi.AspInfrastructure;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ValidationProblem(Expression<Func<object>> property, string message)
    {
        if (property.Body is not MemberExpression propertyExpression)
        {
            throw new ArgumentException($"Must be of type {nameof(MemberExpression)}.", nameof(property));
        }

        if (propertyExpression.Member.MemberType != MemberTypes.Property)
        {
            throw new ArgumentException("Must be property expression.", nameof(property));
        }

        var propertyName = propertyExpression.Member.Name;
        return ValidationProblem(propertyName, message);
    }

    private IActionResult ValidationProblem(string property, string message)
    {
        var validationState = new ModelStateDictionary();
        validationState.AddModelError(property, message);
        return ValidationProblem(validationState);
    }
}
