using System.Collections.Generic;
using FluentAssertions;
using Stormpath.SDK.Impl.Resource;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class DefaultCreationOptionsShould
    {
        [Fact]
        public void ReturnNullForNoParameters()
        {
            var options = new DefaultCreationOptions(null);

            options.GetQueryString().Should().BeNullOrEmpty();
        }

        [Fact]
        public void ReturnSingleParameter()
        {
            var options = new DefaultCreationOptions(new Dictionary<string, string>
            {
                ["challenge"] = "true"
            });

            options.GetQueryString().Should().Be("challenge=true");
        }

        [Fact]
        public void ReturnMultipleParameters()
        {
            var options = new DefaultCreationOptions(new Dictionary<string, string>
            {
                ["foo"] = "bar",
                ["baz"] = "123"
            });

            options.GetQueryString().Should().Be("foo=bar&baz=123");
        }
    }
}
