using API.Models;
using API.Models.DTOs;
using Mapster;

namespace API.Configs
{
    public class CustomerMapping
    {

        public static TypeAdapterConfig GetCustomerCreationDtoToCustomerMappingConfig()
        {
            var config = new TypeAdapterConfig();
            config.NewConfig<CustomerCreationDTO, Customer>()
                .Ignore(dest => dest.City);

            return config;
        }
    }
}
