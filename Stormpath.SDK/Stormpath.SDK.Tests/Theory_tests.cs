using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Stormpath.SDK.Tests
{
    public class Theory_tests
    {
        [Theory]
        [InlineData("test", 4)]
        [InlineData("abcde", 5)]
        [InlineData("ab", 2)]
        public void String_length_theory(string test, int length)
        {
            var result = test.Length;

            result.ShouldBe(length);
        }
    }
}
