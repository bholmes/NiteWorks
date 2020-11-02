using System;
//
namespace FlightUtilities
{
    public class Position
    {
        public Position (double longitude, double latitude)
        {
            if (longitude < -180 || longitude > 180)
                throw new ArgumentException ($"Longitude must be between -180 and 180", nameof (longitude));

            if (latitude < -90 || latitude > 90)
                throw new ArgumentException ($"Latitude must be between -90 and 90", nameof (latitude));

            Longitude = longitude;
            Latitude = latitude;
        }

        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is Position))
                return base.Equals (obj);

            var otherPos = obj as Position;

            return otherPos.Latitude == Latitude && otherPos.Longitude == Longitude;
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Longitude.GetHashCode ();
                hash = hash * 23 + Latitude.GetHashCode ();
                return hash;
            }
        }
    }
}
