namespace NET8.Demo.Redis.RedisDtos.GlobalAdmin;

public class WardRedisDto
{
    public Guid Id { get; set; }

    public Guid DistrictId { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }
}
