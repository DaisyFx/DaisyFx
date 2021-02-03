using Mindscape.Raygun4Net;

namespace DaisyFx.Raygun
{
    public class RaygunClientProvider : IRaygunClientProvider
    {
        public virtual RaygunClient GetClient(RaygunSettings settings)
        {
            return new(settings);
        }
    }
}