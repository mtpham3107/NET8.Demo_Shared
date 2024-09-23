using AutoMapper;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.GlobalAdmin.Domain.IRepositories;
using NET8.Demo.Redis.RedisDtos.GlobalAdmin;

namespace NET8.Demo.Redis;

public class GlobalAdminMapper : Profile
{
    public GlobalAdminMapper()
    {
        CreateMap<Module, ModuleRedisDto>();
        CreateMap<Province, ProvinceRedisDto>();
        CreateMap<District, DistrictRedisDto>();
        CreateMap<Ward, WardRedisDto>();
    }

    public static Dictionary<Type, ServiceMapping> Mappings => new()
    {
        {typeof(ModuleRedisDto), new ServiceMapping(typeof(Module), typeof(IRepository<Module>))},
        {typeof(ProvinceRedisDto), new ServiceMapping(typeof(Province), typeof(IRepository<Province>))},
        {typeof(DistrictRedisDto), new ServiceMapping(typeof(District), typeof(IRepository<District>))},
        {typeof(WardRedisDto), new ServiceMapping(typeof(Ward), typeof(IRepository<Ward>))},
    };
}
