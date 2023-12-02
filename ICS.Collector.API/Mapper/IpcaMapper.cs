using AutoMapper;
using ICS.Collector.API.DTO;
using ICS.Models.Models;

namespace ICS.Collector.API.Mapper;

public static class IpcaMapper
{
    public static void ToIpcaDto(this IMapperConfigurationExpression cfg)
    {
        cfg.CreateMap<Ipca, IpcaDto>()
            .ForMember(dest => dest.data, opt => opt.MapFrom(src => $"{src.D2N.ToUpper()}"))
            .ForMember(dest => dest.valor, opt => opt.MapFrom(src => src.V));
    }

}
