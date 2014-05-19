namespace WpfPropertyGrid
{
    using System.Threading.Tasks;

    public interface IRepository
    {
        SomeSetting SaveSomeSettingc();
        Task SaveSomeSettingAsync(SomeSetting setting);
    }
}