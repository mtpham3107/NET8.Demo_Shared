using Microsoft.EntityFrameworkCore;
using NET8.Demo.GlobalAdmin.Core.DbContexts;
using NET8.Demo.GlobalAdmin.Domain.Dtos;
using NET8.Demo.GlobalAdmin.Domain.Entities;
using System.Collections.Concurrent;
using static NET8.Demo.Shared.SharedFunction;

namespace NET8.Demo.GlobalAdmin.Core.SeedDatas;

public static class DataSeeder
{
    public static async ValueTask SeedAdministrativeUnitsAsync(GlobalAdminDbContext context)
    {
        var provinces = new ConcurrentBag<Province>();
        var provincesMap = new ConcurrentDictionary<string, Guid>();
        var districts = new ConcurrentBag<District>();
        var districtsMap = new ConcurrentDictionary<string, Guid>();
        var wards = new ConcurrentBag<Ward>();

        var basePath = Path.Combine(AppContext.BaseDirectory, "SeedDatas");
        var provinceTask = LoadJsonAsync<ProvinceDto>(Path.Combine(basePath, "Provinces.js")).AsTask();
        var districtTask = LoadJsonAsync<DistrictDto>(Path.Combine(basePath, "Districts.js")).AsTask();
        var wardTask = LoadJsonAsync<WardDto>(Path.Combine(basePath, "Wards.js")).AsTask();

        await Task.WhenAll(provinceTask, districtTask, wardTask);

        var provincesDto = await provinceTask;
        var districtsDto = await districtTask;
        var wardsDto = await wardTask;

        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == "admin");

        var tasks = provincesDto.Select(x => Task.Run(() =>
        {
            var province = new Province()
            {
                Id = Guid.NewGuid(),
                Code = x.Code,
                Name = x.Name,
                CreatedBy = user?.Id,
                ModifiedBy = user?.Id,
            };

            provincesMap[province.Code] = province.Id;
            provinces.Add(province);
        }));

        await Task.WhenAll(tasks);

        tasks = districtsDto.Select(x => Task.Run(() =>
        {
            var district = new District()
            {
                Id = Guid.NewGuid(),
                ProvinceId = provincesMap[x.ProvinceCode],
                Code = x.Code,
                Name = x.Name,
                CreatedBy = user?.Id,
                ModifiedBy = user?.Id,
            };

            districtsMap[district.Code] = district.Id;
            districts.Add(district);
        }));

        await Task.WhenAll(tasks);

        tasks = wardsDto.Select(x => Task.Run(() =>
        {
            var ward = new Ward()
            {
                Id = Guid.NewGuid(),
                DistrictId = districtsMap[x.DistrictCode],
                Code = x.Code,
                Name = x.Name,
                CreatedBy = user?.Id,
                ModifiedBy = user?.Id,
            };

            wards.Add(ward);
        }));

        await Task.WhenAll(tasks);

        context.Provinces.AddRange(provinces);
        context.Districts.AddRange(districts);
        context.Wards.AddRange(wards);
        await context.SaveChangesAsync();
    }
}
