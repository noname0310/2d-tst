namespace Assets.Scripts.Event.Dialogs
{
    public class Label : IDialog
    {
        public string LabelName { get; }
        public Label(string labelName) => LabelName = labelName;
    }
}
