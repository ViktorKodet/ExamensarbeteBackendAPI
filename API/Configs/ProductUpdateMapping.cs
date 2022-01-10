using API.Models;
using API.Models.DTOs;
using Mapster;

namespace API.Configs
{
    public class ProductUpdateMapping
    {

        public static TypeAdapterConfig GetProductUpdateMappingConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<ProductUpdateDTO, Product>()
                .IgnoreNullValues(true)
                .Ignore(dest => dest.Category)
                .Ignore(dest => dest.Company);

            return config;
        }
    }
}
