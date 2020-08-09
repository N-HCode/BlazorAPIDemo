using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using TSBlazorAPILibrary.Internal.DataAccess;
using TSBlazorAPILibrary.Models;

namespace TSBlazorAPILibrary.DataAccess
{
    public class CardItemData
    {
        //IConfiguration is used to read configuration settings.
        //Example: appsettings.json, appsettings environment
        //User secrets
        //Environment variable
        //command-line arguments
        private readonly IConfiguration _config;

        public CardItemData(IConfiguration config)
        {
            _config = config;
        }

        //
        public List<CardItemModel> GetCardItem(string userId)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var p = new { UserId = userId };


            //Parameters (Name of Stored Procedure, Paramaters for that Stored Procedure
            //, Then finally the Connection String. The Connection String will reference
            //The app.settings.json file from the API project. Which is most likely the reason
            //We have to pass in the Iconfiguration.
            var output = sql.LoadData<CardItemModel, dynamic>("dbo.spCardItem_GetUserCardItems",
                p, "BlazorDatabase1");


            return output;
        }

        public void SaveCardItem(CardItemModel cardItem)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            sql.SaveData("dbo.spCardItem_Insert", cardItem, "BlazorDatabase1");
        }


        public void DeleteCardItem(string userId, int cardId)
        {
            SqlDataAccess sql = new SqlDataAccess(_config);

            var p = new { UserId = userId, CardId = cardId };

            sql.SaveData("dbo.spCardItem_DeleteACardItem", p, "BlazorDatabase1");
        }


    }
}
