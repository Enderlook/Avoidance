namespace AvalonStudios.Additions.Utils.Interfaces
{
    public interface IInitialize<in T>
    {
        void Initialize(T init);
    }
}
