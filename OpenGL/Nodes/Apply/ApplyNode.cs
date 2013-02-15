using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;
using VVVV.Utils.VColor;
using OpenTK.Graphics.OpenGL;
using VVVV.Nodes.OpenGL.Utilities;
using OpenTK;

namespace VVVV.Nodes.OpenGL
{
	#region PluginInfo
	[PluginInfo(Name = "Apply", Category = "OpenGL", Version="Apply", Help = "Apply an IState to input layers", Tags = "")]
	#endregion PluginInfo
	public class ApplyNode : ILayerNode
	{
		#region fields & pins
		[Input("Layer")]
		ISpread<ILayer> FPinInLayer;

		[Input("Action")]
		ISpread<IApplyable> FPinInState;

		[Input("Enabled", DefaultValue=1)]
		ISpread<bool> FPinInEnabled;

		#endregion fields & pins

		public override void Draw(DrawArguments a)
		{
			for (int i = 0; i < SpreadMax; i++)
			{
				if (FPinInEnabled[i])
					if (FPinInState[i] != null)
						FPinInState[i].Push(a);

				if (FPinInLayer[i] != null)
					FPinInLayer[i].Draw(a);

				if (FPinInEnabled[i])
					if (FPinInState[i] != null)
						FPinInState[i].Pop();
			}
		}

		public override void KeyPress(System.Windows.Forms.KeyPressEventArgs e)
		{
			foreach (var Layer in FPinInLayer)
				Layer.KeyPress(e);
		}

		public override void KeyUp(System.Windows.Forms.KeyEventArgs e)
		{
			foreach (var Layer in FPinInLayer)
				Layer.KeyUp(e);
		}

		public override void MouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			foreach (var Layer in FPinInLayer)
				Layer.MouseDown(e);
		}

		public override void MouseUp(System.Windows.Forms.MouseEventArgs e)
		{
			foreach (var Layer in FPinInLayer)
				Layer.MouseUp(e);
		}

		public override void MouseMove(System.Windows.Forms.MouseEventArgs e)
		{
			foreach (var Layer in FPinInLayer)
				Layer.MouseMove(e);
		}

		public override void MouseDragged(System.Windows.Forms.MouseEventArgs e)
		{
			foreach (var Layer in FPinInLayer)
				Layer.MouseDragged(e);
		}
	}
}
