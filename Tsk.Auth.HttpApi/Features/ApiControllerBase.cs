using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Tsk.Auth.HttpApi.Features;

[ApiController]
[Consumes(MediaTypeNames.Application.Json)]
[Produces(MediaTypeNames.Application.Json)]
public abstract class ApiControllerBase : ControllerBase;
