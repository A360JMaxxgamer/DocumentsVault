using Grpc.Core;

namespace UnitTestHelper.Mocks
{
    public class AsyncStreamReaderMock<T> : IAsyncStreamReader<T>
    {
        private readonly IEnumerator<T> _valuesEnumerator;

        /// <inheritdoc />
        public T Current => _valuesEnumerator.Current;

        public AsyncStreamReaderMock(IEnumerable<T> values)
        {
            _valuesEnumerator = values.GetEnumerator();
        }

        /// <inheritdoc />
        public Task<bool> MoveNext(CancellationToken cancellationToken) =>
            Task.FromResult(_valuesEnumerator.MoveNext());
    }
}