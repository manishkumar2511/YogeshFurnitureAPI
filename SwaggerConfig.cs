using Microsoft.OpenApi.Models;

namespace YogeshFurnitureAPI
{
    public static class SwaggerConfig
    {
        public static void AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Yogesh Furniture API",
                    Version = "v1",
                    Description = "This API provides a backend service for managing the Yogesh Furniture store." +
                                  "For API documentation, please reach out at solutionmedicares@gmail.com."
                });
            });
        }
    }
}
