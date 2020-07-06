using System;
using System.Threading.Tasks;
using Xunit;
using AwaitableExercises.Core;

namespace AwaitableExercises.Tests
{
    // UNCOMMENT all asserts when start to implement this exercise!
    // It is by default commented to make compilation possible for other lessons.
    public class BoolAwaiterTests
    {
        [Fact]
        public async Task AwaitFalse()
        {
            //Assert.False(await false);
        }

        [Fact]
        public async Task AwaitTrue()
        {
            //Assert.True(await true);
        }

        [Fact]
        
        public async Task AwaitAwaitFalse()
        {
            //Assert.False(await await false);
        }

        [Fact]
        public async Task AwaitAwaitTrue()
        {
            //Assert.True(await await true);
        }

        [Fact]
        public async Task AwaitOperatorsTrueAndTrue()
        {
            //Assert.True(await true && await true);
        }

        [Fact]
        public async Task AwaitOperatorsTrueAndFalse()
        {
            //Assert.False(await true && await false);
        }

        [Fact]
        public async Task AwaitOperatorsFalseAndTrue()
        {
            //Assert.False(await false && await true);
        }
    }
}
