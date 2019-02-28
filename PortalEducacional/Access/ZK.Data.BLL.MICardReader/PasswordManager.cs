using System;
using System.Text;

namespace ZK.Data.BLL.MICardReader
{
	public class PasswordManager
	{
		private static byte[] XorKey = new byte[12]
		{
			242,
			109,
			132,
			71,
			29,
			179,
			213,
			60,
			75,
			141,
			79,
			28
		};

		public static string Encode(byte[] Key)
		{
			byte[] array = new byte[12];
			if (Key.Length < 6)
			{
				throw new ArgumentException("密钥长度不能小于6");
			}
			for (int i = 0; i < 6; i++)
			{
				array[i * 2] = (byte)((Key[i] & 0xF0) >> 4);
				array[i * 2 + 1] = (byte)(Key[i] & 0xF);
			}
			for (int j = 0; j < 12; j++)
			{
				if (array[j] == 0)
				{
					array[j] = 0;
				}
				if (array[j] == 1)
				{
					array[j] = 17;
				}
				if (array[j] == 2)
				{
					array[j] = 34;
				}
				if (array[j] == 3)
				{
					array[j] = 51;
				}
				if (array[j] == 4)
				{
					array[j] = 68;
				}
				if (array[j] == 5)
				{
					array[j] = 85;
				}
				if (array[j] == 6)
				{
					array[j] = 102;
				}
				if (array[j] == 7)
				{
					array[j] = 119;
				}
				if (array[j] == 8)
				{
					array[j] = 136;
				}
				if (array[j] == 9)
				{
					array[j] = 153;
				}
				if (array[j] == 10)
				{
					array[j] = 170;
				}
				if (array[j] == 11)
				{
					array[j] = 187;
				}
				if (array[j] == 12)
				{
					array[j] = 204;
				}
				if (array[j] == 13)
				{
					array[j] = 221;
				}
				if (array[j] == 14)
				{
					array[j] = 238;
				}
				if (array[j] == 15)
				{
					array[j] = 240;
				}
			}
			for (int k = 0; k < 12; k++)
			{
				array[k] = (byte)(array[k] + 5 ^ PasswordManager.XorKey[k]);
			}
			return CardReader.Bytes2HexString(array);
		}

		public static string Encode(string pwd)
		{
			if (string.IsNullOrEmpty(pwd))
			{
				return PasswordManager.Encode(CardReader.HexString2Bytes("FF FF FF FF FF FF"));
			}
			return PasswordManager.Encode(Encoding.ASCII.GetBytes(pwd));
		}

		public static string Decode(string CipherText)
		{
			byte[] array = new byte[6];
			byte[] array2 = CardReader.HexString2Bytes(CipherText);
			if (array2.Length < 12)
			{
				throw new ArgumentException("密文长度不能小于12字节");
			}
			for (int i = 0; i < 12; i++)
			{
				array2[i] = (byte)((array2[i] ^ PasswordManager.XorKey[i]) - 5);
			}
			for (int i = 0; i < 12; i++)
			{
				if (array2[i] == 0)
				{
					array2[i] = 0;
				}
				if (array2[i] == 17)
				{
					array2[i] = 1;
				}
				if (array2[i] == 34)
				{
					array2[i] = 2;
				}
				if (array2[i] == 51)
				{
					array2[i] = 3;
				}
				if (array2[i] == 68)
				{
					array2[i] = 4;
				}
				if (array2[i] == 85)
				{
					array2[i] = 5;
				}
				if (array2[i] == 102)
				{
					array2[i] = 6;
				}
				if (array2[i] == 119)
				{
					array2[i] = 7;
				}
				if (array2[i] == 136)
				{
					array2[i] = 8;
				}
				if (array2[i] == 153)
				{
					array2[i] = 9;
				}
				if (array2[i] == 170)
				{
					array2[i] = 10;
				}
				if (array2[i] == 187)
				{
					array2[i] = 11;
				}
				if (array2[i] == 204)
				{
					array2[i] = 12;
				}
				if (array2[i] == 221)
				{
					array2[i] = 13;
				}
				if (array2[i] == 238)
				{
					array2[i] = 14;
				}
				if (array2[i] == 240)
				{
					array2[i] = 15;
				}
			}
			for (int i = 0; i < 6; i++)
			{
				array[i] = (byte)((array2[i * 2] & 0xF) << 4 | (array2[i * 2 + 1] & 0xF));
			}
			return Encoding.ASCII.GetString(array);
		}
	}
}
