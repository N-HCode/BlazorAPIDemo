using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSBlazorAPI
{
    public class SwaggerOptions : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            //Gets the descriptions of all the Controllers
            var descriptor = context.ApiDescription.ActionDescriptor as ControllerActionDescriptor;


            //If the descriptor is not null and the controller name does not start with Token, then added
            //These parameters to the Swagger API
            if (descriptor != null && !descriptor.ControllerName.StartsWith("Token"))
            {
                operation.Parameters.Add(new OpenApiParameter()
                {
                    Name = "Token",
                    Description = "Access Token",
                    In = ParameterLocation.Header,
                    //Schema = new OpenApiSchema() { Type = "String" },
                    Required = true,
                    //Example = new OpenApiString("Tenant ID example"),

                });
            } 
        }
        

    }
}
