using System.Data;
using ZK.Data.IDAL;
using ZK.Framework;

namespace ZK.Data.BLL
{
	public class LevelDataClassBll
	{
		private IApplication _ia = null;

		private readonly ILevelDataClass dal = null;

		public LevelDataClassBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(ILevelDataClass));
			if (service != null)
			{
				this.dal = (service as ILevelDataClass);
			}
		}

		public DataSet GetList()
		{
			return this.dal.GetList();
		}
	}
}
