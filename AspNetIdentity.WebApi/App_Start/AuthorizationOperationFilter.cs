using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace AspNetIdentity.WebApi.App_Start
{
    public class AuthorizationOperationFilter : IOperationFilter
    {
        /// <summary>
        /// Created by Suraj Bundel on 18/05/2022
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="schemaRegistry"></param>
        /// <param name="apiDescription"></param>

        // use to get the response
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            (operation.parameters ?? (operation.parameters = new List<Parameter>())).Add(new Parameter
            {
                name = "Authorization",
                @in = "header",
                @default = "bearer ",
                description = "access token",
                required = false,
                type = "string"
            });
        }
    }
}