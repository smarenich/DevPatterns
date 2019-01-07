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

namespace Patterns
{
	public class VirtualDAC : PX.Data.IBqlTable
	{
		#region Code
		public abstract class code : PX.Data.IBqlField { }
		[PXDBString(10, IsUnicode = true, IsFixed = false, IsKey = true)]
		[PXUIField(DisplayName = "Code")]
		public virtual string Code { get; set; }
		#endregion
		#region Description
		public abstract class description : PX.Data.IBqlField { }
		[PXDBString(100)]
		[PXUIField(DisplayName = "Description")]
		public virtual string Description { get; set; }
		#endregion
	}
}
