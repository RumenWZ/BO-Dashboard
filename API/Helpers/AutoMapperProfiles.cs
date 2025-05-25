using API.Models;
using API.Models.DTO.Appointment;
using API.Models.DTO.Business;
using API.Models.DTO.Service;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<Appointment, AppointmentDto>();
            CreateMap<CreateBusinessDto, Business>();
            CreateMap<CreateServiceDto, Models.Service>();
            CreateMap<Models.Service, ServiceDto>();
            CreateMap<ApplicationUserService, ApplicationUserServiceDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

        }
    }
}
