using System.Linq.Expressions;
using Moq;

namespace UnitTestHelper.Extensions;

public static class MockExtension
{
    /// <summary>
    /// Setups <paramref name="expression"/> on <paramref name="mock"/> to return the mocked
    /// object instance of the returned mock.
    /// </summary>
    /// <param name="mock"></param>
    /// <param name="expression"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns>The mock which holds the object which is returned by the expression.</returns>
    public static Mock<TResult> SetupReturnMock<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression)
        where T : class
        where TResult : class
    {
        var resultMock = new Mock<TResult>();
        mock
            .Setup(expression)
            .Returns(resultMock.Object);
        return resultMock;
    }
}