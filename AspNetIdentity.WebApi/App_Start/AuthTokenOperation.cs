using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace AspNetIdentity.WebApi.App_Start
{
    public class AuthTokenOperation : IDocumentFilter
    {
        /// <summary>
        /// Created by Suraj Bundel on 18/05/2022
        ///
        /// </summary>
        /// <param name="grant_type"></param>
        /// <param name="name"></param>
        /// <param name="password"></param>
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Add("/oauth/token", new PathItem
            {
                post = new Operation
                {
                    tags = new List<string> { "Auth" },
                    consumes = new List<string>
                    {
                        "application/x-www-form-urlencoded"
                    },
                    parameters = new List<Parameter>
                    {
                        new Parameter
                        {
                            type="string",
                            name="grant_type",
                            required = true,
                            @in ="formdata",
                            @default="password"
                        },
                        new Parameter
                        {
                            type="string",
                            name="UserName",
                            required = true,
                            @in ="formdata"
                        },
                        new Parameter
                        {
                            type="string",
                            name="Password",
                            required = true,
                            @in ="formdata"
                        },
                    }
                }
            });
        }
    }
}