using API.Models;
using API.Models.DTO.Appointment;
using API.Models.DTO.AppointmentRequests;
using API.Models.DTO.Business;
using API.Models.DTO.Service;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Appointments
            CreateMap<Appointment, AppointmentDto>();
            CreateMap<CreateAppointmentDto, Appointment>();
            // Business
            CreateMap<CreateBusinessDto, Business>();
            
            // Services
            CreateMap<Models.Service, ServiceDto>();
            CreateMap<CreateServiceDto, Models.Service>();
            CreateMap<UpdateServiceDto, Service>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.Business, opt => opt.Ignore())
            .ForMember(dest => dest.ApplicationUserServices, opt => opt.Ignore());

            // Application Requests
            CreateMap<AppointmentRequest, AppointmentRequestDto>();
            CreateMap<CreateAppointmentRequestDto, AppointmentRequest>();
            CreateMap<UpdateAppointmentDto, Appointment>();

            // User
            CreateMap<ApplicationUserService, ApplicationUserServiceDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
        }
    }
}
