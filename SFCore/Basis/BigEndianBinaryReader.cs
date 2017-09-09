using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Basis
{
	public class BigEndianBinaryReader : BinaryReader
	{
		public BigEndianBinaryReader( Stream s )
			: base( s )
		{
		}

		public BigEndianBinaryReader( Stream s, Encoding encoding )
			: base( s, encoding )
		{
		}

		public BigEndianBinaryReader( Stream s, Encoding encoding, bool leaveOpen  )
			: base( s, encoding, leaveOpen )
		{
		}

		public override decimal ReadDecimal()
		{
			var buf = base.ReadBytes( 16 );
			int[] bits = new int[4];
			bits[0] = ( (int)buf[0] ) | ( (int)buf[1] << 8 ) | ( (int)buf[2] << 16 ) | ( (int)buf[3] << 24 );
			bits[1] = ( (int)buf[4] ) | ( (int)buf[5] << 8 ) | ( (int)buf[6] << 16 ) | ( (int)buf[7] << 24 );
			bits[2] = ( (int)buf[8] ) | ( (int)buf[9] << 8 ) | ( (int)buf[10] << 16 ) | ( (int)buf[11] << 24 );
			bits[3] = ( (int)buf[12] ) | ( (int)buf[13] << 8 ) | ( (int)buf[14] << 16 ) | ( (int)buf[15] << 24 );
			return new Decimal( bits );
		}

		public override double ReadDouble()
		{
			var buf = base.ReadBytes( 8 );
			Array.Reverse( buf );
			return BitConverter.ToDouble( buf, 0 );
		}

		public override short ReadInt16()
		{
			var buf = base.ReadBytes( 2 );
			Array.Reverse( buf );
			return BitConverter.ToInt16( buf, 0 );
		}

		public override int ReadInt32()
		{
			var buf = base.ReadBytes( 4 );
			Array.Reverse( buf );
			return BitConverter.ToInt32( buf, 0 );
		}

		public override long ReadInt64()
		{
			var buf = base.ReadBytes( 8 );
			Array.Reverse( buf );
			return BitConverter.ToInt64( buf, 0 );
		}

		public override float ReadSingle()
		{
			var buf = base.ReadBytes( 4 );
			Array.Reverse( buf );
			return BitConverter.ToSingle( buf, 0 );
		}

		public override ushort ReadUInt16()
		{
			var buf = base.ReadBytes( 2 );
			Array.Reverse( buf );
			return BitConverter.ToUInt16( buf, 0 );
		}

		public override uint ReadUInt32()
		{
			var buf = base.ReadBytes( 4 );
			Array.Reverse( buf );
			return BitConverter.ToUInt32( buf, 0 );
		}

		public override ulong ReadUInt64()
		{
			var buf = base.ReadBytes( 8 );
			Array.Reverse( buf );
			return BitConverter.ToUInt64( buf, 0 );
		}

	}
}
