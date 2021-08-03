namespace Assets.Scripts.Event.Dialogs
{
    public class Exit : IDialog
    {
        public int ExitValue { get; }
        public Exit(int exitValue = -1) => ExitValue = exitValue;
    }
}