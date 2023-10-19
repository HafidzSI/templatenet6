using System.Collections.Generic;
using NetCa.Domain.Common;
using NUnit.Framework;

namespace NetCa.Domain.UnitTests.Common
{
    public class ValueObjectTests
    {
        [Test]
        public void Equals_GivenDifferentValues_ShouldReturnFalse()
        {
            var point1 = new Point(1, 2);
            var point2 = new Point(2, 1);

            Assert.False(point1.Equals(point2));
        }

        [Test]
        public void Equals_GivenMatchingValues_ShouldReturnTrue()
        {
            var point1 = new Point(1, 2);
            var point2 = new Point(1, 2);

            Assert.True(point1.Equals(point2));
        }

        private class Point : ValueObject
        {
            private int X { get; set; }

            private int Y { get; set; }

            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            protected override IEnumerable<object> GetEqualityComponents()
            {
                yield return X.ToString();
                yield return Y.ToString();
            }
        }
    }
}