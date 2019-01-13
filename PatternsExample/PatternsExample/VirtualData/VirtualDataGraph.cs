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

				Entity[] result = null;
				using (DefaultSoapClient client = new DefaultSoapClient(CreateBinding(), CreateEndpoint()))
				{
					try
					{
						client.Login("admin@Summit", "123", null, null, null);

						result = client.GetList(new APIExample.Account()
						{
							AccountCD = new StringReturn(),
							Description = new StringReturn(),
							ReturnBehavior = ReturnBehavior.OnlySpecified,
						});
					}
					catch
					{
					}
					client.Logout();

					if (result != null)
					{
						foreach (APIExample.Account acct in result)
						{
							VirtualDAC row = (VirtualDAC)VirtualDataView.Cache.CreateInstance();
							row.Code = acct.AccountCD.Value;
							row.Description = acct.Description.Value;

							row = VirtualDataView.Insert(row);
							yield return row;
						}
					}
				}

				VirtualDataView.Cache.IsDirty = false;
			}
		}

		public const String URL = "http://sm/Summit/";
		public const String ENDPOINT = "Default";
		public const String VERSION = "18.200.001";

		public static System.ServiceModel.EndpointAddress CreateEndpoint()
		{
			string url = URL;
			if (url != null && url[url.Length - 1] != '/') url += "/";
			string endpoint = ENDPOINT;
			if (endpoint != null && endpoint[endpoint.Length - 1] != '/') endpoint += "/";

			Uri uri = new Uri(new Uri(new Uri(new Uri(url), "entity/"), endpoint), VERSION);
			return new System.ServiceModel.EndpointAddress(uri.ToString());
		}
		public static System.ServiceModel.Channels.Binding CreateBinding()
		{
			if (URL.StartsWith("https://"))
			{
				return new System.ServiceModel.BasicHttpsBinding()
				{
					AllowCookies = true,
					MaxReceivedMessageSize = 6553600,
					SendTimeout = TimeSpan.FromMinutes(5),
					ReceiveTimeout = TimeSpan.FromMinutes(5),
					Security = new System.ServiceModel.BasicHttpsSecurity() { Mode = System.ServiceModel.BasicHttpsSecurityMode.Transport },
				};
			}
			else
			{
				return new System.ServiceModel.BasicHttpBinding()
				{
					AllowCookies = true,
					MaxReceivedMessageSize = 6553600,
					SendTimeout = TimeSpan.FromMinutes(5),
					ReceiveTimeout = TimeSpan.FromMinutes(5),
					//Security = new System.ServiceModel.BasicHttpSecurity() { Mode = System.ServiceModel.BasicHttpSecurityMode.Transport },
				};
			}
		}
	}
}
