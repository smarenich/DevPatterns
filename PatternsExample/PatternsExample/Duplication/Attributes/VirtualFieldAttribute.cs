using System;
using PX.Data;
using PX.Common;
using PX.Objects.CR;
using PX.Objects.CS;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns
{
	#region VirtualFieldAttribute
	public abstract class VirtualFieldAttribute : PXEventSubscriberAttribute, IPXFieldUpdatedSubscriber, IPXRowSelectingSubscriber, IPXRowSelectedSubscriber
	{
		public Boolean EvaluateOnRowSelected { get; set; }
		protected Type _MasterField;

		public VirtualFieldAttribute(Type masterField)
		{
			if (masterField == null) throw new ArgumentException("masterField");

			_MasterField = masterField;
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

			if (refNbr == null && EvaluateOnRowSelected)
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
	public class AttributeRefAttribute : VirtualFieldAttribute
	{
		protected String _AttrName;
		protected Type _MasterNote;

		public AttributeRefAttribute(Type masterField, Type masterNote, String attrName)
			: base(masterField)
		{
			if (attrName == null) throw new ArgumentException("attrName");
			if (masterNote == null) throw new ArgumentException("masterNote");

			_MasterNote = masterNote;
			_AttrName = attrName;
		}

		protected override object SelectValue(PXCache sender, object row)
		{
			Object master = PXSelectorAttribute.Select(sender, row, _MasterField.Name);
			
			if (master != null)
			{
				CSAnswers answer = (CSAnswers)PXSelect<CSAnswers,
					Where<CSAnswers.attributeID, Equal<Required<CSAnswers.attributeID>>,
						And<CSAnswers.refNoteID, Equal<Required<CSAnswers.refNoteID>>>>>
					.Select(sender.Graph, _AttrName, PXNoteAttribute.GetNoteID(sender.Graph.Caches[BqlCommand.GetItemType(_MasterNote)], master, _MasterNote.Name));

				if (answer != null)
				{
					return answer.Value;
				}
			}

			return null;
		}
	}
	#endregion
}
