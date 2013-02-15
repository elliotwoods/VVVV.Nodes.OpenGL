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
using OpenTK.Graphics;
using VVVV.PluginInterfaces.V2.NonGeneric;

namespace VVVV.Nodes.OpenGL
{
	#region PluginInfo
	[PluginInfo(Name = "+", Category = "OpenGL", Version = "Spectral", Help = "Combine many draw slices into a single slice", Tags = "")]
	#endregion PluginInfo
	public class AddSpectralNode : IPluginEvaluate
	{
		class CombinedLayer : ILayer
		{
			IEnumerable<ILayer> Layers;

			public CombinedLayer(IEnumerable<ILayer> Layers)
			{
				this.Layers = Layers;
			}

			public void Update()
			{
				foreach (var Layer in Layers)
					Layer.Update();
			}

			public void Draw(DrawArguments a)
			{
				foreach (var Layer in Layers)
					Layer.Draw(a);
			}

			public void KeyPress(System.Windows.Forms.KeyPressEventArgs e)
			{
				foreach (var Layer in Layers)
					Layer.KeyPress(e);
			}

			public void KeyUp(System.Windows.Forms.KeyEventArgs e)
			{
				foreach (var Layer in Layers)
					Layer.KeyUp(e);
			}

			public void MouseDown(System.Windows.Forms.MouseEventArgs e)
			{
				foreach (var Layer in Layers)
					Layer.MouseDown(e);
			}

			public void MouseUp(System.Windows.Forms.MouseEventArgs e)
			{
				foreach (var Layer in Layers)
					Layer.MouseUp(e);
			}

			public void MouseMove(System.Windows.Forms.MouseEventArgs e)
			{
				foreach (var Layer in Layers)
					Layer.MouseMove(e);
			}

			public void MouseDragged(System.Windows.Forms.MouseEventArgs e)
			{
				foreach (var Layer in Layers)
					Layer.MouseDragged(e);
			}
		}

		#region pins
		[Input("Input")]
		IDiffSpread<ISpread<ILayer>> FInInput;

		[Output("Output")]
		ISpread<ILayer> FOutOutput;
		#endregion

		bool FFirstRun = true;
		public void Evaluate(int SpreadMax)
		{
			if (FInInput.IsChanged)
			{
				FOutOutput.SliceCount = FInInput.SliceCount;

				for (int i = 0; i < FInInput.SliceCount; i++)
					FOutOutput[i] = new CombinedLayer(FInInput[i]);
			}
		}
	}
}
