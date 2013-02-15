using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VMath;
using VVVV.Utils.VColor;
using System.Drawing;
using System.Drawing.Imaging;

using VVVV.Nodes.OpenGL;
using VVVV.Nodes.OpenGL.Utilities;
using VVVV.Core.Logging;
using System.ComponentModel.Composition;

namespace VVVV.Nodes.OpenFrameworks
{
    #region PluginInfo
    [PluginInfo(Name = "NodeLoad", Category = "openFrameworks", Help = "Load an ofxVVVV::Node", Tags = "")]
    #endregion PluginInfo
	public class Template : IPluginEvaluate
	{
		#region fields & pins
		[Input("Input", DefaultValue = 1.0)]
		ISpread<double> FInput;

		[Output("Output")]
		ISpread<double> FOutput;

		[Import()]
		ILogger FLogger;
		#endregion fields & pins
 
		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;

			for (int i = 0; i < SpreadMax; i++)
				FOutput[i] = FInput[i] * 2;
				 
			//FLogger.Log(LogType.Debug, "hi tty!");
		}
	}
}
