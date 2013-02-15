using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVVV.PluginInterfaces.V2;
using OpenTK.Graphics.OpenGL;
using VVVV.Utils.VMath;
using OpenTK;
using VVVV.Nodes.OpenGL.Utilities;

namespace VVVV.Nodes.OpenGL
{
	#region PluginInfo
	[PluginInfo(Name = "Transform", Category = "OpenGL", Version = "Apply, Stereo", Help = "Apply a VVVV transform to an OpenGL matrix", Tags = "")]
	#endregion PluginInfo
	public class StereoTransformNode : IPluginEvaluate
	{
		public enum LoadOrMultiply { Multiply, Load };

		class StereoTransformApply : IApplyable
		{
            Matrix4d Left;
            Matrix4d Right;
			MatrixMode Mode;
			LoadOrMultiply LoadOrMultiply;

            public StereoTransformApply(Matrix4x4 Left, Matrix4x4 Right, MatrixMode Mode, LoadOrMultiply LoadOrMultiply)
			{
				this.Left = UMath.ToGL(Left);
                this.Right = UMath.ToGL(Right);
				this.Mode = Mode;
				this.LoadOrMultiply = LoadOrMultiply;
			}

			public void Push(DrawArguments a)
			{
                GL.MatrixMode(this.Mode);
                GL.PushMatrix();

                Matrix4d Matrix;
				if (a.Eye == StereoVisibility.Left || a.Eye == StereoVisibility.Both)
					Matrix = this.Left;
				else
					Matrix = this.Right;

				if (this.LoadOrMultiply == LoadOrMultiply.Load)
					GL.LoadMatrix(ref Matrix);
				else
					GL.MultMatrix(ref Matrix);
			}

			public void Pop()
			{
				GL.MatrixMode(this.Mode);
				GL.PopMatrix();
			}
		}

		[Input("Left")]
		IDiffSpread<Matrix4x4> FPinInLeft;

        [Input("Right")]
		IDiffSpread<Matrix4x4> FPinInRight;

		[Input("Type", DefaultEnumEntry = "Modelview")]
		IDiffSpread<MatrixMode> FPinInMatrixMode;

		[Input("Method", DefaultEnumEntry = "Multiply")]
		IDiffSpread<LoadOrMultiply> FPinInLoadOrMultiply;

		[Output("Action")]
		ISpread<IApplyable> FPinOutAction;

		public void Evaluate(int SpreadMax)
		{
			if (FPinInLeft.IsChanged || FPinInRight.IsChanged || FPinInMatrixMode.IsChanged || FPinInLoadOrMultiply.IsChanged)
			{
				FPinOutAction.SliceCount = SpreadMax;
				for (int i = 0; i < SpreadMax; i++)
				{
					FPinOutAction[i] = new StereoTransformApply(FPinInLeft[i], FPinInRight[i], FPinInMatrixMode[i], FPinInLoadOrMultiply[i]);
				}
			}
		}
	}
}
