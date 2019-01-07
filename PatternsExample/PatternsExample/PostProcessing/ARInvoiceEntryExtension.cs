using System;
using System.Linq;
using System.Collections.Generic;
using PX.Data;
using PX.Common;
using PX.Objects.AR;
using PX.Objects.GL;
using PX.Objects.CS;
using PX.Objects.CR;

namespace Patterns
{
	public class ARReleaseProcessExt : PXGraphExtension<ARReleaseProcess>
	{
		public void ARRegister_Released_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			ARRegister row = e.Row as ARRegister;

			if (row != null && row.Released == true && row.DocType != null)
			{
				switch (row.DocType)
				{
					case ARDocType.CashSale:
					case ARDocType.DebitMemo:
					case ARDocType.CreditMemo:
					case ARDocType.Invoice:
						cache.SetValue<ARInvoiceExt.usrRequiredProcess>(row, true);
						break;
				}
			}
		}
	}
}