using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace YogeshFurnitureAPI
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public AuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString().ToLower();

            if (path.StartsWith("/api/account") || path.StartsWith("/api/notification") ||
                (path.StartsWith("/api/product") && (path.Contains("get"))))
            {
                await _next(context);
                return;
            }

            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);

                        tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = true,
                            ValidIssuer = _configuration["JwtSettings:Issuer"],
                            ValidateAudience = true,
                            ValidAudience = _configuration["JwtSettings:Audience"],
                            ValidateLifetime = true
                        }, out SecurityToken validatedToken);

                        await _next(context);
                        return;
                    }
                    catch (SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("{\"error\":\"Token has expired.\"}");
                        return;
                    }
                    catch (Exception)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("{\"error\":\"Invalid token.\"}");
                        return;
                    }
                }
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("{\"error\":\"Authorization token is missing.\"}");
        }
    }
}
