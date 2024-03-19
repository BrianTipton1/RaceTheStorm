#nullable enable
using Direction = Player.SpeederInput.Controller.Direction;

namespace Player.SpeederInput
{
    public abstract class AbstractSpeederActionResult
    {
        public abstract bool Fired { get; set; }
        public abstract Direction Direction { get; set; }
    }

    public class SpeederActionResult<T> : AbstractSpeederActionResult
    {
        public T Result { get; set; } = default!;
        public override bool Fired { get; set; }
        public override Direction Direction { get; set; }
    }

    public class SpeederActionResult : AbstractSpeederActionResult
    {
        public override bool Fired { get; set; }
        public override Direction Direction { get; set; }
    }
}