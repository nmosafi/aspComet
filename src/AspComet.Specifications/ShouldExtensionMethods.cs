using System;

using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace AspComet.Specifications
{
    public static class ShouldExtensionMethods
    {
        public static void ShouldHaveHadCalled<T>(this T mock, Action<T> action)
        {
            mock.AssertWasCalled(action);
        }

        public static void ShouldHaveHadCalled<T>(this T mock, Action<T> action, Action<IMethodOptions<object>> setupConstraints)
        {
            mock.AssertWasCalled(action, setupConstraints);
        }
    }
}