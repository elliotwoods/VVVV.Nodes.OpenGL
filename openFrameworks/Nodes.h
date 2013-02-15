#pragma once

#include "ofMain.h"
#include "ofxVVVV.h"
#include "ofxOpenNI.h"

namespace CNodes {
	class ofSetupScreenNode : public ofxVVVV::Node {
	public:
		ofSetupScreenNode();
		void update();
		void draw();

		void keyPressed(int key);
		void keyReleased(int key);
		void mouseMoved(int x, int y);
		void mouseDragged(int x, int y, int button);
		void mousePressed(int x, int y, int button);
		void mouseReleased(int x, int y, int button);
	protected:
		ofxVVVV::AutoWindow window;
		ofPtr<ofAppBaseWindow> windowPointer;
	};

	class ofLineNode : public ofxVVVV::Node {
	public:
		void draw();
	};

	class ofDrawBitmapStringNode : public ofxVVVV::Node {
	public:
		void draw();
	};

	class GraphicsExampleNode : public ofxVVVV::Node {
	public:
		void setup();
		void update();
		void draw();

		void keyPressed(int key);
		void mouseMoved( int x, int y );

		float 	counter;
		bool	bSmooth;

		deque<ofPoint> cursorHistory;
	};

	class EasyCamExampleNode : public ofxVVVV::Node {
	public:
		void setup();
		void draw();
		void keyPressed(int key);
		void drawInteractionArea();
		bool bShowHelp;
		ofEasyCam cam; // add mouse controls for camera movement
	};

	class OpenNIExampleNode : public ofxVVVV::Node {
		void setup();
		void update();
		void draw();

		ofxOpenNI openNI;
	};

	class VideoExampleNode : public ofxVVVV::Node {
		void setup();
		void update();
		void draw();

		void keyPressed(int key);
		void mouseMoved(int x, int y );
		void mouseDragged(int x, int y, int button);
		void mousePressed(int x, int y, int button);
		void mouseReleased(int x, int y, int button);

		ofVideoPlayer 		* fingerMovie;
		bool                frameByframe;
	};
}