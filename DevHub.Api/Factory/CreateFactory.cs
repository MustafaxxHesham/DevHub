namespace DevHub.Factory;

public static class CreateFactory<T> where T : class
{
    public static T GetInstance<T>() where T : new()
    {
        return new T();
    }
}
