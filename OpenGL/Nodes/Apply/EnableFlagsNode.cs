using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVVV.PluginInterfaces.V2;
using OpenTK.Graphics.OpenGL;

namespace VVVV.Nodes.OpenGL
{

	#region PluginInfo
	[PluginInfo(Name = "EnableFlags", Category = "OpenGL", Version = "Apply", Help = "Give 'Enable' states as IState's", Tags = "")]
	#endregion PluginInfo
	public class EnableFlagsNode : IApplyableNode
	{
		[Input("Flags", DefaultEnumEntry = "DepthEnable")]
		ISpread<ISpread<EnableCap>> FPinInFlags;

		public override void Push(int SliceIndex, DrawArguments a)
		{
			var Flags = FPinInFlags[SliceIndex];
			GL.PushAttrib(AttribMask.EnableBit);
			foreach (var Flag in Flags)
				GL.Enable(Flag);
		}

		public override void Pop(int SliceIndex)
		{
			GL.PopAttrib();
		}
	}
}
