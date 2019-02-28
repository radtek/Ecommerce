using System.Collections.Generic;
using System.IO;
using ZK.Data.IDAL.UDisk;
using ZK.Data.Model;
using ZK.Data.Model.PullSDK;
using ZK.MachinesManager.UDisk;

namespace ZK.Data.BLL.UDisk
{
	public class UDiskBll
	{
		private int m_errorNo = -999;

		private readonly IUDataManager dal = null;

		public UDiskBll(SDKType sdkType)
		{
			if (sdkType == SDKType.StandaloneSDK)
			{
				this.dal = new StdUDiskDal();
			}
			else
			{
				this.dal = new PullUDiskDal();
			}
		}

		public bool InitializeFiles(DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.InitializeFiles(dir, Machine);
			}
			return false;
		}

		public int ImportUserInfo(out List<UserInfo> lstUserInfo, int PinWidth, bool IsTFT)
		{
			if (this.dal != null)
			{
				return this.dal.ImportUserInfo(out lstUserInfo, PinWidth, IsTFT);
			}
			lstUserInfo = new List<UserInfo>();
			return this.m_errorNo;
		}

		public int ImportFingerPrint(int FpVersion, out List<Template> lstTemplate)
		{
			if (this.dal != null)
			{
				return this.dal.ImportFingerPrint(FpVersion, out lstTemplate);
			}
			lstTemplate = new List<Template>();
			return this.m_errorNo;
		}

		public int ImportFingerVein(out List<FingerVein> lstFingerVein)
		{
			if (this.dal != null)
			{
				return this.dal.ImportFingerVein(out lstFingerVein);
			}
			lstFingerVein = new List<FingerVein>();
			return this.m_errorNo;
		}

		public int ImportFaceTemplate(out List<FaceTemp> lstFaceTemp)
		{
			if (this.dal != null)
			{
				return this.dal.ImportFaceTemplate(out lstFaceTemp);
			}
			lstFaceTemp = new List<FaceTemp>();
			return this.m_errorNo;
		}

		public int ImportTransaction(string FullFileName, out List<ObjTransAction> lstTransaction, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ImportTransaction(FullFileName, out lstTransaction, Machine);
			}
			lstTransaction = new List<ObjTransAction>();
			return this.m_errorNo;
		}

		public int ExportUserInfo(List<UserInfo> list, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportUserInfo(list, dir, Machine);
			}
			return this.m_errorNo;
		}

		public int ExportFingerPrint(List<Template> lstFPTemplate, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportFingerPrint(lstFPTemplate, dir, Machine);
			}
			return this.m_errorNo;
		}

		public int ExportFaceTemplate(List<FaceTemp> lstFaceTemplate, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportFaceTemplate(lstFaceTemplate, dir, Machine);
			}
			return this.m_errorNo;
		}

		public int ExportFingerVein(List<FingerVein> lstFVTemplate, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportFingerVein(lstFVTemplate, dir, Machine);
			}
			return this.m_errorNo;
		}

		public int ExportAuthorize(List<ObjUserAuthorize> lstUserAuthorize, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportAuthorize(lstUserAuthorize, dir, Machine);
			}
			return this.m_errorNo;
		}

		public int ExportTimeZone(List<AccTimeseg> lstTimeZone, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportTimeZone(lstTimeZone, dir, Machine);
			}
			return this.m_errorNo;
		}

		public int ExportHoliday(List<AccHolidays> lstHoliday, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportHoliday(lstHoliday, dir, Machine);
			}
			return this.m_errorNo;
		}

		public int ExportFirstCard(List<ObjFirstCard> lstFirstCard, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportFirstCard(lstFirstCard, dir, Machine);
			}
			return this.m_errorNo;
		}

		public int ExportMultiCard(List<ObjMultimCard> lstMultimCard, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportMultiCard(lstMultimCard, dir, Machine);
			}
			return this.m_errorNo;
		}

		public int ExportInOutFun(List<ObjInOutFun> lstInOutFun, DirectoryInfo dir, Machines Machine)
		{
			if (this.dal != null)
			{
				return this.dal.ExportInOutFun(lstInOutFun, dir, Machine);
			}
			return this.m_errorNo;
		}
	}
}
