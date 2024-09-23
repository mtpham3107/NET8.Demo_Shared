namespace NET8.Demo.Redis.RedisDtos.GlobalAdmin;

public class DistrictRedisDto
{
    public Guid Id { get; set; }

    public Guid ProvinceId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }
}
