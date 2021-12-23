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
                //.Map(dest => dest.FirstName, src => src.Email)
                //.Map(dest => dest.LastName, src => src.Email)
                //.Map(dest => dest.Adress, src => src.Adress)
                //.Map(dest => dest.ZipCode, src => src.ZipCode)
                //.Map(dest => dest.Email, src => src.Email)
                //.Map(dest => dest.PhoneNumber, src => src.PhoneNumber);
            return config;
        }
    }
}
