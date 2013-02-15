#include "WrappedWindow.h"

namespace VVVV {
	namespace Nodes {
		namespace OpenFrameworks {
			//---------
			WrappedWindow::WrappedWindow() {
				this->window = new ofxVVVV::Window;
				this->windowPointer = new ofPtr<ofAppBaseWindow>(this->window);
			}

			//---------
			WrappedWindow::~WrappedWindow() {
				delete this->window;
			}

			//---------
			void WrappedWindow::setWindowPosition(int windowX, int windowY) {
				this->window->setWindowPosition(windowX, windowY);
			}

			//---------
			void WrappedWindow::setWindowSize(int windowWidth, int windowHeight) {
				this->window->setWindowSize(windowWidth, windowHeight);
			}

			//---------
			void WrappedWindow::setScreenSize(int screenWidth, int screenHeight) {
				this->window->setScreenSize(screenWidth, screenHeight);
			}

			//---------
			void WrappedWindow::bind() {
				ofSetCurrentWindow(*windowPointer);
			}
		}
	}
}