using System;
using PX.Data;
using PX.Common;
using PX.Objects.SO;

namespace Patterns
{
	[Serializable]
	public class SOOrderExt : PXCacheExtension<SOOrder>
	{
		#region AcctReferenceNbr
		[PXString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "CRM ID", Visibility = PXUIVisibility.SelectorVisible)]
		[VirtualCRMIDAttribute(typeof(SOOrder.customerID))]
		public virtual string AcctReferenceNbr { get; set; }
		public abstract class acctReferenceNbr : IBqlField { }
		#endregion
	}
}