using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;

namespace NET8.Demo.GlobalAdmin.Application.IServices;

public interface ILocationService
{
    ValueTask<IEnumerable<ProvinceResponse>> GetProvincesAsync();

    ValueTask<IEnumerable<DistrictResponse>> GetDistrictsAsync(Guid provinceId);

    ValueTask<IEnumerable<WardResponse>> GetWardsAsync(Guid districtId);

    ValueTask<AddressResponse> GetAddressAsync(Guid addressId);

    ValueTask<IEnumerable<AddressResponse>> GetAddressesAsync(ICollection<Guid> addressIds);
}
