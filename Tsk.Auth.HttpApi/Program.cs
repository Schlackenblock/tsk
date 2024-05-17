using Tsk.Auth.HttpApi.Context;
using Tsk.Auth.HttpApi.FeaturesDiscovery;
using Tsk.Auth.HttpApi.JwtAuth.JwtBearerImplementation;
using Tsk.Auth.HttpApi.Swagger;
using Tsk.Auth.HttpApi.Passwords;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

webApplicationBuilder.AddTskAuthDbContext();
webApplicationBuilder.AddBCryptPasswordHandler();
webApplicationBuilder.AddJwtTokenAuth();
webApplicationBuilder.AddFeatures();
webApplicationBuilder.AddSwaggerGeneration();

var webApplication = webApplicationBuilder.Build();

webApplication.UseSwagger(Environments.Development);
webApplication.UseRouting();

webApplication.UseAuthentication();
webApplication.UseAuthorization();

webApplication.MapControllers();

webApplication.Run();
