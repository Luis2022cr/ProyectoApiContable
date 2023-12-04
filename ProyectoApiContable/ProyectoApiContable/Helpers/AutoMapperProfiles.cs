using AutoMapper;
using ProyectoApiContable.Dtos.Catalogos;
using ProyectoApiContable.Dtos.EstadosPartidas;
using ProyectoApiContable.Dtos.FilasPartidas;
using ProyectoApiContable.Dtos.Partidas;
using ProyectoApiContable.Dtos.TiposCuentas;
using ProyectoApiContable.Entities;

namespace ProyectoApiContable.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            MapsForCuentas();
            MapsForPartidas();
            MapsForFilasPartidas();
            MapsForEstadosPartidas();
            MapsForTiposCuentas();
        }

        private void MapsForCuentas()
        {
            CreateMap<Cuenta, CuentaDto>();
            CreateMap<CreateCuentaDto, Cuenta>().ReverseMap();
            CreateMap<UpdateCuentaDto, Cuenta>().ReverseMap();
        }

        private void MapsForPartidas()
        {
            CreateMap<CreatePartidaDto, Partida>()
                .ForMember(dest => dest.FilasPartida, opt => opt.MapFrom(src => src.FilasPartida));

            CreateMap<CreateFilasPartidaDto, FilasPartida>();
            CreateMap<Partida, PartidaDto>()
                .ForMember(dest => dest.FilasPartida, opt => opt.MapFrom(src => src.FilasPartida))
                .ForMember(dest => dest.CreadoPor, opt => opt.MapFrom(src => src.CreadoPor.UserName))
                .ForMember(dest => dest.RevisadoPor, opt => opt.MapFrom(src => src.RevisadoPor != null ? src.RevisadoPor.UserName : null));
        }

        private void MapsForFilasPartidas()
        {
            CreateMap<FilasPartida, FilasPartidaDto>();
            CreateMap<CreateFilasPartidaDto, FilasPartida>().ReverseMap();
        }
        
        private void MapsForEstadosPartidas()
        {
            CreateMap<EstadoPartida, EstadosPartidaDto>();
            CreateMap<CreateEstadosPartidaDto, EstadoPartida>().ReverseMap();
            CreateMap<UpdateEstadosPartidaDto, EstadoPartida>().ReverseMap();
        }

        private void MapsForTiposCuentas()
        {
            CreateMap<TipoCuenta, TiposCuentaDto>();
            CreateMap<CreateTiposCuentaDto, TipoCuenta>().ReverseMap();
            CreateMap<UpdateTiposCuentaDto, TipoCuenta>().ReverseMap();
        }
    }
}

