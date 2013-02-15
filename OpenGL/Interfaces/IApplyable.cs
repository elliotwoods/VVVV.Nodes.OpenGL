#region usings
using System;
using System.ComponentModel.Composition;
using System.Drawing;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Core.Logging;

using System.Collections.Generic;

#endregion usings

namespace VVVV.Nodes.OpenGL
{
	public interface IApplyable
	{
		void Push(DrawArguments a);
		void Pop();
	}

	public abstract class IApplyableNode : IPluginEvaluate
	{
		public class ApplyFromThisNode : IApplyable
		{
			IApplyableNode Node;
			int SliceIndex;

			public ApplyFromThisNode(IApplyableNode Node, int SliceIndex)
			{
				this.Node = Node;
				this.SliceIndex = SliceIndex;
			}

			public void Push(DrawArguments a)
			{
				Node.Push(SliceIndex, a);
			}

			public void Pop()
			{
				Node.Pop(SliceIndex);
			}
		}

		#region fields & pins
		[Output("Action")]
		ISpread<IApplyable> FOutAction;

		[Import]
		ILogger FLogger;

		#endregion fields & pins

		[ImportingConstructor]
		public IApplyableNode()
		{

		}

		public virtual void Evaluate(int SpreadMax)
		{
			if (SpreadMax != FOutAction.SliceCount)
				UpdateOutput(SpreadMax);
		}

		protected void UpdateOutput(int SpreadMax)
		{
			FOutAction.SliceCount = SpreadMax;
			for (int i = 0; i < SpreadMax; i++)
			{
				FOutAction[i] = new ApplyFromThisNode(this, i);
			}
		}

		public abstract void Push(int SliceIndex, DrawArguments a);
		public abstract void Pop(int SliceIndex);
	}
}