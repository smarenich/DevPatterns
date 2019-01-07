using System;
using System.Linq;
using System.Collections.Generic;
using PX.Data;
using PX.Data.Update;
using PX.Common;
using PX.Objects.AP;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.Objects.CR;
using System.Collections;
using PX.SM;
using Patterns.APIExample;

namespace Patterns
{
	public class YearGroupMaint : PXGraph<YearGroupMaint>
	{
		public PXCancel<VirtualDAC> Cancel;

		[PXVirtualDAC]
		public PXSelect<VirtualDAC> VirtualDataView;

		public IEnumerable virtualDataView()
		{
			Boolean anyfound = false;
			foreach (VirtualDAC row in VirtualDataView.Cache.Inserted)
			{
				anyfound = true;
				yield return row;
			}

			if (!anyfound)
			{
				VirtualDataView.Cache.Clear();

				DefaultSoapClient client = new DefaultSoapClient();
				client.Login("admin", "123", null, null, null);

				Entity[] result = client.GetList(new APIExample.Account()
				{
					AccountCD = new StringReturn(),
					Description = new StringReturn(),
					ReturnBehavior = ReturnBehavior.OnlySpecified,
				});

				foreach (APIExample.Account acct in result)
				{
					VirtualDAC row = (VirtualDAC)VirtualDataView.Cache.CreateInstance();
					row.Code = acct.AccountCD.Value;
					row.Description = acct.Description.Value;

					row = VirtualDataView.Insert(row);
					yield return row;
				}

				VirtualDataView.Cache.IsDirty = false;
			}
		}
	}
}
