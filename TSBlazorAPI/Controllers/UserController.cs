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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TSBlazorAPI.Models;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace TSBlazorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly JWTSettingsModel _jwtSettings;

        public UserController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IOptions<JWTSettingsModel> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            //get the value of the IOptions and assigning it to the JWTSettingsModel
            //This is to get the serect key. This is injected thanks to the Startup.cs
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost]
        [Route("register")]
        public async Task<Object> Register(UserModel userInfo)
        {
            //search to see if there is already an existing user with the same
            //username or email (in this case email and username are the same)
            var existingUser = await _userManager.FindByEmailAsync(userInfo.email);

            if (existingUser != null)
            {
                return "Email Address is already taken";
            }

            //Do not put the password here because it needs to be encrpyted
            var identityUser = new IdentityUser()
            {
                UserName = userInfo.email,
                Email = userInfo.email,
               
            };


            try
            {
                //put password here as it will be encrypted by userManager
                var result = await _userManager.CreateAsync(identityUser, userInfo.password);
                return Ok(result);
            }
            catch (Exception ex)
            {

                throw ex;
            }       

        }


        //[HttpGet]
        //[Route("login")]
        //public async Task<Object> Login(UserModel userInfo)
        //{
        //    var user = await _userManager.FindByEmailAsync(userInfo.email);
        //    if(await _userManager.CheckPasswordAsync(user, userInfo.password))
        //    {
        //        //Token Handler is what is used to create token
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        //get the secret key from appsetting. convert that by into byptes and then
        //        //we will use it for encrptyion
        //        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
        //        //Security Token Descriptor is the details or configuration of the
        //        //token itself. This will be used by the token handler to create a token.
        //        var tokenDescriptor = new SecurityTokenDescriptor
        //        {
        //            //Subject is the name identifier. This is where you add the claims of the
        //            //token
        //            Subject = new ClaimsIdentity(new Claim[]
        //            {
        //                new Claim(ClaimTypes.Name, userInfo.email)

        //            }),
        //            //Expires is when the token will expire. In this example it will expire
        //            //After 1 day of the current time it was created
        //            Expires = DateTime.UtcNow.AddDays(1),
        //            //this signin Credentails takes out secret key and create a new key
        //            //using the algorithm we provided.
        //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
        //            SecurityAlgorithms.HmacSha256Signature)

        //        };

        //        //use our tokenhandle to create a token based on the configuration
        //        //we provided.
        //        var token = tokenHandler.CreateToken(tokenDescriptor);

                

        //    }

        //    return null;
        //}


    }
}
