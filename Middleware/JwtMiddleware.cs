using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ThumbpayContext;

namespace Thubpay.Middleware
{
    public class JwtMiddleware
    {
        public class JwtHandler
        {
            private readonly RequestDelegate _next;

            public JwtHandler(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last()?.Trim('\"');



                if (token != null)
                {
                    getUserDataFromToken(context, token);
                }
                await _next(context);
            }

            private void getUserDataFromToken(HttpContext context, string token)
            {
                try
                {
                    var tokenhandler = new JwtSecurityTokenHandler();
                    tokenhandler.ValidateToken(token, new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("don't try to crack me")),
                        ClockSkew = TimeSpan.Zero,
                        ValidateAudience = false,
                        ValidateIssuer = false,
                    }, out SecurityToken validatedToken
                       );
                    var jwtToken = (JwtSecurityToken)validatedToken;

                    string id = jwtToken.Claims.First(x => x.Type == "Userid").Value;
                    context.Items["Userid"] = id;
                }
                catch (Exception)
                { }
            }
        }
    }
}
