using System.Threading.Tasks;

namespace DaisyFx
{
    public class Signal
    {
        public static readonly Signal Static = new Signal();

        public static implicit operator ValueTask<Signal>(Signal signal)
        {
            return new(signal);
        }
    }
}