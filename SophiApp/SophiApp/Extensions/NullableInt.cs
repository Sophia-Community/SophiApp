namespace SophiApp.Extensions
{
    public static class NullableInt
    {
        public static bool HasNullOrValue(this int? integer, int value) => integer is null || integer == value;
    }
}