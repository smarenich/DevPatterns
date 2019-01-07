using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Common;
using PX.Objects.IN;
using PX.Objects.SO;
using PX.Data.ReferentialIntegrity.Attributes;

namespace Patterns.Keys
{
	public partial class InventoryItem : PX.Data.IBqlTable
	{
		public class PK : PrimaryKeyOf<InventoryItem>.By<inventoryID>
		{
			public static InventoryItem Find(PXGraph graph, int inventoryID)
			  => FindBy(graph, inventoryID);
		}
		public abstract class inventoryID : PX.Data.IBqlField { }
	}

	public partial class SOOrder : PX.Data.IBqlTable
	{
		public class PK : PrimaryKeyOf<SOOrder>.By<orderType, orderNbr>
		{
			public static SOOrder Find(
				PXGraph graph, string orderType, string orderNbr)
				=> FindBy(graph, orderType, orderNbr);
		}

		public abstract class orderType : PX.Data.IBqlField { }
		public abstract class orderNbr : PX.Data.IBqlField { }
	}

	public partial class SOLine : PX.Data.IBqlTable
	{
		public class PK : PrimaryKeyOf<SOLine>.By<orderType, orderNbr, lineNbr>
		{
			public static SOLine Find(
				PXGraph graph, string orderType, string orderNbr, int lineNbr)
					=> FindBy(graph, orderType, orderNbr, lineNbr);
		}

		public class SOOrderFK : SOOrder.PK.ForeignKeyOf<SOLine>.By<orderType, orderNbr>
		{ }

		public abstract class orderType : PX.Data.IBqlField { }
		public abstract class orderNbr : PX.Data.IBqlField { }
		public abstract class lineNbr : PX.Data.IBqlField { }
	}

	public class PrimaryKeyAPIExample : PXGraph<PrimaryKeyAPIExample>
	{
		public void GetInventoryItem(SOLineSplit split)
		{
			InventoryItem item = InventoryItem.PK.Find(this, split.InventoryID.Value);
		}
		public void GetSOLine(SOLineSplit split)
		{ 
			SOLine line = SOLine.PK.Find(this, split.OrderType, split.OrderNbr, split.LineNbr.Value);
		}

		public void ForeignKey(SOLine soLine, SOOrder soOrder)
		{
			//Select the parent record
			SOOrder order = SOLine.SOOrderFK.FindParent(this, soLine);

			//Select the child records
			IEnumerable<SOLine> lines = SOLine.SOOrderFK.SelectChildren(this, soOrder);
		}
	}
}
