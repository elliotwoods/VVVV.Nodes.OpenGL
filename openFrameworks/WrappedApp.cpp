// This is the main DLL file.

#include "WrappedApp.h"

namespace VVVV {
	namespace Nodes {
		namespace OpenFrameworks {

			WrappedApp::WrappedApp(string name, ofxVVVV::Node * node)
			{
				ofPtr<ofAppBaseWindow> * currentWindow = & ofGetCurrentWindow();
				
				if (!isInitialised) {
					ofSeedRandom();
					ofResetElapsedTimeCounter();
					ofSetCurrentRenderer(ofPtr<ofBaseRenderer>(new ofGLRenderer(false)));
					isInitialised = true;
				}

				//if (WrappedApp::preparedWindows.count(currentWindow) == 0) {
				//	//any code that needs to happen per window / context
				//	WrappedApp::preparedWindows.insert( & ofGetCurrentWindow() );
				//}

				this->name = gcnew String(name.c_str());
				this->node = node;
				this->isSetup = false;
				this->node->mouseX = 0;
				this->node->mouseY = 0;
			}

			WrappedApp::~WrappedApp()
			{
				delete this->node;
			}

			void WrappedApp::setup()
			{
				this->node->setup();
				this->isSetup = true;
			}

			void WrappedApp::update()
			{
				if (this->isSetup)
					this->node->update();
			}

			void WrappedApp::draw()
			{
				if (!this->isSetup) {
					this->setup();
					this->update();
				}

				this->node->draw();
			}

			void WrappedApp::keyPressed(int key)
			{
				if (this->isSetup)
					this->node->keyPressed(key);
			}

			void WrappedApp::keyReleased(int key)
			{
				if (this->isSetup)
					this->node->keyReleased(key);
			}

			void WrappedApp::mouseMoved(int x, int y)
			{
				if (this->isSetup)
					this->node->mouseMoved(x, y);
			}
			
			void WrappedApp::mouseDragged(int x, int y, int button)
			{
				if (this->isSetup)
					this->node->mouseDragged(x, y, button);
			}

			void WrappedApp::mousePressed(int x, int y, int button)
			{
				if (this->isSetup)
					this->node->mousePressed(x, y, button);
			}

			void WrappedApp::mouseReleased(int x, int y, int button)
			{
				if (this->isSetup)
					this->node->mouseReleased(x, y, button);
			}
		}
	}
}