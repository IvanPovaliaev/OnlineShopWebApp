using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopWebApp.Tests.Helpers
{
    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumerator(IEnumerator<T> enumerator) => _enumerator = enumerator;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_enumerator.MoveNext());

        public T Current => _enumerator.Current;
    }
}
