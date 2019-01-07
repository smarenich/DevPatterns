using System;
using System.Collections;
using System.Collections.Generic;
using PX.SM;
using PX.Data;
using PX.Objects.SO;
using PX.Objects.CR;
using PX.Common;
using PX.TM;
using PX.Objects.EP;
using PX.Objects.AR;

namespace Patterns
{
	public class SOOrderEntryExtension : PXGraphExtension<SOOrderEntry>
	{
		[PXViewName(PX.Objects.SO.Messages.SOOrder)]
		public PXSelectJoin<SOOrder,
			LeftJoinSingleTable<Customer, On<Customer.bAccountID, Equal<SOOrder.customerID>>>,
			Where<SOOrder.orderType, Equal<Optional<SOOrder.orderType>>,
				And2<Where<Customer.bAccountID, IsNull,
					Or<Match<Customer, Current<AccessInfo.userName>>>>,
				And<Where<
					SOOrder.createdByID, Equal<Current<AccessInfo.userID>>,
					Or<SOOrder.ownerID, OwnedUser<Current<AccessInfo.userID>>,
					Or<SOOrder.noteID, OwnedGroup<SOOrder.noteID, Current<AccessInfo.userID>>>>>
				>>>> Document;

		[PXViewName("VisibleTo")]
		public EntityRestrictionAutomation<SOOrder.noteID, SOOrder.customerID, SOLine.inventoryID> VisibileTo;

		[PXRefNote]
		[PXDefault(typeof(SOOrder.noteID))]
		public virtual void EntityWorkgroup_RefNoteID_CacheAttached(PXCache sender) { }
	}
}