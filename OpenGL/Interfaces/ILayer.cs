#region usings
using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Windows.Forms;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Core.Logging;

using System.Collections.Generic;

#endregion usings

namespace VVVV.Nodes.OpenGL
{
	public interface ILayer
	{
		/// <summary>
		/// This update call is performed once per frame
		/// </summary>
		void Update();

		/// <summary>
		/// This draw call is performed once per device.
		/// </summary>
		void Draw(DrawArguments a);

		/// <summary>
		/// This is called whilst a key is pressed
		/// </summary>
		void KeyPress(KeyPressEventArgs e);

		/// <summary>
		/// This is called whenever a key is released
		/// </summary>
		void KeyUp(KeyEventArgs e);

		/// <summary>
		/// This is called whenever a mouse is pressed
		/// </summary>
		void MouseDown(System.Windows.Forms.MouseEventArgs e);

		/// <summary>
		/// This is called whenever a mouse is released
		/// </summary>
		void MouseUp(System.Windows.Forms.MouseEventArgs e);

		/// <summary>
		/// This is called whenever a mouse is moved
		/// </summary>
		void MouseMove(System.Windows.Forms.MouseEventArgs e);

		/// <summary>
		/// This is called whenever a mouse is moved whilst a button is pressed
		/// </summary>
		void MouseDragged(System.Windows.Forms.MouseEventArgs e);
	}

	public abstract class ILayerNode : IPluginEvaluate, ILayer, IDisposable
	{
		#region fields & pins
		[Output("Layer")]
		ISpread<ILayer> FPinOutLayer;

		[Import]
		ILogger FLogger;

		SharedContext FSharedContext = new SharedContext();
		#endregion fields & pins

		[ImportingConstructor]
		public ILayerNode()
		{

		}

		protected int SpreadMax = 0;

		private bool FFirstRun = true;
		public void Evaluate(int SpreadMax)
		{
			this.SpreadMax = SpreadMax;

			if (FFirstRun)
			{
				FPinOutLayer[0] = this;
				FFirstRun = false;
			}

			Update();
		}

		public virtual void Update() { }
        public virtual void Draw(DrawArguments a) { }
		public virtual void KeyPress(KeyPressEventArgs e) { }
		public virtual void KeyUp(KeyEventArgs e) { }
		public virtual void MouseDown(System.Windows.Forms.MouseEventArgs e) { }
		public virtual void MouseUp(System.Windows.Forms.MouseEventArgs e) { }
		public virtual void MouseMove(System.Windows.Forms.MouseEventArgs e) { }
		public virtual void MouseDragged(System.Windows.Forms.MouseEventArgs e) { }
 
		public void Dispose()
		{
			//throw new NotImplementedException();
		}
	}
}