namespace Player.SpeederInput
{
    public abstract class AbstractMovement
    {
        public Controller.Direction Direction { get; set; }
        public Controller.Modifier Modifier { get; set; }
    }

    public class Movement : AbstractMovement
    {
        public SpeederAction Action { get; set; }
    }

    public class Movement<T> : AbstractMovement
    {
        public SpeederAction<T> Action { get; set; }
    }
}