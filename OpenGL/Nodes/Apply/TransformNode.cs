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
    [PluginInfo(Name = "Transform", Category = "OpenGL", Version = "Transform", Help = "Apply a VVVV transform to an OpenGL matrix", Tags = "")]
    #endregion PluginInfo
    public class TransformNode : IPluginEvaluate
    {
        public enum LoadOrMultiply { Multiply, Load };

        class TransformApply : IApplyable
        {
            Matrix4d Matrix;
            MatrixMode Mode;
            LoadOrMultiply LoadOrMultiply;

            public TransformApply(Matrix4x4 Matrix, MatrixMode Mode, LoadOrMultiply LoadOrMultiply)
            {
                this.Matrix = UMath.ToGL(Matrix);
                this.Mode = Mode;
                this.LoadOrMultiply = LoadOrMultiply;
            }

            public void Push(DrawArguments a)
            {
                GL.MatrixMode(this.Mode);
                GL.PushMatrix();
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

        [Input("Input")]
        IDiffSpread<Matrix4x4> FPinInInput;

        [Input("Type", DefaultEnumEntry = "Modelview")]
        IDiffSpread<MatrixMode> FPinInMatrixMode;

        [Input("Method", DefaultEnumEntry = "Multiply")]
        IDiffSpread<LoadOrMultiply> FPinInLoadOrMultiply;

        [Output("Action")]
        ISpread<IApplyable> FPinOutAction;

        public void Evaluate(int SpreadMax)
        {
            if (FPinInInput.IsChanged || FPinInMatrixMode.IsChanged || FPinInLoadOrMultiply.IsChanged)
            {
                FPinOutAction.SliceCount = SpreadMax;
                for (int i = 0; i < SpreadMax; i++)
                {
                    FPinOutAction[i] = new TransformApply(FPinInInput[i], FPinInMatrixMode[i], FPinInLoadOrMultiply[i]);
                }
            }
        }
    }
}
