namespace NET8.Demo.Redis;

public readonly struct RedisConstant
{
    public const string KeyCommand = "KEYS";

    public const string GlobalAdmin_Prefix = "globalAdmin";
    public static readonly string GlobalAdmin_Module_Prefix = $"{GlobalAdmin_Prefix}:module";
    public static readonly string GlobalAdmin_Province_Prefix = $"{GlobalAdmin_Prefix}:province";
    public static readonly string GlobalAdmin_District_Prefix = $"{GlobalAdmin_Prefix}:district";
    public static readonly string GlobalAdmin_Ward_Prefix = $"{GlobalAdmin_Prefix}:ward";

    public const string Prodventory_Prefix = "prodventory";
    public static readonly string Prodventory_Category_Prefix = $"{Prodventory_Prefix}:category";
    public static readonly string Prodventory_SubCategory_Prefix = $"{Prodventory_Prefix}:subcategory";
    public static readonly string Prodventory_CategoryTag_Prefix = $"{Prodventory_Prefix}:categorytag";

}
