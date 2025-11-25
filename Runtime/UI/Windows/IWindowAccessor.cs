namespace Ouroboros.Common.UI.Windows
{
    public interface IWindowAccessor
    {
        T GetOpenedWindow<T>() where T : Window;
        Window GetOpenedWindow(string id);
        bool IsWindowOpen(string id);
        bool IsWindowOpen<T>() where T : Window;
    }
}