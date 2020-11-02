using System;
using Xunit;

namespace FlightUtilities.Tests
{
    public class PositionTest
    {
        [Theory (DisplayName = "Position ctor Good Values")]
        [InlineData (-180, -90)]
        [InlineData (180, 90)]
        [InlineData (0, 0)]
        public void PositionTestCtorGoodValues (double longitude, double latitude)
        {
            var pos = new Position (longitude, latitude);
            Assert.Equal (longitude, pos.Longitude);
            Assert.Equal (latitude, pos.Latitude);
        }

        [Theory (DisplayName = "Position ctor ArgumentExceptions")]
        [InlineData ("longitude", -190, 0)]
        [InlineData ("longitude", 190, 0)]
        [InlineData ("latitude", 0, -91)]
        [InlineData ("latitude", 0, 91)]
        public void PositionTestCtorArgumentExceptions (string paramName, double longitude, double latitude)
        {
            Assert.Throws<ArgumentException> (paramName, () => { new Position (longitude, latitude); });
        }
    }
}
