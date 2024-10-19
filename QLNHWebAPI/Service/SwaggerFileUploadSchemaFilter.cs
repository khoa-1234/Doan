using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace QLNHWebAPI.Service
{
    public class SwaggerFileUploadSchemaFilter: ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(IFormFile))
            {
                schema.Properties.Clear();
                schema.Properties.Add("ContentType", new OpenApiSchema { Type = "string" });
                schema.Properties.Add("ContentDisposition", new OpenApiSchema { Type = "string" });
                schema.Properties.Add("Headers", new OpenApiSchema { Type = "object" });
                schema.Properties.Add("Length", new OpenApiSchema { Type = "integer", Format = "int64" });
                schema.Properties.Add("Name", new OpenApiSchema { Type = "string" });
                schema.Properties.Add("FileName", new OpenApiSchema { Type = "string" });
            }
        }
    }
}
