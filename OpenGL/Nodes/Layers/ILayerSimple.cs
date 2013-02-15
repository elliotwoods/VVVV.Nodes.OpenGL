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
using System.Drawing;

namespace VVVV.Nodes.OpenGL
{
    /// <summary>
    /// This abstract class performs most functions for a basic layer including world transform, color, texture and stereo selection
    /// </summary>
	public abstract class ILayerSimple : ILayerNode
	{
		#region pins and fields
		[Input("Transform")]
		ISpread<Matrix4x4> FPinInTransform;

		[Input("Color", DefaultColor= new double[]{1, 1, 1, 1})]
		IDiffSpread<RGBAColor> FPinInColor;

		[Input("Texture")]
		IDiffSpread<Texture> FPinInTexture;

        [Input("Stereo Visibility", Visibility=PinVisibility.OnlyInspector)]
        ISpread<StereoVisibility> FPinInStereoVisibility;
		#endregion

		/// <summary>
		/// Override this with your per-slice draw function
		/// </summary>
		/// <param name="SliceIndex"></param>
		protected abstract void DrawSlice(int SliceIndex);
	
		public override void Draw(DrawArguments a)
		{
			Matrix4d mat;
			Color col;

			for (int i = 0; i < SpreadMax; i++)
			{
                if (a.Eye == StereoVisibility.Both || FPinInStereoVisibility[i] == StereoVisibility.Both || a.Eye == FPinInStereoVisibility[i])
                {
                    //apply matrix
                    GL.PushMatrix();
                    mat = UMath.ToGL(FPinInTransform[i]);
                    GL.MultMatrix(ref mat);

                    //apply color
                    col = FPinInColor[i].Color;
                    GL.Color4(col);

                    //apply texture
                    bool textureBound = false;
                    if (FPinInTexture[i] != null)
                    {
                        FPinInTexture[i].Bind();
                        textureBound = true;
                    }

                    //
                    //draw the slice
                    DrawSlice(i);
                    //
                    //

                    //release texture
                    if (textureBound)
                        FPinInTexture[i].Unbind();

                    //release matrix
                    GL.PopMatrix();
                }
			}
		}
	}
}
