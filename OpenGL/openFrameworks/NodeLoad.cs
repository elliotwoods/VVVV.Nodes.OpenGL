using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;
using VVVV.Utils.VColor;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using VVVV.Utils.Streams;

using VVVV.Nodes.OpenGL;
using VVVV.Nodes.OpenGL.Utilities;
using VVVV.Core.Logging;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace VVVV.Nodes.OpenGL.openFrameworks
{
    class NodeFactorySet : IDisposable
    {
    
        Dictionary<string, NodeFactory> FFactories = new Dictionary<string, NodeFactory>();

        public NodeFactory GetFactory(string DllFilename)
        {
            if (!FFactories.ContainsKey(DllFilename))
            {
                FFactories.Add(DllFilename, new NodeFactory(DllFilename));
			}
			else if (FFactories[DllFilename].Loaded == false)
			{
				FFactories[DllFilename].Load();
			}
            return FFactories[DllFilename];
        }

        public void Dispose()
        {
            foreach(var factory in FFactories.Values)
            {
                factory.Dispose();
            }
			FFactories.Clear();
        }
	}

	#region PluginInfo
	[PluginInfo(Name = "Test", Category = "openFrameworks", Help = "Load an ofxVVVV Node", Tags = "")]
	#endregion PluginInfo
	public class nodjsdp : IPluginEvaluate
	{
		public void Evaluate(int SpreadMax)
		{
		}
	}

    #region PluginInfo
    [PluginInfo(Name = "NodeLoad", Category = "openFrameworks", Help = "Load an ofxVVVV Node", Tags = "")]
    #endregion PluginInfo
	public class NodeLoad : ILayerNode, IDisposable, IPartImportsSatisfiedNotification
	{
        static NodeFactorySet FFactories = new NodeFactorySet();

		NodeInstance FNodeInstance = null;
		NodeFactory FFactory = null;

		bool FLoaded = false;
        int FHandle = -1;

		#region fields & pins
		// A spread which contains our inputs
		Spread<IIOContainer<ISpread<double>>> FInputs = new Spread<IIOContainer<ISpread<double>>>();

		// A spread which contains our outputs
		Spread<IIOContainer<ISpread<double>>> FOutputs = new Spread<IIOContainer<ISpread<double>>>();

        [Config("Filename", StringType=StringType.Filename, IsSingle=true)]
        IDiffSpread<string> FInFilename;

		[Config("Auto Reload", IsSingle = true, DefaultBoolean = true)]
		ISpread<bool> FInAutoReload;

		[Config("Reload", IsSingle = true, IsBang = true)]
		ISpread<bool> FInReload;

		[Output("On Reload", Order = Int32.MaxValue-1, IsBang=true)]
		ISpread<bool> FOutOnReload;

        [Output("Status", Order = Int32.MaxValue)]
        ISpread<string> FOutStatus;

		[Import()]
		ILogger FLogger;

		[Import()]
		IIOFactory FIOFactory;
		#endregion fields & pins

		public void OnImportsSatisfied()
		{
			FInFilename.Changed += new SpreadChangedEventHander<string>(FInFilename_Changed);
		}

		void FInFilename_Changed(IDiffSpread<string> spread)
		{
			try
			{
				if (FLoaded)
					Unload();

				FFactory = FFactories.GetFactory(FInFilename[0]);
				FNodeInstance = FFactory.NewNode();
				RecreatePins();

				FLoaded = true;
				FOutStatus[0] = "OK";
			}
			catch (Exception e)
			{
				FOutStatus[0] = e.Message;
			}
		}

		private void HandlePinCountChanged<T>(int count, Spread<IIOContainer<T>> pinSpread, Func<int, IOAttribute> ioAttributeFactory) where T : class
		{
			pinSpread.ResizeAndDispose(
				count,
				(i) =>
				{
					var ioAttribute = ioAttributeFactory(i + 1);
					return FIOFactory.CreateIOContainer<T>(ioAttribute);
				}
			);
		}

		void RecreatePins()
		{
			HandlePinCountChanged(this.FNodeInstance.InputPinCount, FInputs, (i) => new InputAttribute(string.Format("Input {0}", i)));
			HandlePinCountChanged(this.FNodeInstance.OutputPinCount, FOutputs, (i) => new OutputAttribute(string.Format("Output {0}", i)));
		}

        public override void Update()
        {
			if (FInReload[0] && FFactory is NodeFactory)
			{
				FFactory.Load();
				FOutOnReload[0] = true;
			}
			else
			{
				FOutOnReload[0] = false;
			}

			if (FInAutoReload[0])
				if (FFactory is NodeFactory)
					FOutOnReload[0] = FFactory.AutoReloadCheck();

			if (FOutOnReload[0])
				RecreatePins();

			if (FLoaded)
			{
				for (int i = 0; i < FNodeInstance.InputPinCount; i++)
					FNodeInstance.SetInputs(FInputs);
				FNodeInstance.Update();
				FNodeInstance.GetOutputs(FOutputs);
			}
        }

		void Unload()
		{
			if (FLoaded)
				FFactory.Unlink(FNodeInstance);
		}

		public override void Draw(DrawArguments a)
        {
			if (FLoaded)
			{
				FNodeInstance.Draw();
			}
        }

		public override void KeyPress(System.Windows.Forms.KeyPressEventArgs e)
        {
        }

		public override void KeyUp(System.Windows.Forms.KeyEventArgs e)
        {
        }

		public override void MouseDown(System.Windows.Forms.MouseEventArgs e)
        {
        }

		public override void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
        }

		public override void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
        }

		public override void MouseDragged(System.Windows.Forms.MouseEventArgs e)
        {
        }

        public void Dispose()
        {
			Unload();
        }
	}
}
