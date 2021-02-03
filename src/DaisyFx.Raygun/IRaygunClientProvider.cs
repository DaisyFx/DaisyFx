using Mindscape.Raygun4Net;

namespace DaisyFx.Raygun
{
    public interface IRaygunClientProvider
    {
        RaygunClient GetClient(RaygunSettings settings);
    }
}