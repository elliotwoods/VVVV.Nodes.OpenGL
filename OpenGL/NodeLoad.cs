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

namespace VVVV.Nodes.OpenGL
{
    class NodeFactorySet : IDisposable
    {
        [DllImport("kernel32.dll")]
        internal static extern IntPtr LoadLibrary(String dllname);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, String procname);

        public class NodeFactory : IDisposable
        {
			public class NodeInstance : IDisposable
			{
				public NodeInstance(NodeFactory Factory)
				{
					this.Factory = Factory;
					Create();
				}

				public int Handle {get; private set;}
				public NodeFactory Factory {get; private set;}
				public bool Loaded { get; private set; }

				public void Dispose()
				{
					Factory.Destroy(this.Handle);
				}

				public void Setup()
				{
					if (Factory.Loaded)
						Factory.Setup(this.Handle);
				}

				public void Update()
				{
					if (Factory.Loaded)
						Factory.Update(this.Handle);
				}

				public void Draw()
				{
					if (Loaded)
						Factory.Draw(this.Handle);
				}

				public void Create()
				{
					if (Factory.Loaded)
					{
						try
						{
							this.Handle = Factory.Create();
							this.Loaded = true;
						}
						catch (Exception e)
						{
							this.Loaded = false;
							throw (e);
						}
					}
					else
					{
						this.Loaded = false;
					}
				}

				public List<Spread<double>> Outputs
				{
					get
					{
						List<Spread<Double>> Outputs = new List<Spread<double>>();
						var outputCount = Factory.GetOutputCount(this.Handle);
						for (int i = 0; i < outputCount; i++)
						{
							int slicecount = Factory.GetOutputSliceCount(this.Handle, i);
							var output = new Spread<double>(slicecount);
							Outputs.Add(output);
							for (int j = 0; j < slicecount; j++)
							{
								Outputs[i][j] = Factory.GetOutputValue(this.Handle, i, j);
							}
						}
						return Outputs;
					}
				}

				public void SetInputs(Spread<IIOContainer<ISpread<double>>> Inputs)
				{
					if (!Factory.Loaded)
						return;

					int pinCount = Factory.GetInputCount(this.Handle);
					for (int i = 0; i < pinCount; i++)
					{
						var sliceCount = Inputs[i].IOObject.SliceCount;
						Factory.SetInputSliceCount(this.Handle, i, sliceCount);
						for (int j = 0; j < sliceCount; j++)
						{
							Factory.SetInputValue(this.Handle, i, j, Inputs[i].IOObject[j]);
						}
					}
				}

				public int InputPinCount
				{
					get
					{
						if (Factory.Loaded)
							return Factory.GetInputCount(this.Handle);
						else
							return 0;
					}
				}

				public int OutputPinCount
				{
					get
					{
						if (Factory.Loaded)
							return Factory.GetOutputCount(this.Handle);
						else
							return 0;
					}
				}
			}

            IntPtr FLibrary;
			public NodeSetDataPath SetDataPath;
			public NodeCreateDelegate Create;
			public NodeArgumentlessDelegate Destroy;
			public NodeArgumentlessDelegate Setup;
			public NodeArgumentlessDelegate Update;
			public NodeArgumentlessDelegate Draw;
			public NodeCountPinsDelegate GetInputCount;
			public NodeCountPinsDelegate GetOutputCount;
			public NodeSetSpreadSliceCountDelegate SetInputSliceCount;
			public NodeSetSpreadValueDelegate SetInputValue;
			public NodeGetSpreadSliceCountDelegate GetOutputSliceCount;
			public NodeGetSpreadValueDelegate GetOutputValue;

			List<NodeInstance> FInstances = new List<NodeInstance>();

            bool FLoaded = false;
			string FFilename = "";
			string FTemporaryFilename = "";
			DateTime FLastWrite;

            public NodeFactory(string DllFilename)
            {
				FFilename = DllFilename;
				Load();
            }

			void WaitForAccess(int iterations)
			{
				try
				{
					System.Threading.Thread.Sleep(1000);
					System.IO.File.Open(FFilename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
				}
				catch
				{
					iterations++;
					if (iterations > 1000)
						throw (new Exception("Cannot open " + FFilename + " for exclusive access"));
					else
						WaitForAccess(iterations);
				}
			}
			unsafe public void Load()
			{
				Unload();

				try
				{
					if (!File.Exists(FFilename))
						throw (new Exception("File not found"));

					FTemporaryFilename = System.IO.Path.GetTempPath() + Path.GetFileName(FFilename);
					if (File.Exists(FTemporaryFilename))
					{
						try
						{
							File.Delete(FTemporaryFilename);
						}
						catch
						{
							int index = 0;
							string noExtension = FTemporaryFilename.Substring(0, FTemporaryFilename.Length - 4);

							while (File.Exists(FTemporaryFilename))
							{
								FTemporaryFilename = noExtension + "~" + index.ToString() + ".dll";
							}
						}
					}

					System.Threading.Thread.Sleep(1000);
					//WaitForAccess(0);

					File.Copy(FFilename, FTemporaryFilename);
					FLastWrite = File.GetLastWriteTime(FFilename);

					FLibrary = LoadLibrary(FTemporaryFilename);
					SetDataPath = (NodeSetDataPath)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "SetDataPath"), typeof(NodeSetDataPath));
					Create = (NodeCreateDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "NodeCreate"), typeof(NodeCreateDelegate));
					Destroy = (NodeArgumentlessDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "NodeDestroy"), typeof(NodeArgumentlessDelegate));
					Setup = (NodeArgumentlessDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "NodeSetup"), typeof(NodeArgumentlessDelegate));
					Update = (NodeArgumentlessDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "NodeUpdate"), typeof(NodeArgumentlessDelegate));
					Draw = (NodeArgumentlessDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "NodeDraw"), typeof(NodeArgumentlessDelegate));
					GetInputCount = (NodeCountPinsDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "NodeInputCount"), typeof(NodeCountPinsDelegate));
					GetOutputCount = (NodeCountPinsDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "NodeOutputCount"), typeof(NodeCountPinsDelegate));
					SetInputSliceCount = (NodeSetSpreadSliceCountDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "NodeSetInputSliceCount"), typeof(NodeSetSpreadSliceCountDelegate));
					SetInputValue = (NodeSetSpreadValueDelegate)Marshal.GetDelegateForFunctionPointer(GetProcAddress(FLibrary, "NodeSetInputValue"), typeof(NodeSetSpreadValueDelegate));

					var dataPath = Path.GetDirectoryName(FFilename).ToCharArray();
					fixed (char * pathAsChar = &dataPath[0])
					    SetDataPath(pathAsChar, dataPath.Length);

					this.FLoaded = true;

					foreach (var instance in FInstances)
					{
						instance.Create();
						instance.Setup();
					}
				}
				catch (Exception e)
				{
					this.FLoaded = false;
					throw (e);
				}
			}

			void Unload()
			{
				if (this.FLoaded)
				{
					FreeLibrary(FLibrary);
					File.Delete(FTemporaryFilename);
				}

				FLoaded = false;
			}

			public bool Loaded
			{
				get
				{
					return this.FLoaded;
				}
			}

			public void CheckFileUpdates()
			{
				if (FLoaded)
				{
					if (File.GetLastWriteTime(FFilename) != FLastWrite)
						Load();
				}
			}

			public NodeInstance NewNode()
			{
				var instance = new NodeInstance(this);
				this.FInstances.Add(instance);
				return instance;
			}
        
			public void Dispose()
			{
				Unload();
			}
		}

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

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	unsafe internal delegate int NodeSetDataPath(char* path, int length);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate int NodeCreateDelegate();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void NodeArgumentlessDelegate(int handle);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate int NodeCountPinsDelegate(int handle);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void NodeSetSpreadSliceCountDelegate(int handle, int PinIndex, int SliceCount);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate void NodeSetSpreadValueDelegate(int handle, int PinIndex, int SliceIndex, double Value);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate int NodeGetSpreadSliceCountDelegate(int handle, int PinIndex);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate double NodeGetSpreadValueDelegate(int handle, int PinIndex, int SliceIndex);

    #region PluginInfo
    [PluginInfo(Name = "NodeLoad", Category = "openFrameworks", Help = "Load an ofxVVVV Node", Tags = "")]
    #endregion PluginInfo
	public class NodeLoad : ILayerNode, IDisposable, IPartImportsSatisfiedNotification
	{
        static NodeFactorySet FFactories = new NodeFactorySet();

		NodeFactorySet.NodeFactory.NodeInstance FNodeInstance = null;
		NodeFactorySet.NodeFactory FFactory = null;

		bool FLoaded = false;
        bool FIsSetup = false;
		string FFilename;
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


        [Output("Status", Order = Int32.MaxValue-1)]
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

				//FInputs.ResizeAndDispose(this.FNodeInstance.InputPinCount, i =>
				//{
				//    var ioAttribute = ioAttributeFactory(i + 1);
				//    return FIOFactory.CreateIOContainer(ioAttribute);
				//});

				FLoaded = true;
				FIsSetup = false;
				FOutStatus[0] = "OK";
			}
			catch (Exception e)
			{
				FOutStatus[0] = e.Message;
			}
		}

        public override void Update()
        {
			if (FInAutoReload[0] && FFactory is NodeFactorySet.NodeFactory)
				FFactory.CheckFileUpdates();

			FNodeInstance.Update();
        }

		void Unload()
		{
			FFactory.Unload(FNodeInstance);
		}

		public override void Draw(DrawArguments a)
        {
			if (FLoaded && FIsSetup)
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
