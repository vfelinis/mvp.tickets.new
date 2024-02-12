using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using mvp.tickets.data;
using mvp.tickets.data.Helpers;
using mvp.tickets.domain.Constants;
using mvp.tickets.domain.Helpers;
using mvp.tickets.domain.Models;
using mvp.tickets.web.Extensions;
using mvp.tickets.web.Middlewares;
using System.Globalization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterDependencies(builder.Configuration, builder.Environment);

var app = builder.Build();

var supportedCultures = new[]
{
    new CultureInfo("en-US")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    // Formatting numbers, dates, etc.
    SupportedCultures = supportedCultures,
    // UI strings that we have localized.
    SupportedUICultures = supportedCultures
});

app.UseForwardedHeaders();
//app.UseHttpsRedirection();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    if (context.Request.Query.ContainsKey("host"))
    {
        context.Response.Cookies.Append("host", context.Request.Query["host"]);
    }
    var path = context.Request.Path.Value.TrimStart('/').ToLower();
    if (path == "support")
    {
        if (context.User.Identity.IsAuthenticated && context.User.Claims.Any(s => s.Type == AuthConstants.AdminClaim))
        {
            using (var scope = context.RequestServices.CreateScope())
            {
                var user = System.Text.Json.JsonSerializer.Deserialize<UserModel>(context.User.Claims.First(s => s.Type == AuthConstants.UserDataClaim).Value);
                var userData = new UserJWTData(user.Email, user.CompanyId, JWTType.Support);
                var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var rootCompany = await dbContext.Companies.AsNoTracking().FirstOrDefaultAsync(s => s.IsRoot && s.IsActive);
                if (rootCompany == null || rootCompany.Id == user.CompanyId)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync("BadRequest");
                    return;
                }
                var code = TokenHelper.GenerateToken(userData, 5);
                context.Response.StatusCode = StatusCodes.Status302Found;
                context.Response.Headers[HeaderNames.Location] = $"https://localhost:5101/login/?code={code}&host=tickets.mvp-stack.ru";
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
    }
    else if (path.StartsWith(AppConstants.TicketFilesFolder))
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userId = int.Parse(context.User.Claims.First(s => s.Type == ClaimTypes.Sid).Value);
            var companyId = int.Parse(context.User.Claims.First(s => s.Type == AuthConstants.CompanyIdClaim).Value);
            if (!(path.StartsWith($"{AppConstants.TicketFilesFolder}/{companyId}/") && context.User.Claims.Any(s => s.Type == AuthConstants.EmployeeClaim))
                && !path.StartsWith($"{AppConstants.TicketFilesFolder}/{companyId}/{userId}/"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        else if (context.Request.Query.ContainsKey("token"))
        {
            var token = (string)context.Request.Query["token"];
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
            else
            {
                using (var scope = context.RequestServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    var fileName = Path.GetFileNameWithoutExtension(path);
                    var isValid = await dbContext.TicketCommentAttachments.AnyAsync(s => s.FileName == fileName && s.TicketComment.Ticket.Token == token);
                    if (!isValid)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                }
            }
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
    }

    await next.Invoke();
});
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers[HeaderNames.CacheControl] = new System.Net.Http.Headers.CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(7),
            MustRevalidate = true
        }.ToString();
    }
});
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseMiddleware<AuthMiddleware>();

app.MapControllers();
app.MapFallbackToFile("/index.html");

InitializationHelper.Initialize(app.Services);

app.Run();
