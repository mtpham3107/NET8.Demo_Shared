using AutoMapper;
using NET8.Demo.GlobalAdmin.Application.Contracts.Requests;
using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.RabbitMQ.Etos;
using NET8.Demo.Redis.RedisDtos.GlobalAdmin;
using NET8.Demo.Shared;

namespace NET8.Demo.GlobalAdmin;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>));
        CreateMap<User, UserResponse>();
        CreateMap<UserInsertRequest, User>();
        CreateMap<Role, RoleResponse>();
        CreateMap<FileUpload, FileUploadResponse>();
        CreateMap<Province, ProvinceResponse>();
        CreateMap<District, DistrictResponse>();
        CreateMap<Ward, WardResponse>();
        CreateMap<ProvinceRedisDto, ProvinceResponse>();
        CreateMap<DistrictRedisDto, DistrictResponse>();
        CreateMap<WardRedisDto, WardResponse>();
        CreateMap<Address, AddressResponse>();
        CreateMap<AddressInsertRequest, Address>();
        CreateMap<AddressUpdateRequest, Address>();

        //RabitMQ
        CreateMap<NotificationSendEto, Notification>();
    }
}
