using AutoMapper;
using NET8.Demo.Shared;

namespace NET8.Demo.TemplateService;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>));
    }
}
