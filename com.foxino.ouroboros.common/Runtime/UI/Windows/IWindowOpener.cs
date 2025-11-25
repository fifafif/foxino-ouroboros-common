namespace Ouroboros.Common.UI.Windows
{
    public interface IWindowOpener
    {
        T OpenWindow<T>(WindowConfig config = null) where T : Window;
        Window OpenWindow(string id, WindowConfig config = null);
    }
}
