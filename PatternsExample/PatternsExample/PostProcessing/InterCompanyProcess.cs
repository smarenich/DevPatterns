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
using PX.Objects.IN;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using PX.Objects.PM;

namespace Patterns
{
	public class PostProcessing : PXGraph<PostProcessing>
	{
		public PXCancel<ARRegister> Cancel;
		public PXProcessing<ARRegister,
			Where<ARInvoiceExt.usrRequiredProcess, Equal<True>,
				And<ARInvoiceExt.usrCompletedProcess, Equal<False>>>> Records;

		public PostProcessing()
		{
			Records.SetSelected<APInvoice.selected>();
			Records.SetProcessDelegate(delegate (List<ARRegister> records)
			{
				ProcessRecords(records);
			});
		}

		public static void ProcessRecords(List<ARRegister> records)
		{
			PXGraph graph = PXGraph.CreateInstance<PXGraph>();

			Boolean anyFailed = false;
			foreach (ARRegister rec in records)
			{
				PXProcessing.SetCurrentItem(rec);
				try
				{
					ARRegister invoice = PXSelect<ARRegister,
						Where<ARRegister.docType, Equal<Required<ARRegister.docType>>, And<ARRegister.refNbr, Equal<Required<ARRegister.refNbr>>>>>.Select(graph, rec.DocType, rec.RefNbr);					

					PXProcessing.SetProcessed();
				}
				catch (Exception ex)
				{
					anyFailed = true;
					PXProcessing.SetError(ex);
				}
			}
			PXProcessing.SetCurrentItem(null);

			if (anyFailed) throw new PXException(PX.Data.ErrorMessages.SeveralItemsFailed);
		}
	}
}