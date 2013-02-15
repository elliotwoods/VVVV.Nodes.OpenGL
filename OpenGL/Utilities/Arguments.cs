using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VVVV.Nodes.OpenGL
{
	public class DrawArguments
	{
		public StereoVisibility Eye;

		public DrawArguments(StereoVisibility Eye)
		{
			this.Eye = Eye;
		}
	}
}