using Xunit;

namespace Stormpath.SDK.Tests
{
    public class XunitTester
    {
        [Fact]
        public void AssertBool()
        {
            Assert.True(true);
        }

        [Fact]
        public void AssertString()
        {
            Assert.NotEqual("foo", "bar");
        }
    }
}