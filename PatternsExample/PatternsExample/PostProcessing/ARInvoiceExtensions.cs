using System;
using PX.Data;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.CM;
using PX.Objects.CR;
using ARCashSale = PX.Objects.AR.Standalone.ARCashSale;
using SOInvoice = PX.Objects.SO.SOInvoice;
using CRLocation = PX.Objects.CR.Standalone.Location;
using IRegister = PX.Objects.CM.IRegister;
using System.Collections.Generic;
using PX.Objects;
using PX.Objects.AR;
using PX.Objects.AP;

namespace Patterns
{
	public class ARInvoiceExt : PXCacheExtension<PX.Objects.AR.ARRegister>
	{
		#region UsrRequiredProcess
		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Process Required", Enabled = false, IsReadOnly = true)]
		public virtual bool? UsrRequiredProcess { get; set; }
		public abstract class usrRequiredProcess : IBqlField { }
		#endregion
		#region UsrCompletedProcess
		[PXDBBool]
		[PXDefault(false, PersistingCheck = PXPersistingCheck.Nothing)]
		[PXUIField(DisplayName = "Process Completed", Enabled = false, IsReadOnly = true)]
		public virtual bool? UsrCompletedProcess { get; set; }
		public abstract class usrCompletedProcess : IBqlField { }
		#endregion
	}
}