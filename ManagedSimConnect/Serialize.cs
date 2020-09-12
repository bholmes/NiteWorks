using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SimConnect
{
	internal class Serialize
	{
		public static byte [] ToByteArray<TT> (TT obj)
		{
			var bf = new BinaryFormatter ();
			using (var ms = new MemoryStream ())
			{
				bf.Serialize (ms, obj);
				return ms.ToArray ();
			}
		}

		static public TT FromByteArray<TT> (byte [] arrBytes)
		{
			MemoryStream memStream = new MemoryStream ();
			BinaryFormatter binForm = new BinaryFormatter ();
			memStream.Write (arrBytes, 0, arrBytes.Length);
			memStream.Seek (0, SeekOrigin.Begin);
			return (TT)binForm.Deserialize (memStream);
		}
	}
}
