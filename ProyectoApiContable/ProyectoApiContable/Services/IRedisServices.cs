namespace ProyectoApiContable.Services
{
    public interface IRedisServices
    {
        Task AgregarLogARedis(string logMessage);
    }
}
