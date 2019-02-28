using System.Collections.Generic;
using ZK.Data.IDAL.PullSDK;
using ZK.Data.Model.PullSDK;
using ZK.Framework;

namespace ZK.Data.BLL.PullSDK
{
	public class MultimCardBll
	{
		private IApplication _ia = null;

		private int m_errorNo = -999;

		private readonly IMultimCard dal = null;

		public MultimCardBll(IApplication ia)
		{
			this._ia = ia;
			object service = ia.GetService(typeof(IMultimCard));
			if (service != null)
			{
				this.dal = (service as IMultimCard);
			}
		}

		public int Add(ObjMultimCard model)
		{
			if (this.dal != null)
			{
				return this.dal.Add(model);
			}
			return this.m_errorNo;
		}

		public int Add(List<ObjMultimCard> list)
		{
			if (this.dal != null)
			{
				return this.dal.Add(list);
			}
			return this.m_errorNo;
		}

		public List<ObjMultimCard> GetList(ref int ret)
		{
			if (this.dal != null)
			{
				return this.dal.GetList(ref ret);
			}
			return null;
		}

		public List<ObjMultimCard> GetList(string filter, string options, ref int ret)
		{
			if (this.dal != null)
			{
				return this.dal.GetList(filter, options, ref ret);
			}
			return null;
		}

		public int GetAllCount()
		{
			if (this.dal != null)
			{
				return this.dal.GetAllCount();
			}
			return this.m_errorNo;
		}

		public int GetAllCount(string filter, string options)
		{
			if (this.dal != null)
			{
				return this.dal.GetAllCount(filter, options);
			}
			return this.m_errorNo;
		}

		public int Delete(ObjMultimCard model)
		{
			if (this.dal != null)
			{
				return this.dal.Delete(model);
			}
			return this.m_errorNo;
		}

		public int Delete(List<ObjMultimCard> list)
		{
			if (this.dal != null)
			{
				return this.dal.Delete(list);
			}
			return this.m_errorNo;
		}

		public int Delete()
		{
			if (this.dal != null)
			{
				return this.dal.Delete();
			}
			return this.m_errorNo;
		}
	}
}
