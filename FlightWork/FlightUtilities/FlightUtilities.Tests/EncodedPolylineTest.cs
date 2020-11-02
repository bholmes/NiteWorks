using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FlightUtilities.Tests
{
    public class EncodedPolylineTest
    {
        [Fact (DisplayName = "Encode Empty Positions")]
        public void EncodeEmptyPositions ()
        {
            var encodedStr = EncodedPolyline.Encode (new List<Position> ());
            Assert.True (string.IsNullOrEmpty (encodedStr));
        }

        [Theory (DisplayName = "Encode Positions")]
        [InlineData (new[] { 0.0, 0.0 }, "??")]
        [InlineData (new[] { 1.0, 0.0 }, "_ibE?")]
        [InlineData (new[] { 1.0, 0.0, 1.0, 1.0 }, "_ibE??_ibE")]
        [InlineData (new double [] { 0, 179, 0, -179 }, "?_}oca@?~z`hcA")]
        public void EncodePositions (double[] inValues, string expected)
        {
            var lonVals = inValues.Where ((val, index) => (index % 2) == 0);
            var latVals = inValues.Where ((val, index) => (index % 2) == 1);

            var inList = latVals.Zip (lonVals).Select ((pos) => new Position (pos.First, pos.Second)).ToList ();

            var encodedStr = EncodedPolyline.Encode (new List<Position> (inList));
            Assert.Equal (expected, encodedStr);
        }

        [Fact (DisplayName = "Decode Empty String")]
        public void DecodeEmptyString ()
        {
            var positions = EncodedPolyline.Decode ("");
            Assert.Empty (positions);
        }

        [Theory (DisplayName = "Decode Positions")]
        [InlineData ("??", new[] { 0.0, 0.0 })]
        [InlineData ("_ibE?", new[] { 1.0, 0.0 })]
        [InlineData ("_ibE??_ibE", new[] { 1.0, 0.0, 1.0, 1.0 })]
        [InlineData ("?_}oca@?~z`hcA", new double [] { 0, 179, 0, -179 })]
        public void DecodePositions (string encoded, double[] expectedValues)
        {
            var lonVals = expectedValues.Where ((val, index) => (index % 2) == 0);
            var latVals = expectedValues.Where ((val, index) => (index % 2) == 1);

            var expectedList = latVals.Zip (lonVals).Select ((pos) => new Position (pos.First, pos.Second)).ToList ();

            var retList = EncodedPolyline.Decode (encoded);
            Assert.Equal (expectedList, retList);
        }
    }
}
