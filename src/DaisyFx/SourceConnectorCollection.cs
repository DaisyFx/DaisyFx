using System;
using System.Collections;
using System.Collections.Generic;
using DaisyFx.Sources;

namespace DaisyFx
{
    public class SourceConnectorCollection<T> : IReadOnlyList<ISourceConnector<T>>
    {
        private readonly string _chainName;
        private readonly IServiceProvider _applicationServices;
        private readonly List<ISourceConnector<T>> _sourceConnectors = new();

        public SourceConnectorCollection(string chainName, IServiceProvider applicationServices)
        {
            _chainName = chainName;
            _applicationServices = applicationServices;
        }

        public IEnumerator<ISourceConnector<T>> GetEnumerator() => _sourceConnectors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _sourceConnectors.GetEnumerator();
        public int Count => _sourceConnectors.Count;
        public ISourceConnector<T> this[int index] => _sourceConnectors[index];

        public void Add<TSource>(string name) where TSource : ISource<T>
        {
            var index = _sourceConnectors.Count;
            ISourceConnector<T> sourceConnector =
                new SourceConnector<TSource, T>(_chainName, name, index, _applicationServices);
            _sourceConnectors.Add(sourceConnector);
        }

        public void Add<TSource, TSourceOutput>(string name, Func<TSourceOutput, T> mapper)
            where TSource : ISource<TSourceOutput>
        {
            var index = _sourceConnectors.Count;
            ISourceConnector<TSourceOutput> sourceConnector =
                new SourceConnector<TSource, TSourceOutput>(_chainName, name, index, _applicationServices);
            ISourceConnector<T> connector = new SourceMapperConnector<TSourceOutput, T>(sourceConnector, mapper);
            _sourceConnectors.Add(connector);
        }
    }
}