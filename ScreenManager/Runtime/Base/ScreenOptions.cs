namespace Endelways.ScreenManager
{
    public class ScreenOptions<T> : IScreenOptions
    {
        public object Value { get; }

        public ScreenOptions(T value)
        {
            Value = value;
        }
    }
}