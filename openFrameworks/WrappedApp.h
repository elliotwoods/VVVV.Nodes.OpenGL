// oF-vvvv.h

#pragma once

#include "Nodes.h"
#include "ofxVVVV.h"
#include "glut.h"
#include <set>

using namespace System;
using namespace VVVV::PluginInterfaces::V1;
using namespace VVVV::Utils::VColor;
using namespace VVVV::Utils::VMath;
using namespace VVVV::Nodes::OpenGL;

namespace VVVV {
	namespace Nodes {
		namespace OpenFrameworks {

			public ref class WrappedApp
			{
			public:
				WrappedApp(string name, ofxVVVV::Node *);
				virtual ~WrappedApp();

				void setup();
				void update();
				void draw();
				
				void keyPressed(int key);
				void keyReleased(int key);
				void mouseMoved(int x, int y);
				void mouseDragged(int x, int y, int button);
				void mousePressed(int x, int y, int button);
				void mouseReleased(int x, int y, int button);

			protected:
				IPluginHost ^ FHost;
				ofxVVVV::Node * node;
				String^ name;
				bool isSetup;
				static bool isInitialised = false;
			private:
				//static std::set<ofPtr<ofAppBaseWindow> *> preparedWindows;
			};
	
			public ref class ofSetupScreenNode : public WrappedApp {
			public:
				ofSetupScreenNode() : 
					WrappedApp("ofSetupScreen", new CNodes::ofSetupScreenNode()) { };
			};

			public ref class ofLineNode : public WrappedApp {
			public:
				ofLineNode() : 
					WrappedApp("ofLine", new CNodes::ofLineNode()) { };
			};

			public ref class ofDrawBitmapStringNode : public WrappedApp {
			public:
				ofDrawBitmapStringNode() :
					WrappedApp("ofDrawBitmapString", new CNodes::ofDrawBitmapStringNode()) { };
			};

			public ref class GraphicsExampleNode : public WrappedApp {
			public:
				GraphicsExampleNode() :
					WrappedApp("graphicsExample", new CNodes::GraphicsExampleNode()) { };
			};

			public ref class EasyCamExampleNode : public WrappedApp {
			public:
				EasyCamExampleNode() :
					WrappedApp("easyCamExample", new CNodes::EasyCamExampleNode()) { };
			};

			public ref class OpenNIExampleNode : public WrappedApp {
			public:
				OpenNIExampleNode() :
					WrappedApp("ofxOpenNIExampleNode", new CNodes::OpenNIExampleNode()) { };
			};

			public ref class VideoExampleNode : public WrappedApp {
			public:
				VideoExampleNode() :
					WrappedApp("VideoExampleNode", new CNodes::VideoExampleNode()) { };
			};
		}
	}
}
