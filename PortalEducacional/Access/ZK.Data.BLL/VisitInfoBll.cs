using ZK.ConfigManager;
using ZK.Data.IDAL;
using ZK.Data.OleDbDAL;
using ZK.Data.SqlDbDAL;

namespace ZK.Data.BLL
{
	public class VisitInfoBll
	{
		public static IVisitInfo getDALObject()
		{
			if (AppSite.Instance.DataType == DataType.SqlServer)
			{
				return new ZK.Data.SqlDbDAL.VisitInfoDal();
			}
			return new ZK.Data.OleDbDAL.VisitInfoDal();
		}
	}
}
