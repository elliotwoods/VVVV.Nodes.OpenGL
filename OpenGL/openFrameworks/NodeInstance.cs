using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVVV.PluginInterfaces.V2;

namespace VVVV.Nodes.OpenGL.openFrameworks
{
	public class NodeInstance : IDisposable
	{
		public NodeInstance(NodeFactory Factory)
		{
			this.IsSetup = false;
			this.Loaded = false;

			this.Factory = Factory;
			Create();
		}

		public int Handle { get; private set; }
		public NodeFactory Factory { get; private set; }
		public bool Loaded { get; private set; }
		public bool IsSetup { get; private set; }

		public void Dispose()
		{
			if (this.Loaded)
				Factory.Unlink(this);
		}

		public void Destroy()
		{
			if (this.Loaded && Factory.Loaded)
			{
				Factory.Destroy(this.Handle);
			}
			this.Loaded = false;
			this.IsSetup = false;
		}

		public void Setup()
		{
			if (this.Loaded)
			{
				Factory.Setup(this.Handle);
				this.IsSetup = true;
			}
		}

		public void Update()
		{
			if (this.Loaded)
			{
				if (!this.IsSetup)
					this.Setup();
				Factory.Update(this.Handle);
			}
		}

		public void Draw()
		{
			if (Loaded && IsSetup)
				Factory.Draw(this.Handle);
		}

		public void Create()
		{
			if (this.Loaded)
				Factory.Unlink(this);

			if (Factory.Loaded)
			{
				try
				{
					this.Handle = Factory.Create();
					Factory.Register(this);
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

		public void GetOutputs(Spread<IIOContainer<ISpread<double>>> Outputs)
		{
			var outputCount = this.OutputPinCount;
			for (int i = 0; i < outputCount; i++)
			{
				int slicecount = Factory.GetOutputSliceCount(this.Handle, i);
				Outputs[i].IOObject.SliceCount = slicecount;
				for (int j = 0; j < slicecount; j++)
				{
					Outputs[i].IOObject[j] = Factory.GetOutputValue(this.Handle, i, j);
				}
			}
		}

		public void SetInputs(Spread<IIOContainer<ISpread<double>>> Inputs)
		{
			if (!Factory.Loaded)
				return;

			int pinCount = this.InputPinCount;
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
}
