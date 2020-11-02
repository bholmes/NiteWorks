using System;
using System.Collections.Generic;
using System.Text;

namespace FlightUtilities
{
    public class EncodedPolyline
    {
        private EncodedPolyline () { }

        private class Encoder
        {
            public Encoder () { }

            StringBuilder sb = new StringBuilder ();

            void AppendFloat (double fVal)
            {
                var iVal = (int)Math.Round (fVal * 100000);
                if (iVal == 0)
                {
                    sb.Append ('?');
                    return;
                }

                iVal = iVal << 1;
                if (fVal < 0)
                    iVal = iVal ^ -1;

                while (iVal != 0)
                {
                    var nextVal = iVal >> 5;
                    var orVal = nextVal == 0 ? 0 : 0x20;
                    var c = (char)('?' + ((iVal & 0b11111) | orVal));
                    sb.Append (c);

                    iVal = nextVal;
                }
            }

            public void AppendPosition (double lat, double lon)
            {
                AppendFloat (lat);
                AppendFloat (lon);
            }

            public string Finish ()
            {
                return sb.ToString ();
            }
        }

        private class Decoder
        {
            string encodedString;

            public Decoder (string encodedString)
            {
                this.encodedString = encodedString;
            }

            public List<Position> Decode ()
            {
                var retList = new List<Position> ();

                if (string.IsNullOrEmpty (encodedString))
                    return retList;

                var cEnum = encodedString.GetEnumerator ();

                var lastLat = 0.0;
                var lastLon = 0.0;

                while (cEnum.MoveNext ())
                {
                    var lat = PopNumber (cEnum);
                    cEnum.MoveNext ();
                    var lon = PopNumber (cEnum);
                    retList.Add (new Position (lon + lastLon, lat + lastLat));

                    lastLat = lat;
                    lastLon = lon;
                }

                return retList;
            }

            double PopNumber (CharEnumerator cEnum)
            {
                int ret = 0;
                var shift = 0;

                do
                {
                    var cur = (int)cEnum.Current;
                    cur -= '?';
                    ret = ret | ((cur & 0b11111) << shift);
                    shift += 5;
                    if ((cur & 0x20) == 0)
                        break;
                } while (cEnum.MoveNext ());

                if ((ret & 0x1) == 0x1)
                {
                    ret = ret ^ -1;
                }

                ret = ret >> 1;

                return ((double)ret) / 100000.0;
            }
        }

        public static string Encode (List<Position> positions)
        {
            if (positions == null || positions.Count == 0)
                return string.Empty;

            var worker = new EncodedPolyline.Encoder ();

            double lastLat = 0;
            double lastLon = 0;

            foreach (var p in positions)
            {
                var curLat = p.Latitude;
                var curLon = p.Longitude;
                worker.AppendPosition (curLat - lastLat, curLon - lastLon);
                lastLat = curLat;
                lastLon = curLon;
            }

            return worker.Finish ();
        }

        public static List<Position> Decode (string encodedString)
        {
            return new Decoder (encodedString).Decode ();
        }

    }
}
