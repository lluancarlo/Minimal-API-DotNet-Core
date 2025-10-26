namespace Minimal.Domain;

public static class EnumUser
{
    public enum Type
    {
        A = 0,
        B = 1,
        C = 2,
    }

    public static T GetEnumByName<T>(string name) => (T)Enum.Parse(typeof(T), name.Trim());
}
