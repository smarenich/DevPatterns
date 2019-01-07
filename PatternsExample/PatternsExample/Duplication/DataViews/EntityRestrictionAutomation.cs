using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PX.Data;
using PX.Data.SQLTree;
using PX.Objects;
using PX.Objects.CR;
using PX.Objects.IN;
using PX.SM;
using PX.TM;

namespace Patterns
{
	public class OwnedGroup<FieldNote, OperandUser> : IBqlComparison
		where FieldNote : IBqlField
		where OperandUser : IBqlOperand, new()
	{
		private IBqlCreator _operandUser;
		public void Verify(PXCache cache, object item, List<object> pars, ref bool? result, ref object value)
		{
			result = null;
			value = null;
		}

		public virtual bool AppendExpression(ref SQLExpression exp, PXGraph graph, BqlCommandInfo info, PX.Data.BqlCommand.Selection selection)
		{
			bool status = true;

			if (info.Fields is BqlCommand.EqualityList list) list.NonStrict = true;

			SQLExpression opUser = null;
			if (!typeof(IBqlCreator).IsAssignableFrom(typeof(OperandUser)))
			{
				if (info.BuildExpression) opUser = BqlCommand.GetSingleExpression(typeof(OperandUser), graph, info.Tables, selection, BqlCommand.FieldPlace.Condition);
				info.Fields?.Add(typeof(OperandUser));
			}
			else
			{
				if (_operandUser == null) _operandUser = _operandUser.createOperand<OperandUser>();
				status &= _operandUser.AppendExpression(ref opUser, graph, info, selection);
			}

			Query qin = new Query();
			qin[typeof(EPCompanyTreeH.workGroupID)].From(typeof(EPCompanyTreeH))
				.Join(typeof(EPCompanyTreeMember))
				.On(SQLExpression.Equal(typeof(EPCompanyTreeH.parentWGID), typeof(EPCompanyTreeMember.workGroupID))
					.And(Column.SQLColumn(typeof(EPCompanyTreeMember.active)).Equal(1))
					.And(Column.SQLColumn(typeof(EPCompanyTreeMember.userID)).Equal(opUser)))
				.Where(PX.Data.SQLTree.Constant.SQLConstant(1).Equal(1));

			Query qout = new Query();
			//Append Tail removes main object, so we fieldNote will not be mapped. Skipping conditions for AppendTail
			if (info.Tables == null || info.Tables.Count <= 0 || info.Tables.Contains(BqlCommand.GetItemType<FieldNote>()))
			{
				qout[typeof(EntityWorkgroup.refNoteID)].From(typeof(EntityWorkgroup))
					.Where(Column.SQLColumn(typeof(EntityWorkgroup.workGroupID)).In(qin)
						.And(SQLExpression.Equal(typeof(EntityWorkgroup.refNoteID), typeof(FieldNote))));
			}
			else
			{
				qout[typeof(EntityWorkgroup.refNoteID)].From(typeof(EntityWorkgroup))
					.Where(Column.SQLColumn(typeof(EntityWorkgroup.workGroupID)).In(qin));
			}

			exp.SetOper(SQLExpression.Operation.IN);
			qout.Limit(-1); // prevent limiting of IN subqueries
			exp.SetRight(new SubQuery(qout));
			return status;
		}

		public void Parse(PXGraph graph, List<IBqlParameter> pars, List<Type> tables, List<Type> fields, List<IBqlSortColumn> sortColumns, StringBuilder text, PX.Data.BqlCommand.Selection selection)
		{
			if (graph != null && text != null)
			{
				text.Append(" IN ").Append(BqlCommand.SubSelect);
				text.Append(graph.SqlDialect.quoteTableAndColumn(typeof(EntityWorkgroup).Name, typeof(EntityWorkgroup.refNoteID).Name));
				text.Append(" FROM ").Append(graph.SqlDialect.quoteDbIdentifier(typeof(EntityWorkgroup).Name)).Append(' ')
					.Append(graph.SqlDialect.quoteDbIdentifier(typeof(EntityWorkgroup).Name));
				text.Append(" WHERE  1=1");
				if (tables.Contains(BqlCommand.GetItemType<FieldNote>()))
				{
					text.Append(" AND ").Append(graph.SqlDialect.quoteTableAndColumn(typeof(EntityWorkgroup).Name, typeof(EntityWorkgroup.refNoteID).Name));
					text.Append('=').Append(graph.SqlDialect.quoteTableAndColumn(BqlCommand.GetItemType(typeof(FieldNote)).Name, typeof(FieldNote).Name));
				}
				text.Append(" AND ").Append(graph.SqlDialect.quoteTableAndColumn(typeof(EntityWorkgroup).Name, typeof(EntityWorkgroup.workGroupID).Name));
				text.Append(" IN ").Append(BqlCommand.SubSelect);
				text.Append(graph.SqlDialect.quoteTableAndColumn(typeof(EPCompanyTreeH).Name, typeof(EPCompanyTreeH.workGroupID).Name));
				text.Append(" FROM ").Append(graph.SqlDialect.quoteDbIdentifier(typeof(EPCompanyTreeH).Name)).Append(' ').Append(graph.SqlDialect.quoteDbIdentifier(typeof(EPCompanyTreeH).Name));
				text.Append(" INNER JOIN ").Append(graph.SqlDialect.quoteDbIdentifier(typeof(EPCompanyTreeMember).Name)).Append(' ').Append(graph.SqlDialect.quoteDbIdentifier(typeof(EPCompanyTreeMember).Name));
				text.Append(" AND ").Append(graph.SqlDialect.quoteTableAndColumn(typeof(EPCompanyTreeH).Name, typeof(EPCompanyTreeH.parentWGID).Name));
				text.Append("<>").Append(graph.SqlDialect.quoteTableAndColumn(typeof(EPCompanyTreeH).Name, typeof(EPCompanyTreeH.workGroupID).Name));
				text.Append(" AND ").Append(graph.SqlDialect.quoteTableAndColumn(typeof(EPCompanyTreeMember).Name, typeof(EPCompanyTreeMember.active).Name)).Append("=1");
				text.Append(" AND ").Append(graph.SqlDialect.quoteTableAndColumn(typeof(EPCompanyTreeMember).Name, typeof(EPCompanyTreeMember.userID).Name));
				text.Append('=');
				ParseOperandUser(graph, pars, tables, fields, sortColumns, text, selection);

				text.Append(" WHERE 1=1");
				text.Append(')').Append(')');
			}
			else
			{
				ParseOperandUser(graph, pars, tables, fields, sortColumns, text, selection);
			}
		}
		private void ParseOperandUser(PXGraph graph, List<IBqlParameter> pars, List<Type> tables, List<Type> fields, List<IBqlSortColumn> sortColumns, StringBuilder text, PX.Data.BqlCommand.Selection selection)
		{
			BqlCommand.EqualityList list = fields as BqlCommand.EqualityList;
			if (list != null)
				list.NonStrict = true;
			if (!typeof(IBqlCreator).IsAssignableFrom(typeof(OperandUser)))
			{
				if (graph != null && text != null)
					text.Append(" ").Append(BqlCommand.GetSingleField(typeof(OperandUser), graph, tables, selection, BqlCommand.FieldPlace.Condition));
				if (fields != null)
					fields.Add(typeof(OperandUser));
			}
			else
			{
				if (_operandUser == null)
					_operandUser = _operandUser.createOperand<OperandUser>();
				_operandUser.Parse(graph, pars, tables, fields, sortColumns, text, selection);
			}
		}
	}

	public class EntityRestrictionAutomation<SourceNote>
		: EntityRestrictionAutomation<SourceNote, SourceNote, SourceNote>
		where SourceNote : class, IBqlField
	{
		public EntityRestrictionAutomation(PXGraph graph, Delegate @delegate)
			: base(graph, @delegate)
		{
		}
		public EntityRestrictionAutomation(PXGraph graph)
			: base(graph)
		{
		}
	}

	public class EntityRestrictionAutomation<SourceNote, BAccountField, InventoryField>
		: PXSelect<EntityWorkgroup,
			Where<EntityWorkgroup.refNoteID, Equal<Current<SourceNote>>>>
		where SourceNote : class, IBqlField
		where BAccountField : class, IBqlField
		where InventoryField : class, IBqlField
	{
		public EntityRestrictionAutomation(PXGraph graph, Delegate @delegate)
			: base(graph, @delegate)
		{
			Initialize(graph);
		}
		public EntityRestrictionAutomation(PXGraph graph)
			: base(graph)
		{
			Initialize(graph);
		}
		private void Initialize(PXGraph graph)
		{
			graph.Initialized += InitLastEvents;
		}
		private void InitLastEvents(PXGraph graph)
		{
			if (typeof(SourceNote) != typeof(BAccountField))
				graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(typeof(BAccountField)), typeof(BAccountField).Name, BAcclountID_FieldUpdated);

			if (typeof(SourceNote) != typeof(InventoryField))
				graph.FieldUpdated.AddHandler(BqlCommand.GetItemType(typeof(InventoryField)), typeof(InventoryField).Name, InventoryID_FieldUpdated);
		}

		protected virtual void BAcclountID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Int32? OldID = (Int32?)e.OldValue;
			if (OldID != null)
			{
				BAccountR obj = (BAccountR)PXSelectorAttribute.Select<BAccountField>(cache, e.Row, OldID);
				Int32? workgroup = obj?.WorkgroupID;
				if (workgroup != null)
				{
					foreach (EntityWorkgroup wg in this.Search<EntityWorkgroup.workGroupID>(workgroup))
					{
						this.Delete(wg);
					}
				}
			}

			Int32? NewID = (Int32?)cache.GetValue<BAccountField>(e.Row);
			if (NewID != null)
			{
				BAccountR obj = (BAccountR)PXSelectorAttribute.Select<BAccountField>(cache, e.Row, NewID);
				Int32? workgroup = obj?.WorkgroupID;
				if (workgroup != null)
				{
					EntityWorkgroup wg = (EntityWorkgroup)this.Cache.CreateInstance();
					wg.WorkGroupID = workgroup;
					wg = this.Insert(wg);
				}
			}
		}
		protected virtual void InventoryID_FieldUpdated(PXCache cache, PXFieldUpdatedEventArgs e)
		{
			Int32? NewID = (Int32?)cache.GetValue<InventoryField>(e.Row);
			if (NewID != null)
			{
				InventoryItem obj = (InventoryItem)PXSelectorAttribute.Select<InventoryField>(cache, e.Row, NewID);
				Int32? workgroup = obj?.ProductWorkgroupID;
				if (workgroup != null)
				{
					EntityWorkgroup wg = (EntityWorkgroup)this.Cache.CreateInstance();
					wg.WorkGroupID = workgroup;
					wg = this.Insert(wg);
				}
			}
		}
	}
}
