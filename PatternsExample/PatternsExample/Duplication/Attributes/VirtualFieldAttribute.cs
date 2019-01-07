using System;
using PX.Data;
using PX.Common;
using PX.Objects.CR;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
	#region VirtualFieldAttribute
	public abstract class VirtualFieldAttribute : PXEventSubscriberAttribute, IPXFieldUpdatedSubscriber, IPXRowSelectingSubscriber, IPXRowSelectedSubscriber
	{
		protected Type _MasterField;

		public VirtualFieldAttribute(Type field)
		{
			_MasterField = field;
		}
		public override void CacheAttached(PXCache sender)
		{
			base.CacheAttached(sender);

			sender.Graph.FieldUpdated.AddHandler(sender.GetItemType(), _MasterField.Name, FieldUpdated);
		}

		protected virtual object GetValue(PXCache sender, object row)
		{
			return sender.GetValue(row, _MasterField.Name);
		}
		protected abstract object SelectValue(PXCache sender, Object row);

		public virtual void RowSelecting(PXCache sender, PXRowSelectingEventArgs e)
		{
			Object row = e.Row;
			Object orig = GetValue(sender, row);

			if (row == null || orig == null || e.IsReadOnly)
			{
				sender.SetValue(row, _FieldOrdinal, String.Empty);
			}
			else
			{
				using (new PXConnectionScope())
				{
					Object value = SelectValue(sender, row);

					if (value != null)
					{
						sender.SetValue(row, _FieldOrdinal, value);
					}
				}
			}
		}
		public virtual void RowSelected(PXCache sender, PXRowSelectedEventArgs e)
		{
			Object row = e.Row;
			Object refNbr = sender.GetValue(row, _FieldOrdinal);

			if (refNbr == null)
			{
				Object value = SelectValue(sender, row);

				if (value != null)
				{
					sender.SetValue(row, _FieldOrdinal, value);
				}
			}
		}
		public virtual void FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
		{
			Object row = e.Row;

			Object value = SelectValue(sender, row);

			if (value != null)
			{
				sender.SetValue(row, _FieldOrdinal, value);
			}
		}
	}
	public class VirtualCRMIDAttribute : VirtualFieldAttribute
	{
		public VirtualCRMIDAttribute(Type field)
			: base(field)
		{
		}

		protected override object SelectValue(PXCache sender, object row)
		{
			BAccount customer = PXSelectorAttribute.Select(sender, row, _MasterField.Name) as BAccount;
			if (customer != null)
			{
				return customer.AcctReferenceNbr;
			}

			return null;
		}
	}
	#endregion
}
