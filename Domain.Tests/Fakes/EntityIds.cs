namespace Domain.Tests.Fakes;

/// <summary>Sets the database-generated <c>Id</c> of an entity, which only persistence may assign outside tests.</summary>
internal static class EntityIds
{
    public static T WithId<T>(this T entity, int id) where T : class
    {
        typeof(T).GetProperty("Id")!.SetValue(entity, id);
        return entity;
    }
}
