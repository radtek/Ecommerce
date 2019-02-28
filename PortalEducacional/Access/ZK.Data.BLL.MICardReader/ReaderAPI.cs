using System;
using System.Runtime.InteropServices;

namespace ZK.Data.BLL.MICardReader
{
	internal class ReaderAPI
	{
		protected static IntPtr handle = IntPtr.Zero;

		protected static int deviceAddress = 0;

		protected static byte mode = 0;

		[DllImport("mi.dll")]
		protected static extern int API_ControlBuzzer(IntPtr comHandle, int DeviceAddress, byte time, byte count, byte[] buffer);

		[DllImport("mi.dll")]
		protected static extern int API_PCDRead(IntPtr comHandle, int DeviceAddress, byte mode, byte blk_add, byte num_blk, byte[] KeyData, byte[] BackData);

		[DllImport("mi.dll")]
		protected static extern int API_PCDWrite(IntPtr comHandle, int DeviceAddress, byte mode, byte blk_add, byte num_blk, byte[] KeyData, byte[] BlockData);

		[DllImport("mi.dll")]
		public static extern int GET_SNR(IntPtr comHandle, int DeviceAddress, byte mode, byte RDM_halt, byte[] snr, byte[] SerialData);

		[DllImport("mi3.dll")]
		protected static extern int MF_Read(byte mode, byte add_blk, byte num_blk, byte[] key, byte[] bufferr);

		[DllImport("mi3.dll")]
		protected static extern int MF_Write(byte mode, byte add_blk, byte num_blk, byte[] key, byte[] keywrite);

		public static int DetectReader()
		{
			byte[] serialData = new byte[4];
			byte[] snr = new byte[16];
			int num = ReaderAPI.GET_SNR(ReaderAPI.handle, ReaderAPI.deviceAddress, 38, 0, snr, serialData);
			if (num != 5)
			{
				return 0;
			}
			return num;
		}

		public static int Beep(byte time, byte count)
		{
			byte[] buffer = new byte[16];
			int num = ReaderAPI.DetectReader();
			if (num != 0)
			{
				return num;
			}
			return ReaderAPI.API_ControlBuzzer(ReaderAPI.handle, ReaderAPI.deviceAddress, time, count, buffer);
		}

		public static int MI_Read(byte BlockId, byte BlockCount, byte[] KeyData, byte[] BackData)
		{
			int num = ReaderAPI.DetectReader();
			if (num != 0)
			{
				return num;
			}
			num = ReaderAPI.API_PCDRead(ReaderAPI.handle, ReaderAPI.deviceAddress, ReaderAPI.mode, BlockId, BlockCount, KeyData, BackData);
			if (num != 0)
			{
				int num2 = BackData[0];
				if (num2 != 0)
				{
					num = num2;
				}
			}
			return num;
		}

		public static int MI_Write(byte BlockId, byte BlockCount, byte[] KeyData, byte[] BlockData)
		{
			int num = ReaderAPI.DetectReader();
			if (num != 0)
			{
				return num;
			}
			num = ReaderAPI.API_PCDWrite(ReaderAPI.handle, ReaderAPI.deviceAddress, ReaderAPI.mode, BlockId, BlockCount, KeyData, BlockData);
			if (num != 0)
			{
				int num2 = BlockData[0];
				if (num2 != 0)
				{
					num = num2;
				}
			}
			return num;
		}

		public static int MI3_Read(byte BlockId, byte BlockCount, byte[] KeyData, byte[] BackData)
		{
			int num = ReaderAPI.DetectReader();
			if (num != 0)
			{
				return num;
			}
			num = ReaderAPI.MF_Read(ReaderAPI.mode, BlockId, BlockCount, KeyData, BackData);
			if (num != 0)
			{
				int num2 = BackData[0];
				if (num2 != 0)
				{
					num = num2;
				}
			}
			return num;
		}

		public static int MI3_Write(byte BlockId, byte BlockCount, byte[] KeyData, byte[] BlockData)
		{
			int num = ReaderAPI.DetectReader();
			if (num != 0)
			{
				return num;
			}
			num = ReaderAPI.MF_Write(ReaderAPI.mode, BlockId, BlockCount, KeyData, BlockData);
			if (num != 0)
			{
				int num2 = BlockData[0];
				if (num2 != 0)
				{
					num = num2;
				}
			}
			return num;
		}
	}
}
