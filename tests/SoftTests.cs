using System;
using Xunit;

namespace Dawn.Tests
{
    public class SoftTests:BaseTests
    {
        [Theory(DisplayName = "Empty correction test")]
        [InlineData("A")]
        public void EmptyCorr(string argument)
        {
           var v=Guard.Argument(argument).Soft(a => a.Empty(), b => "c");
           Assert.Equal("c",v);
            
        }

        [Theory(DisplayName = "Empty OK test")]
        [InlineData("")]
        public void EmptyOk(string argument)
        {
            var v = Guard.Argument(argument).Soft(a => a.Empty(), b => "c");
            Assert.Equal("", v);

        }

        [Theory(DisplayName = "Min OK test")]
        [InlineData(6)]
        public void MinOk(int argument)
        {
            var v = Guard.Argument(argument).Soft(a => a.Min(5), b => 5);
            Assert.Equal(6, v);

        }

        [Theory(DisplayName = "Min correction test")]
        [InlineData(4)]
        public void MinCorr(int argument)
        {
            var v = Guard.Argument(argument).Soft(a => a.Min(5), b => 5);
            Assert.Equal(5, v);

        }

        [Theory(DisplayName = "Min+Max correction test (correct min)")]
        [InlineData(4)]
        public void MinMaxCorrectionWithMin(int argument)
        {
            var v = Guard.Argument(argument).Soft(a => a.Min(5).Max(10), b => b<5?5:10);
            Assert.Equal(5, v);

        }
        [Theory(DisplayName = "Min+Max OK test")]
        [InlineData(8)]
        public void MinMaxCorrectionOk(int argument)
        {
            var v = Guard.Argument(argument).Soft(a => a.Min(5).Max(10), b => b < 5 ? 5 : 10);
            Assert.Equal(8, v);

        }
        [Theory(DisplayName = "Min+Max correction test (correct max)")]
        [InlineData(15)]
        public void MinMaxCorrectionWithMax(int argument)
        {
            var v = Guard.Argument(argument).Soft(a => a.Min(5).Max(10), b => b < 5 ? 5 : 10);
            Assert.Equal(10, v);

        }

        [Theory(DisplayName = "Recheck OK test")]
        [InlineData(4)]
        public void RecheckOk(int argument)
        {
            var v = Guard.Argument(argument).Soft(a => a.Min(5), b => 5,recheck:true);
            Assert.Equal(5, v);

        }

        [Theory(DisplayName = "Recheck fail test")]
        [InlineData(4)]
        public void RecheckFail(int argument)
        {
            Assert.Throws<ArgumentException>(() =>
                Guard.Argument(argument).Soft(a => a.Min(5), b => 4, recheck: true));
        }

        [Theory(DisplayName = "Exception in correction func test")]
        [InlineData(4)]
        public void CorrectionException(int argument)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                Guard.Argument(argument).Soft(
                    a => a.Min(5),
                    a => throw new Exception(),
                    (a)=>"ModifyErr"));
            Assert.StartsWith("ModifyErr", exception.Message);

        }
    }
}
