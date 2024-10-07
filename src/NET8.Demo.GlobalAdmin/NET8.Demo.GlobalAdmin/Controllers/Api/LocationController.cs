//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using NET8.Demo.GlobalAdmin.Application.Contracts.IServices;
//using NET8.Demo.GlobalAdmin.Application.Contracts.Requests;
//using NET8.Demo.GlobalAdmin.Application.Contracts.Responses;
//using NET8.Demo.GlobalAdmin.Application.IServices;
//using NET8.Demo.GlobalAdmin.Domain.Entities;

//namespace NET8.Demo.GlobalAdmin.Controllers.Api;

//[Route("api/locations")]
//public class LocationController : ApiControllerBase
//{
//    private readonly ILocationService _service;
//    private readonly IService<Address> _addressServices;
//    private readonly IMapper _mapper;

//    public LocationController(ILocationService service, IService<Address> addressServices, IMapper mapper)
//    {
//        _service = service;
//        _addressServices = addressServices;
//        _mapper = mapper;
//    }

//    [HttpGet("get-provinces")]
//    public async ValueTask<ActionResult<IEnumerable<ProvinceResponse>>> GetProvinces() => Ok(await _service.GetProvincesAsync());

//    [HttpGet("get-districts/{provinceId}")]
//    public async ValueTask<ActionResult<IEnumerable<DistrictResponse>>> GetDistricts(Guid provinceId) => Ok(await _service.GetDistrictsAsync(provinceId));

//    [HttpGet("get-wards/{districtId}")]
//    public async ValueTask<ActionResult<IEnumerable<WardResponse>>> GetWards(Guid districtId) => Ok(await _service.GetWardsAsync(districtId));

//    [HttpGet("get-address/{addressId}")]
//    public async ValueTask<ActionResult<AddressResponse>> GetAddress(Guid addressId) => Ok(await _service.GetAddressAsync(addressId));

//    [HttpPost("get-addresses")]
//    public async ValueTask<ActionResult<IEnumerable<AddressResponse>>> GetAddresses([FromBody] ICollection<Guid> addressIds) => Ok(await _service.GetAddressesAsync(addressIds));

//    [HttpPut("address")]
//    public async ValueTask<ActionResult<Address>> InsertAddresses(AddressInsertRequest request) => Ok(await _addressServices.InsertAsync(_mapper.Map<Address>(request)));

//    [HttpPost("address")]
//    public async ValueTask<ActionResult<Address>> UpdateAddresses(AddressUpdateRequest request)
//        => Ok(await _addressServices.ModifyAsync(_mapper.Map<Address>(request),
//            x => x.ProvinceId,
//            x => x.DistrictId,
//            x => x.WardId,
//            x => x.AddressLine,
//            x => x.Latitude,
//            x => x.Longitude,
//            x => x.IsActive));

//    [HttpDelete("address/{addressId}")]
//    public async ValueTask<ActionResult<bool>> DeleteAddresses(Guid addressId) => Ok(await _addressServices.DeleteAsync(addressId));
//}
