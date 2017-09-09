using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SFLibs.Core.IO
{
	public class XmlSerializerUtil
	{
		private static Dictionary<Type, XmlSerializer> dic = new Dictionary<Type, XmlSerializer>();

		public static XmlSerializer GetSerializer(Type type)
		{
			if (!dic.TryGetValue(type, out var s))
			{
				s = new XmlSerializer(type);
				dic.Add(type, s);
			}

			return s;
		}
	}
}
