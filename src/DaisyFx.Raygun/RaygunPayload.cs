using System;
using System.Collections.Generic;

namespace DaisyFx.Raygun
{
    public class RaygunPayload
    {
        public RaygunPayload(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
        public Dictionary<string, string> UserCustomData { get; } = new();
        public List<string> Tags { get; } = new();
    }
}