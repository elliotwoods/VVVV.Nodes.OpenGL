#include "Nodes.h"

namespace CNodes {
	//--------
	ofSetupScreenNode::ofSetupScreenNode() {
		this->windowPointer = ofPtr<ofAppBaseWindow>( &window );
	}

	//--------
	void ofSetupScreenNode::update() {
		ofNotifyUpdate();
	}

	//--------
	void ofSetupScreenNode::draw() {
		ofSetCurrentWindow(windowPointer);
		window.update();
		ofSetupScreenPerspective();
		ofNotifyDraw();
	}

	//--------
	void ofSetupScreenNode::keyPressed(int key) {
		ofNotifyKeyPressed(key);
	}
	
	//--------
	void ofSetupScreenNode::keyReleased(int key) {
		ofNotifyKeyReleased(key);
	}

	//--------
	void ofSetupScreenNode::mouseMoved(int x, int y) {
		ofNotifyMouseMoved(x, y);
	}
	
	//--------
	void ofSetupScreenNode::mouseDragged(int x, int y, int button) {
		ofNotifyMouseDragged(x, y, button);
	}	

	//--------
	void ofSetupScreenNode::mousePressed(int x, int y, int button) {
		ofNotifyMousePressed(x, y, button);
	}	

	//--------
	void ofSetupScreenNode::mouseReleased(int x, int y, int button) {
		ofNotifyMouseReleased(x, y, button);
	}	

	//--------
	void ofLineNode::draw() {
		ofLine(0,0,ofGetWidth(),ofGetHeight());
	}

	//--------
	void ofDrawBitmapStringNode::draw() {
		ofDrawBitmapString("hello", 30, 30);
	}

	//--------
	void GraphicsExampleNode::setup() {
		counter = 0;
		ofSetCircleResolution(50);
		bSmooth = true;
		ofSetWindowTitle("graphics example");
	}
	
	//--------
	void GraphicsExampleNode::update() {
		counter = counter + 0.033f;
	}

	//--------
	void GraphicsExampleNode::draw() {
		ofBackground(255,255,255);

		ofPushMatrix();
		ofTranslate(0, -240, 0);

		//--------------------------- circles
		//let's draw a circle:
		ofSetColor(255,130,0);
		float radius = 50 + 10 * sin(counter);
		ofFill();		// draw "filled shapes"
		ofCircle(100,400,radius);

		// now just an outline
		ofNoFill();
		ofSetHexColor(0xCCCCCC);
		ofCircle(100,400,80);

		// use the bitMap type
		// note, this can be slow on some graphics cards
		// because it is using glDrawPixels which varies in
		// speed from system to system.  try using ofTrueTypeFont
		// if this bitMap type slows you down.
		ofSetHexColor(0x000000);
		ofDrawBitmapString("circle", 75,500);


		//--------------------------- rectangles
		ofFill();
		for (int i = 0; i < 200; i++){
			ofSetColor((int)ofRandom(0,255),(int)ofRandom(0,255),(int)ofRandom(0,255));
			ofRect(ofRandom(250,350),ofRandom(350,450),ofRandom(10,20),ofRandom(10,20));
		}
		ofSetHexColor(0x000000);
		ofDrawBitmapString("rectangles", 275,500);

		//---------------------------  transparency
		ofSetHexColor(0x00FF33);
		ofRect(400,350,100,100);
		// alpha is usually turned off - for speed puposes.  let's turn it on!
		ofEnableAlphaBlending();
		ofSetColor(255,0,0,127);   // red, 50% transparent
		ofRect(450,430,100,33);
		ofSetColor(255,0,0,(int)(counter * 10.0f) % 255);   // red, variable transparent
		ofRect(450,370,100,33);
		ofDisableAlphaBlending();

		ofSetHexColor(0x000000);
		ofDrawBitmapString("transparency", 410,500);

		//---------------------------  lines
		// a bunch of red lines, make them smooth if the flag is set

		if (bSmooth){
			ofEnableSmoothing();
		}

		ofSetHexColor(0xFF0000);
		for (int i = 0; i < 20; i++){
			ofLine(600,300 + (i*5),800, 250 + (i*10));
		}

		if (bSmooth){
			ofDisableSmoothing();
		}

		ofSetHexColor(0x000000);
		ofDrawBitmapString("lines\npress 's' to toggle smoothness", 600,500);

		ofPopMatrix();


		ofPushStyle();
		ofColor c(200, 100, 100);
		for(int i=0; i<cursorHistory.size(); i++) {
			c.setHue(i * 30);
			ofSetColor(c);
			ofCircle(cursorHistory[i], i); 
		}
		ofPopStyle();

		ofDrawBitmapString(ofToString(ofGetFrameRate()), 20, 20);
	}

	//--------
	void GraphicsExampleNode::keyPressed(int key){
		if (key == 's')
			bSmooth = !bSmooth;
	}

	//--------
	void GraphicsExampleNode::mouseMoved(int x, int y){
		cursorHistory.push_back(ofPoint(x,y));
		if (cursorHistory.size() > 10)
			cursorHistory.pop_front();
	}

	//--------
	void EasyCamExampleNode::setup(){	
		// this uses depth information for occlusion
		// rather than always drawing things on top of each other
		glEnable(GL_DEPTH_TEST);
	
		cam.enableMouseInput();
		// this sets the camera's distance from the object
		cam.setDistance(100);
	
		ofSetCircleResolution(64);
		bShowHelp = true;
	}

	//--------
	void EasyCamExampleNode::draw(){
		cam.begin();		
		ofRotateX(ofRadToDeg(.5));
		ofRotateY(ofRadToDeg(-.5));
	
		ofBackground(0);
	
		ofSetColor(255,0,0);
		ofFill();
		ofBox(30);
		ofNoFill();
		ofSetColor(0);
		ofBox(30);
	
		ofPushMatrix();
		ofTranslate(0,0,20);
		ofSetColor(0,0,255);
		ofFill();
		ofBox(5);
		ofNoFill();
		ofSetColor(0);
		ofBox(5);
		ofPopMatrix();
		cam.end();
		drawInteractionArea();
		ofSetColor(255);
		string msg = string("Using mouse inputs to navigate (press 'c' to toggle): ") + (cam.getMouseInputEnabled() ? "YES" : "NO");
		msg += string("\nShowing help (press 'h' to toggle): ")+ (bShowHelp ? "YES" : "NO");
		if (bShowHelp) {
			msg += "\n\nLEFT MOUSE BUTTON DRAG:\nStart dragging INSIDE the yellow circle -> camera XY rotation .\nStart dragging OUTSIDE the yellow circle -> camera Z rotation (roll).\n\n";
			msg += "LEFT MOUSE BUTTON DRAG + TRANSLATION KEY (" + ofToString(cam.getTranslationKey()) + ") PRESSED\n";
			msg += "OR MIDDLE MOUSE BUTTON (if available):\n";
			msg += "move over XY axes (truck and boom).\n\n";
			msg += "RIGHT MOUSE BUTTON:\n";
			msg += "move over Z axis (dolly)";
		}
		msg += "\n\nfps: " + ofToString(ofGetFrameRate(), 2);
		ofDrawBitmapStringHighlight(msg, 10, 20);
	}

	//--------
	void EasyCamExampleNode::drawInteractionArea(){
		ofRectangle vp = ofGetCurrentViewport();
		float r = MIN(vp.width, vp.height) * 0.5f;
		float x = vp.width * 0.5f;
		float y = vp.height * 0.5f;
	
		ofPushStyle();
		ofSetLineWidth(3);
		ofSetColor(255, 255, 0);
		ofNoFill();
		glDepthMask(false);
		ofCircle(x, y, r);
		glDepthMask(true);
		ofPopStyle();
	}

	//--------
	void EasyCamExampleNode::keyPressed(int key){
		switch(key) {
			case 'C':
			case 'c':
				if(cam.getMouseInputEnabled()) cam.disableMouseInput();
				else cam.enableMouseInput();
				break;
			
			case 'F':
			case 'f':
				ofToggleFullscreen();
				break;
			case 'H':
			case 'h':
				bShowHelp ^=true;
				break;
		}
	}

	//--------
	void OpenNIExampleNode::setup(){
		ofSetLogLevel(ofxOpenNI::LOG_NAME,OF_LOG_VERBOSE);

		bool live = true;

		if(live){
			openNI.setupFromXML("C:\\openFrameworks\\openFrameworks\\addons\\ofxOpenNI2\\example\\bin\\data\openni/config/ofxopenni_config.xml",false);
		}else{
			openNI.setupFromRecording("recording.oni");
		}
	}

	//--------
	void OpenNIExampleNode::update(){
		openNI.update();
	}

	//--------
	void OpenNIExampleNode::draw(){
		openNI.draw(0,0);
		openNI.drawRGB(640,0);
		ofDrawBitmapString(ofToString(ofGetFrameRate()),20,20);
	}

	//--------
	void VideoExampleNode::setup(){
		fingerMovie = new ofVideoPlayer();
		string path = "C:\\openFrameworks\\openFrameworks\\examples\\video\\videoPlayerExample\\bin\\data\\movies\\fingers.mov";
		fingerMovie->loadMovie(path);
		path = ofToDataPath(path);
		cout << path;
		fingerMovie->play();
	}

	//--------
	void VideoExampleNode::update(){
	}

	//--------
	void VideoExampleNode::draw(){
		fingerMovie->update();
		ofSetHexColor(0xFFFFFF);

		ofImage image;
		image.setFromPixels(fingerMovie->getPixelsRef());
		image.update();
		image.draw(20,20);

		ofSetHexColor(0x000000);
		unsigned char * pixels = fingerMovie->getPixels();
    
		int nChannels = fingerMovie->getPixelsRef().getNumChannels();
    
		// let's move through the "RGB(A)" char array
		// using the red pixel to control the size of a circle.
		for (int i = 4; i < 320; i+=8){
			for (int j = 4; j < 240; j+=8){
				unsigned char r = pixels[(j * 320 + i)*nChannels];
				float val = 1 - ((float)r / 255.0f);
				ofCircle(400 + i,20+j,10*val);
			}
		}


		ofSetHexColor(0x000000);
		ofDrawBitmapString("press f to change",20,320);
		if(frameByframe) ofSetHexColor(0xCCCCCC);
		ofDrawBitmapString("mouse speed position",20,340);
		if(!frameByframe) ofSetHexColor(0xCCCCCC); else ofSetHexColor(0x000000);
		ofDrawBitmapString("keys <- -> frame by frame " ,190,340);
		ofSetHexColor(0x000000);

		ofDrawBitmapString("frame: " + ofToString(fingerMovie->getCurrentFrame()) + "/"+ofToString(fingerMovie->getTotalNumFrames()),20,380);
		ofDrawBitmapString("duration: " + ofToString(fingerMovie->getPosition()*fingerMovie->getDuration(),2) + "/"+ofToString(fingerMovie->getDuration(),2),20,400);
		ofDrawBitmapString("speed: " + ofToString(fingerMovie->getSpeed(),2),20,420);

		if(fingerMovie->getIsMovieDone()){
			ofSetHexColor(0xFF0000);
			ofDrawBitmapString("end of movie",20,440);
		}
	}

	//--------------------------------------------------------------
	void VideoExampleNode::keyPressed  (int key){
		switch(key){
			case 'f':
				frameByframe=!frameByframe;
				fingerMovie->setPaused(frameByframe);
			break;
			case OF_KEY_LEFT:
				fingerMovie->previousFrame();
			break;
			case OF_KEY_RIGHT:
				fingerMovie->nextFrame();
			break;
			case '0':
				fingerMovie->firstFrame();
			break;
		}
	}

	//--------------------------------------------------------------
	void VideoExampleNode::mouseMoved(int x, int y ){
		if(!frameByframe){
			int width = ofGetWidth();
			float pct = (float)x / (float)width;
			float speed = (2 * pct - 1) * 5.0f;
			fingerMovie->setSpeed(speed);
		}
	}

	//--------------------------------------------------------------
	void VideoExampleNode::mouseDragged(int x, int y, int button){
		if(!frameByframe){
			int width = ofGetWidth();
			float pct = (float)x / (float)width;
			fingerMovie->setPosition(pct);
		}
	}

	//--------------------------------------------------------------
	void VideoExampleNode::mousePressed(int x, int y, int button){
		if(!frameByframe){
			fingerMovie->setPaused(true);
		}
	}


	//--------------------------------------------------------------
	void VideoExampleNode::mouseReleased(int x, int y, int button){
		if(!frameByframe){
			fingerMovie->setPaused(false);
		}
	}
}
