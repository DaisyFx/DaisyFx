using System;
using System.Diagnostics.CodeAnalysis;

namespace DaisyFx
{
    public interface IReadOnlyChainContext
    {
        Guid Id { get; }
        string ChainName { get; }
        bool TryGet<T>(object key, [MaybeNullWhen(false)] out T value);
    }
}