using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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





    }
}
