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
using VVVV.Core.Logging;
using System.ComponentModel.Composition;
using OpenTK.Graphics;
using OpenTK.Input;
using System.Threading;
using System.Diagnostics;

namespace VVVV.Nodes.OpenGL
{
	#region PluginInfo
	[PluginInfo(Name = "Renderer", Category = "OpenGL", Version = "Fast", Help = "Render external to VVVV window. Warning: fast renderers do not support mouse events", Tags = "", AutoEvaluate=true)]
	#endregion PluginInfo
	public class RendererFastNode : IPluginEvaluate, IDisposable
	{
		#region fields & pins
		[Input("Input")]
		ISpread<ILayer> FPinInLayer;

		[Input("Background", DefaultColor = new double[] { 0, 0, 0, 1 }, IsSingle = true)]
		IDiffSpread<RGBAColor> FPinInBackground;

        [Input("Clear", IsSingle = true, DefaultValue=1)]
        IDiffSpread<bool> FPinInClear;

		[Input("Fullscreen", IsSingle = true)]
		IDiffSpread<bool> FPinInFullscreen;

		[Input("Mode", IsSingle = true, Visibility = PinVisibility.Hidden)]
		IDiffSpread<GraphicsMode> FPinInGraphicsMode;

		[Input("Version", IsSingle = true, Visibility = PinVisibility.OnlyInspector)]
		IDiffSpread<OpenGLVersion> FPinInOpenGLVersion;

		[Input("Width", IsSingle=true, DefaultValue=800)]
		IDiffSpread<int> FPinInWidth;

		[Input("Height", IsSingle = true, DefaultValue=600)]
		IDiffSpread<int> FPinInHeight;

		[Import()]
		ILogger FLogger;

		SharedContext FSharedContext = new SharedContext();
		GameWindow FWindow = null;
		bool FBackgroundChange = false;

		#endregion fields & pins

		[ImportingConstructor]
		RendererFastNode()
		{
			
		}

		public void Evaluate(int SpreadMax)
		{
			if (FPinInBackground.IsChanged)
				FBackgroundChange = true;

			bool FullscreenResChanged = false;

			if (FPinInWidth.IsChanged || FPinInHeight.IsChanged)
			{
				if (FPinInFullscreen[0])
				{
					FullscreenResChanged = true;
				}
				else
				{
					if (FWindow != null)
					{
						FWindow.Width = FPinInWidth[0];
						FWindow.Height = FPinInHeight[0];
					}
				}
			}

			if (FPinInFullscreen.IsChanged || FPinInGraphicsMode.IsChanged || FPinInOpenGLVersion.IsChanged || FullscreenResChanged)
				Start();

			Render();
		}

		void Start()
		{
			Stop();

			var Mode = FPinInGraphicsMode[0] == null ? new GraphicsMode(new ColorFormat(8, 8, 8, 8)) : FPinInGraphicsMode[0];
			int Version = FPinInOpenGLVersion[0] == OpenGLVersion.OpenGL2 ? 2 : 3;
			GameWindowFlags Flags = FPinInFullscreen[0] ? GameWindowFlags.Fullscreen : GameWindowFlags.Default;

			FWindow = new GameWindow(FPinInWidth[0], FPinInHeight[0], Mode, "Renderer", Flags, DisplayDevice.Default, Version, 0, GraphicsContextFlags.Default, SharedContext.Context.Context);
            FWindow.Visible = true;
            ContextRegister.Add(FWindow);
		}

        void Stop()
		{
			if (FWindow != null)
			{
                ContextRegister.Remove(FWindow);
				FWindow.Exit();
				FWindow.Dispose();
				FWindow = null;
			}
		}
            
		void Render()
		{
			FWindow.MakeCurrent();
            ContextRegister.BindContext(FWindow.Context);

			GL.Viewport(FWindow.ClientSize);

			if (FBackgroundChange)
			{
				FBackgroundChange = false;
				GL.ClearColor(FPinInBackground[0].Color);
			}

			if (FWindow.Context.GraphicsMode.Stereo)
			{
                GL.DrawBuffer(DrawBufferMode.BackLeft);
                if (FPinInClear[0])
                    GL.Clear(ClearBufferMask.AccumBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
                DrawEye(StereoVisibility.Left);

                GL.DrawBuffer(DrawBufferMode.BackRight);
                if (FPinInClear[0])
                    GL.Clear(ClearBufferMask.AccumBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit); 
                DrawEye(StereoVisibility.Right);
			}
			else
			{
                GL.DrawBuffer(DrawBufferMode.Back);
                if (FPinInClear[0])
                    GL.Clear(ClearBufferMask.AccumBufferBit | ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit); 
                DrawEye(StereoVisibility.Both);
			}

			FWindow.SwapBuffers();
		}

        void DrawEye(StereoVisibility EyeSelection)
        {
            foreach (var Layer in FPinInLayer)
                if (Layer != null)
                {
                    Layer.Draw(new DrawArguments(EyeSelection));
                }
        }

		public void Dispose()
		{
			Stop();
		}
	}
}
