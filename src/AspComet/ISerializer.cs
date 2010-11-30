namespace AspComet
{
    public interface ISerializer
    {
        string Serialize(object obj);
        T Deserialize<T>(string json);
    }
}