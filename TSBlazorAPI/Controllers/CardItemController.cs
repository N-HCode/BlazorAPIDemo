using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TSBlazorAPILibrary.Models;
using TSBlazorAPILibrary.DataAccess;
using System.Security.Claims;

//This adds the standards status code conventions like 404 etc.
//This will help with swagger documentation.

[assembly: ApiConventionType(typeof(DefaultApiConventions))]


namespace TSBlazorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CardItemController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CardItemController(IConfiguration config)
        {
            _config = config;
        }

        [Authorize]//(Roles = "Admin,Manager")]
        //.CORE requires Http method to be expicitly defined
        [HttpGet]
        public List<CardItemModel> Get()
        {
            //This makes it so we can use the token pass in from the header of an
            //HttpRequestMessage to get the userId. This also help make sure that user only
            //get the items that is their's.
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CardItemData data = new CardItemData(_config);
            return data.GetCardItem(userId);
        }

        [Authorize]
        [HttpPost]
        public void Post(CardItemModel item)
        {
            //Get the Id of the current user logged in and add it in the
            //CardItemModel
            item.CreatorId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            CardItemData data = new CardItemData(_config);
            data.SaveCardItem(item);
        }

        [Authorize]
        [HttpDelete]
        public void Delete(int cardId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            CardItemData data = new CardItemData(_config);

            data.DeleteCardItem(userId, cardId);

        }



    }
}
