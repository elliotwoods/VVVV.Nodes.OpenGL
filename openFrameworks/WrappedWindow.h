// oF-vvvv.h

#pragma once

#include "Nodes.h"
#include "ofxVVVV.h"

using namespace System;
using namespace VVVV::PluginInterfaces::V1;
using namespace VVVV::Utils::VColor;
using namespace VVVV::Utils::VMath;
using namespace VVVV::Nodes::OpenGL;

namespace VVVV {
	namespace Nodes {
		namespace OpenFrameworks {
			public ref class WrappedWindow
			{
			public:
				WrappedWindow();
				virtual ~WrappedWindow();

				void setWindowPosition(int windowX, int windowY);
				void setWindowSize(int windowWidth, int windowHeight);
				void setScreenSize(int screenWidth, int screenHeight);

				void bind();
			protected:
				ofxVVVV::Window * window;
				ofPtr<ofAppBaseWindow> * windowPointer;
			};
		}
	}
}