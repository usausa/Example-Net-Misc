using System;
using Xunit;

namespace Example.Library.Tests
{
    [Api]
    public interface IHogeApi
    {
        int Add(int x, int y);

        [Method("Subtract")]
        int Sub([Parameter("x")] int a, [Parameter("y")] int b);
    }

    public class MetadataFactoryTest
    {
        [Fact]
        public void Test1()
        {
            var md = MetadataFactory.CreateMethodMetadata(typeof(IHogeApi), "Sub",
                new Type[] {typeof(int), typeof(int)});
        }
    }
}
