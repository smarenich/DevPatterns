using System;
using PX.Data;
using PX.Common;
using PX.Objects.SO;
using PX.Objects.AR;
using PX.Objects.CR;
using PX.TM;

namespace Patterns
{
	[Serializable]
	public class SOOrderExt : PXCacheExtension<SOOrder>
	{
		#region OrderNbr
		public abstract class orderNbr : PX.Data.IBqlField
		{
		}
		[PXMergeAttributes(Method = MergeMethod.Append)]
		[PXRemoveBaseAttribute(typeof(SO.RefNbrAttribute))]
		[SO.RefNbr(typeof(Search2<SOOrder.orderNbr,
			LeftJoinSingleTable<Customer, On<SOOrder.customerID, Equal<Customer.bAccountID>,
				And<Where<Match<Customer, Current<AccessInfo.userName>>>>>,
			LeftJoin<SOOrderType, On<SOOrderType.orderType, Equal<SOOrder.orderType>>>>,
			Where<SOOrder.orderType, Equal<Optional<SOOrder.orderType>>,
				And2<Where<SOOrderType.aRDocType, Equal<ARDocType.noUpdate>,
					Or<Customer.bAccountID, IsNotNull>>,
				And<Where<
					SOOrder.createdByID, Equal<Current<AccessInfo.userID>>,
					Or<SOOrder.ownerID, OwnedUser<Current<AccessInfo.userID>>,
					Or<SOOrder.noteID, OwnedGroup<SOOrder.noteID, Current<AccessInfo.userID>>>>>>>>,
			 OrderBy<Desc<SOOrder.orderNbr>>>), Filterable = true)]
		public virtual String OrderNbr { get; set; }
		#endregion

		#region AcctReferenceNbr
		[PXString(50, IsUnicode = true)]
		[PXUIField(DisplayName = "Customer Ext ID", Visibility = PXUIVisibility.SelectorVisible)]
		[AttributeRefAttribute(typeof(SOOrder.customerID), typeof(BAccount.noteID), "EXTREFNBR")]
		public virtual string AcctReferenceNbr { get; set; }
		public abstract class acctReferenceNbr : IBqlField { }
		#endregion
	}
}