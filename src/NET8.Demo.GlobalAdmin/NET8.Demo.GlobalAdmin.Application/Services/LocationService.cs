using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;
using NET8.Demo.GlobalAdmin.Application.IServices;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using NET8.Demo.GlobalAdmin.Domain.IUnitOfWorks;
using NET8.Demo.Redis;
using NET8.Demo.Redis.RedisDtos.GlobalAdmin;
using NET8.Demo.Redis.Services;
using static Newtonsoft.Json.JsonConvert;

namespace NET8.Demo.GlobalAdmin.Application.Services;

public class LocationService : ServiceBase, ILocationService
{
    private readonly IRedisService<ProvinceRedisDto> _provinceRedisService;
    private readonly IRedisService<DistrictRedisDto> _districtRedisService;
    private readonly IRedisService<WardRedisDto> _wardRedisService;
    private readonly IMapper _mapper;

    public LocationService(
        IRedisService<ProvinceRedisDto> provinceRedisService,
        IRedisService<DistrictRedisDto> districtRedisService,
        IRedisService<WardRedisDto> wardRedisService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ILogger<LocationService> logger,
        IStringLocalizer<LocationService> localizer) : base(unitOfWork, httpContextAccessor, logger, localizer)
    {
        _provinceRedisService = provinceRedisService;
        _districtRedisService = districtRedisService;
        _wardRedisService = wardRedisService;
        _mapper = mapper;
    }
    public async ValueTask<IEnumerable<ProvinceResponse>> GetProvincesAsync()
    {
        try
        {
            var provinces = await _provinceRedisService.GetListAsync(RedisConstant.GlobalAdmin_Province_Prefix);
            return _mapper.Map<IEnumerable<ProvinceResponse>>(provinces.Values.Where(x => x.IsActive));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "LocationService-GetProvinces-Exception");
            throw;
        }
    }

    public async ValueTask<IEnumerable<DistrictResponse>> GetDistrictsAsync(Guid provinceId)
    {
        try
        {
            var districts = await _districtRedisService.GetListAsync(RedisConstant.GlobalAdmin_District_Prefix);
            return _mapper.Map<IEnumerable<DistrictResponse>>(districts.Values.Where(x => x.IsActive));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "LocationService-GetDistricts-Exception: {provinceId}", provinceId);
            throw;
        }
    }

    public async ValueTask<IEnumerable<WardResponse>> GetWardsAsync(Guid districtId)
    {
        try
        {
            var wards = await _wardRedisService.GetListAsync(RedisConstant.GlobalAdmin_Ward_Prefix);
            return _mapper.Map<IEnumerable<WardResponse>>(wards.Values.Where(x => x.IsActive));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "LocationService-GetWards-Exception: {districtId}", districtId);
            throw;
        }
    }

    public async ValueTask<AddressResponse> GetAddressAsync(Guid addressId)
    {
        try
        {
            var address = await UnitOfWork.Repository<Address>().GetByIdAsync(addressId);

            var province = await _provinceRedisService.GetAsync(RedisConstant.GlobalAdmin_Province_Prefix, address.ProvinceId.ToString());
            var district = await _districtRedisService.GetAsync(RedisConstant.GlobalAdmin_District_Prefix, address.DistrictId.ToString());
            var ward = await _wardRedisService.GetAsync(RedisConstant.GlobalAdmin_Ward_Prefix, address.WardId.ToString());

            var result = _mapper.Map<AddressResponse>(address);
            result.ProvinceName = province?.Name;
            result.DistrictName = district?.Name;
            result.WardName = ward?.Name;

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "LocationService-GetAddress-Exception: {addressId}", addressId);
            throw;
        }
    }

    public async ValueTask<IEnumerable<AddressResponse>> GetAddressesAsync(ICollection<Guid> addressIds)
    {
        try
        {
            var addresses = await UnitOfWork.Repository<Address>().GetListAsync(x => addressIds.Contains(x.Id));

            var provinces = await _provinceRedisService.GetListAsync(RedisConstant.GlobalAdmin_Province_Prefix);
            var districts = await _districtRedisService.GetListAsync(RedisConstant.GlobalAdmin_District_Prefix);
            var wards = await _wardRedisService.GetListAsync(RedisConstant.GlobalAdmin_Ward_Prefix);

            var result = _mapper.Map<IEnumerable<AddressResponse>>(addresses)
                .Select(address =>
                {
                    address.ProvinceName = provinces.TryGetValue(address.ProvinceId.ToString(), out var province) ? province.Name : null;
                    address.DistrictName = districts.TryGetValue(address.DistrictId.ToString(), out var district) ? district.Name : null;
                    address.WardName = wards.TryGetValue(address.WardId.ToString(), out var ward) ? ward.Name : null;

                    return address;
                });

            return result;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "LocationService-GetAddresses-Exception: {addressIds}", SerializeObject(addressIds));
            throw;
        }
    }
}
