namespace Ouroboros.Common.UI.Windows
{
    public interface IWindowCloser
    {
        void CloseWindow<T>() where T : Window;
        void CloseWindow(string id);
        void CloseAllWindows();
    }
}