using AutoMapper;
using ProyectoApiContable.Dtos.Catalogos;
using ProyectoApiContable.Dtos.FilasPartidas;
using ProyectoApiContable.Dtos.Partidas;
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
        }

        private void MapsForCuentas()
        {
            CreateMap<Cuenta, CuentaDto>();
            CreateMap<CreateCuentaDto, Cuenta>().ReverseMap();
        }

        private void MapsForPartidas()
        {
            CreateMap<Partida, PartidaDto>();
            CreateMap<CreatePartidaDto, Partida>().ReverseMap();
        }

        private void MapsForFilasPartidas()
        {
            CreateMap<FilasPartida, FilasPartidaDto>();
            CreateMap<CreateFilasPartidaDto, FilasPartida>().ReverseMap();
        }
    }
}

