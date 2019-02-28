using System;
using System.Text;

namespace ZK.Data.BLL.MICardReader
{
	public class CardReader
	{
		public static string Bytes2HexString(byte[] bytArray, int startIndex, int length)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (bytArray != null)
			{
				for (int i = 0; i < length; i++)
				{
					stringBuilder.Append(bytArray[startIndex + i].ToString("X2"));
				}
			}
			return stringBuilder.ToString();
		}

		public static string Bytes2HexString(byte[] bytArray)
		{
			return CardReader.Bytes2HexString(bytArray, 0, bytArray.Length);
		}

		public static byte[] HexString2Bytes(string HexStr)
		{
			HexStr = HexStr.Replace(" ", "");
			if (HexStr.Length % 2 != 0)
			{
				HexStr = HexStr.Insert(HexStr.Length - 1, "0");
			}
			byte[] array = new byte[HexStr.Length / 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Convert.ToByte(HexStr.Substring(i * 2, 2), 16);
			}
			return array;
		}

		public static byte[] ReverseBytes(byte[] data)
		{
			if (data == null)
			{
				return null;
			}
			byte[] array = new byte[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				array[array.Length - 1 - i] = data[i];
			}
			return array;
		}

		public static void CaclSectorCount(int StartSectorId, int DataLength, out int LowSecCount, out int HeightSecCount)
		{
			LowSecCount = (HeightSecCount = 0);
			if (StartSectorId >= 0 && DataLength >= 0)
			{
				int num = (int)Math.Ceiling((double)DataLength / 16.0);
				if (StartSectorId < 32)
				{
					if (StartSectorId * 4 + num < 128)
					{
						LowSecCount = (int)Math.Ceiling((double)((float)num / 3f));
						HeightSecCount = 0;
					}
					else
					{
						LowSecCount = 32 - StartSectorId;
						HeightSecCount = (int)Math.Ceiling((double)((float)(num - LowSecCount * 3) / 15f));
					}
				}
				else
				{
					LowSecCount = 0;
					HeightSecCount = (int)Math.Ceiling((double)((float)num / 15f));
				}
			}
		}

		public static void CaclBlockCount(int StartSectorId, int DataLength, out int LowBlockCount, out int HeightBlockCount)
		{
			LowBlockCount = (HeightBlockCount = 0);
			if (StartSectorId >= 0 && DataLength >= 0)
			{
				int num = (int)Math.Ceiling((double)DataLength / 16.0);
				if (StartSectorId < 32)
				{
					int num2 = 128 - StartSectorId * 4 - (32 - StartSectorId);
					if (num > num2)
					{
						LowBlockCount = num2;
						HeightBlockCount = num - LowBlockCount;
					}
					else
					{
						LowBlockCount = num;
					}
				}
				else
				{
					HeightBlockCount = num;
				}
			}
		}

		public static bool CheckOpMutilBlockLength(int BlockCount)
		{
			switch (BlockCount)
			{
			case 1:
			case 2:
			case 3:
			case 6:
			case 9:
			case 12:
				return true;
			default:
				return false;
			}
		}

		public static bool IsKeyBlock(int BlockId)
		{
			if (BlockId < 128)
			{
				if ((BlockId + 1) % 4 == 0)
				{
					return true;
				}
			}
			else if (BlockId < 256 && (BlockId - 128 + 1) % 16 == 0 && BlockId - 128 != 0)
			{
				return true;
			}
			return false;
		}

		public static bool IsFirstBlockInSector(int BlockId)
		{
			if (BlockId < 128)
			{
				if (BlockId % 4 == 0)
				{
					return true;
				}
			}
			else if (BlockId < 256 && (BlockId - 128) % 16 == 0)
			{
				return true;
			}
			return false;
		}

		public static int Beep(byte time, byte count)
		{
			return ReaderAPI.Beep(time, count);
		}

		public static int GetCardSerial(out uint CardSerialNo)
		{
			byte[] array = new byte[4];
			byte[] snr = new byte[16];
			int num = 0;
			CardSerialNo = 0u;
			num = ReaderAPI.GET_SNR(IntPtr.Zero, 0, 38, 0, snr, array);
			if (num == 0)
			{
				CardSerialNo = BitConverter.ToUInt32(array, 0);
			}
			else if (1 == num)
			{
				num = 131;
			}
			return num;
		}

		public static int ReadOneBlock(int BlockId, out byte[] BackData, string Key = "FF FF FF FF FF FF")
		{
			byte[] array = new byte[6];
			BackData = new byte[16];
			int num = 0;
			if (BlockId < 0 || BlockId > 255)
			{
				BackData = null;
				return -100;
			}
			Array.Copy(CardReader.HexString2Bytes(Key), array, 6);
			if (array.Length != 6)
			{
				return -102;
			}
			return ReaderAPI.MI_Read((byte)BlockId, 1, array, BackData);
		}

		public static int ReadOneSector(int SectorId, out byte[] BackData, string Key)
		{
			int num = SectorId * 4;
			int num2 = 4;
			int num3 = 48;
			byte[] array = new byte[6];
			byte[] array2 = new byte[num3];
			int num4 = 0;
			if (SectorId < 0 || SectorId > 39)
			{
				BackData = null;
				return -101;
			}
			if (SectorId < 32)
			{
				num = SectorId * 4;
				num2 = 4;
			}
			else if (SectorId < 40)
			{
				num = 128 + (SectorId - 32) * 16;
				num2 = 16;
			}
			BackData = new byte[(num2 - 1) * 16];
			int num5 = 0;
			int result;
			while (true)
			{
				if (num5 < (num2 - 1) / 3)
				{
					array = CardReader.HexString2Bytes(Key);
					if (array.Length != 6)
					{
						result = -102;
						break;
					}
					num4 = ReaderAPI.MI_Read((byte)(num + num5 * 3), 3, array, array2);
					if (num4 != 0)
					{
						return num4;
					}
					Array.Copy(array2, 0, BackData, num5 * 3 * 16, array2.Length);
					num5++;
					continue;
				}
				return num4;
			}
			return result;
		}

		public static int ReadData(int StartSectorId, int DataLength, out byte[] BackData, string Key)
		{
			int num = -1;
			BackData = new byte[DataLength];
			if (StartSectorId < 0 || StartSectorId > 39)
			{
				BackData = null;
				return -101;
			}
			CardReader.CaclSectorCount(StartSectorId, DataLength, out int num2, out int num3);
			int num4 = num2 + num3;
			int num5 = 0;
			int num6 = 0;
			int result;
			while (true)
			{
				if (num6 < num4)
				{
					num = CardReader.ReadOneSector(StartSectorId + num6, out byte[] array, Key);
					if (num != 0)
					{
						result = num;
						break;
					}
					if (DataLength - num5 > array.Length)
					{
						Array.Copy(array, 0, BackData, num5, array.Length);
					}
					else if (DataLength > array.Length)
					{
						Array.Copy(array, 0, BackData, num5, DataLength - num5);
					}
					else
					{
						Array.Copy(array, 0, BackData, num5, DataLength);
					}
					num5 += array.Length;
					if (num5 < DataLength)
					{
						num6++;
						continue;
					}
				}
				return num;
			}
			return result;
		}

		public static int WriteOneBlock(int BlockId, byte[] BlockData, string Key = "FF FF FF FF FF FF")
		{
			if (BlockId < 0 || BlockId > 255)
			{
				BlockData = null;
				return -100;
			}
			if (CardReader.IsKeyBlock(BlockId))
			{
				return -104;
			}
			int num = 0;
			byte[] array = new byte[6];
			byte[] array2 = new byte[16];
			array = CardReader.HexString2Bytes(Key);
			if (array.Length != 6)
			{
				return -102;
			}
			if (BlockData.Length < array2.Length)
			{
				Array.Copy(BlockData, 0, array2, 0, BlockData.Length);
			}
			else
			{
				Array.Copy(BlockData, 0, array2, 0, array2.Length);
			}
			return ReaderAPI.MI_Write((byte)BlockId, 1, array, array2);
		}

		public static int WriteAllData(int StartSectorId, byte[] AllData, string Key = "FF FF FF FF FF FF", bool CheckKey = false)
		{
			int num = -1;
			if (StartSectorId < 0 || StartSectorId > 39)
			{
				AllData = null;
				return -101;
			}
			CardReader.CaclSectorCount(StartSectorId, AllData.Length, out int num2, out int num3);
			int num4 = num2 + num3;
			if (CheckKey)
			{
				for (int i = 0; i < num4; i++)
				{
					num = CardReader.CheckSectorKey(StartSectorId + i, Key);
					if (num != 0)
					{
						return num;
					}
				}
			}
			int num5 = 48;
			int num6 = 0;
			int num7 = AllData.Length;
			for (int j = 0; j < num2; j++)
			{
				byte[] array = new byte[num5];
				Array.Copy(AllData, num6, array, 0, (num7 > array.Length) ? array.Length : num7);
				num = CardReader.WriteSector(StartSectorId + j, array, Key);
				if (num != 0)
				{
					return num;
				}
				num6 += array.Length;
				num7 -= array.Length;
			}
			if (num3 > 0)
			{
				num5 = 240;
				int num8 = 32;
				for (int k = 0; k < num3; k++)
				{
					byte[] array = new byte[num5];
					Array.Copy(AllData, num6, array, 0, (num7 > array.Length) ? array.Length : num7);
					num = CardReader.WriteSector(num8 + k, array, Key);
					if (num != 0)
					{
						return num;
					}
					num6 += array.Length;
					num7 -= array.Length;
				}
			}
			return num;
		}

		public static int WriteSector(int SectorId, byte[] SectorData, string Key)
		{
			int num = 0;
			int num2 = SectorId * 4;
			int num3 = 48;
			byte[] array = new byte[6];
			if (SectorId < 0 || SectorId > 39)
			{
				SectorData = null;
				return -101;
			}
			array = CardReader.HexString2Bytes(Key);
			if (array.Length != 6)
			{
				return -102;
			}
			if (SectorId < 32)
			{
				num2 = SectorId * 4;
				num3 = 48;
				byte[] array2 = new byte[num3];
				Array.Copy(SectorData, 0, array2, 0, (SectorData.Length > array2.Length) ? array2.Length : SectorData.Length);
				num = ReaderAPI.MI_Write((byte)num2, 3, array, array2);
				if (num != 0)
				{
					return num;
				}
			}
			else if (SectorId < 40)
			{
				num2 = 128 + (SectorId - 32) * 16;
				num3 = 16;
				int num4 = (int)Math.Ceiling((double)SectorData.Length / (double)num3);
				if (num4 > 15)
				{
					num4 = 15;
				}
				int num5 = SectorData.Length;
				for (int i = 0; i < num4; i++)
				{
					array = CardReader.HexString2Bytes(Key);
					byte[] array2 = new byte[num3];
					Array.Copy(SectorData, i * num3, array2, 0, (num5 > array2.Length) ? array2.Length : num5);
					num = ReaderAPI.MI_Write((byte)(num2 + i), 1, array, array2);
					if (num != 0)
					{
						return num;
					}
					num5 -= array2.Length;
				}
			}
			return num;
		}

		public static int CheckSectorKey(int SectorId, string Key)
		{
			byte[] array = new byte[16];
			if (SectorId < 0 || SectorId > 39)
			{
				return -101;
			}
			int blockId = -1;
			if (SectorId < 32)
			{
				blockId = SectorId * 4;
			}
			else if (SectorId < 40)
			{
				blockId = 128 + (SectorId - 32) * 16;
			}
			return CardReader.ReadOneBlock(blockId, out array, Key);
		}

		public static int ChangeSectorKey(int SectorId, string newKey, string oldKey = "FF FF FF FF FF FF")
		{
			byte[] array = CardReader.HexString2Bytes(newKey);
			byte[] array2 = new byte[16];
			if (newKey.Replace(" ", "").Length != 12)
			{
				return -106;
			}
			if (SectorId < 0 || SectorId > 39)
			{
				return -101;
			}
			if (array.Length != 6)
			{
				return -102;
			}
			int num = -1;
			if (SectorId < 32)
			{
				num = (SectorId + 1) * 4 - 1;
			}
			else if (SectorId < 40)
			{
				num = 127 + (SectorId - 31) * 16;
			}
			array2 = CardReader.HexString2Bytes("000000000000FF078069FFFFFFFFFFFF");
			Array.Copy(array, 0, array2, 0, 6);
			return ReaderAPI.MI_Write((byte)num, 1, CardReader.HexString2Bytes(oldKey), array2);
		}
	}
}
