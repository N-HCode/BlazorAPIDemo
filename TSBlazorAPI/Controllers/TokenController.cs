using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TSBlazorAPI.Data;
using TSBlazorAPI.Models;

namespace TSBlazorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase 
    {
    

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        //In .NET the Token was built in by default. For .CORE, it is not built in.
        //So we have to manually create the /Token pathway. Also, we need some
        //nuget packages below, which were default in .NET

        //microsoft.AspNetCore.authenication.JWTbearer for authencation
        //Microsoft for .CORE disable the authenication be default to allow
        //for easy third party intergated. This seems part of their
        //modular design for the program. So we have to add it in
        //using the manage nuget packages for .CORE

        //System.IdentityModel.Tokens.Jwt. Provides support for Token JSON

        //Again we do not need to worry how the constructor get the parameters
        //Due to dependentcy injection built in to .CORE API
        //Will need to modify dependcy injection when we need something
        //that is not built in
        public TokenController(ApplicationDbContext context
            , UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("/token")]
        [HttpPost]
        public async Task<IActionResult> Create(UserModel userModel, string grant_type)
        {
            if (await IsValidUsernameAndPassword(userModel))
            {
                return new ObjectResult(await GenerateToken(userModel.email));
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<bool> IsValidUsernameAndPassword(UserModel userModel)
        {
            var user = await _userManager.FindByEmailAsync(userModel.email);
            return await _userManager.CheckPasswordAsync(user, userModel.password);
        }

        //We have to manually type in the token creation
        private async Task<dynamic> GenerateToken(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            var roles = from ur in _context.UserRoles
                        join r in _context.Roles
                        on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new { ur.UserId, ur.RoleId, r.Name };


            //authenication uses claims based on the claim schema.
            //https://docs.microsoft.com/en-us/dotnet/api/system.security.claims.claim?view=netcore-3.1
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username)
                    , new Claim(ClaimTypes.NameIdentifier, user.Id)
                    //Nbf means the not active before datetime.
                    //We want it to be active immediately, so we get the time now
                    //Then we convert it to seconds and then to a string.
                    , new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now)
                        .ToUnixTimeSeconds().ToString())
                    //This is when the token expires
                    //We use the .AddDays(1) to add one day before it expires
                    , new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1))
                        .ToUnixTimeSeconds().ToString())
                };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));

            }

            var token = new JwtSecurityToken(
                new JwtHeader(
                    //Use encyption for signing it.
                    new SigningCredentials(
                        //use this ket for the signing encrption. This is vert important
                        //This key must not get out.
                        //We will not store it in code. We store it now for testing.
                        //However, we will make a random key and store it somewhere
                        //protected later on.
                        new SymmetricSecurityKey(Encoding.UTF8
                            .GetBytes("MySecretKeyIsSceretSoDoNotTell")),
                            SecurityAlgorithms.HmacSha256)),
                    new JwtPayload(claims)
                );

            var output = new
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
            };

            return output;

        }
    }
}
