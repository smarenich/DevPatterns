using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.SM;
using PX.TM;
using PX.Data;
using PX.Objects;
using PX.Objects.CR;
using PX.Objects.AR;
using PX.Objects.CT;
using PX.Objects.CS;
using PX.Data.SQLTree;


namespace Patterns
{
	#region ScalarCount
	public sealed class ScalarCount<Field> : IBqlFunction
		where Field : IBqlField
	{
		public void GetAggregates(List<IBqlFunction> fields)
		{
			fields.Add(this);
		}

		public bool AppendExpression(ref SQLExpression exp, PXGraph graph, PX.Data.BqlCommandInfo info, BqlCommand.Selection selection) 
		{
			info.Tables?.Add(typeof(int));

			exp = new SQLExpression();
			exp.SetOper(SQLExpression.Operation.COUNT_DISTINCT);
			exp.SetRight(BqlCommand.GetSingleExpression(typeof(Field), graph, info.Tables, selection, BqlCommand.FieldPlace.Condition));

			return true;
		}

		public void Parse(PXGraph graph, List<IBqlParameter> pars, List<Type> tables, List<Type> fields, List<IBqlSortColumn> sortColumns, StringBuilder text, BqlCommand.Selection selection)
		{
			if (tables != null)
			{
				tables.Add(typeof(int));
			}
			if (graph != null && text != null)
			{
				StringBuilder bld = new StringBuilder("COUNT(DISTINCT ");
				bld.Append(BqlCommand.GetSingleField(typeof(Field), graph, tables, selection, BqlCommand.FieldPlace.Condition));
				bld.Append(')');
			}
		}
		public void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value) { }
		public string GetFunction() { return "COUNT"; }
		public Type GetField() { return typeof(Field); ; }
		public bool IsGroupBy() { return false; }
	}
	#endregion

	#region Customer Extension
	[Serializable]
	public sealed class CustomerExt : PXCacheExtension<Customer>
	{
		#region CasesTotal
		public abstract class casesTotal : IBqlField
		{ }
		[PXDBScalar(typeof(Search4<CRCase.caseID,
			Where<CRCase.customerID, Equal<Customer.bAccountID>>,
			Aggregate<ScalarCount<CRCase.caseID>>>))]
		[PXUIField(DisplayName = "Total Cases")]
		public Int32? CasesTotal { get; set; }
		#endregion
	}
	#endregion
}
