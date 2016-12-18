COMP 537 - Intelligent User Interfaces Term Project
===================================================

UML Class Diagram Sketching Tool
################################

Recognizing user sketches to draw UML class diagrams.

Index
-----
- `Demonstration Video`_
- `Functionalities`_
- `Dataset`_
- `Implementation`_
- `Requirements`_
- `References`_

Demonstration Video
-------------------

https://youtu.be/tXRMwprSzBM

Annotations provide information when an action is taken.

.. |draw| image:: https://github.com/ekyurdakul/COMP537/blob/master/docs/images/draw.png?raw=true
	:align: middle
.. |erase| image:: https://github.com/ekyurdakul/COMP537/blob/master/docs/images/erase.png?raw=true
	:align: middle
.. |handwriting| image:: https://i-msdn.sec.s-msft.com/dynimg/IC5936.gif
	:align: middle
.. |clear| image:: https://github.com/ekyurdakul/COMP537/blob/master/docs/images/clear.png?raw=true
	:align: middle
.. |save| image:: https://github.com/ekyurdakul/COMP537/blob/master/docs/images/save.png?raw=true
	:align: middle
.. |chevronup| image:: https://i-msdn.sec.s-msft.com/dynimg/IC100787.gif 
	:align: middle
.. |chevrondown| image:: https://i-msdn.sec.s-msft.com/dynimg/IC109874.gif 
	:align: middle
.. |chevronleft| image:: https://i-msdn.sec.s-msft.com/dynimg/IC137959.gif 
	:align: middle
.. |chevronright| image:: https://i-msdn.sec.s-msft.com/dynimg/IC40744.gif
	:align: middle

Functionalities
---------------
- Sketching Mode |draw|
	Default mode.
- Deletion Mode |erase|
	Draw a shape to cover the area where you want to erase.
- Handwriting Mode |handwriting|
	Enabled by a check gesture while in sketching mode. A second check gesture exits this mode.
- Canvas Panning
	Moving around the canvas is achieved by gestures:

	Chevron Up |chevronup|

	Chevron Down |chevrondown|

	Chevron Left |chevronleft|

	Chevron Right |chevronright|

- Canvas Clearing |clear|
	Clears everything.
- Suggestions Tab
	You can correct a misrecognition by clicking on a suggestion. Predicted probabilities decrease from left to right.
- Image Export |save|
	Save canvas as image.
- Sketching Scale
	The bigger you sketch, the bigger is the recognized sketch.

Dataset
-------

There are three classes, each having 50 train and 10 test sketches at a resolution of 1440x2560 pixels.

#) Class
	.. image:: https://github.com/ekyurdakul/COMP537/blob/master/docs/images/class.png?raw=true
#) Implementation Arrow
	.. image:: https://github.com/ekyurdakul/COMP537/blob/master/docs/images/implementation.png?raw=true
#) Inheritance Arrow
	.. image:: https://github.com/ekyurdakul/COMP537/blob/master/docs/images/inheritance.png?raw=true

Implementation
--------------

| `HOG <https://en.wikipedia.org/wiki/Histogram_of_oriented_gradients>`_ features extracted using `VLFeat <https://github.com/vlfeat/vlfeat/releases/tag/v0.9.20>`_ in MATLAB.
| Multi-class SVM with linear kernel trained using `LIBSVM <http://www.csie.ntu.edu.tw/~cjlin/libsvm/#matlab>`_ in MATLAB.
| Thread running in the background on the Windows Presentation Foundation (WPF) GUI written in C#.
| When you complete a sketch, its class is predicted by sending data from C# to MATLAB.
| Recognized sketch gets replaced by a predefined image.

Requirements
------------
- Visual Studio 2015 Community Edition
- MATLAB
- LIBSVM_ for MATLAB
- Windows 10

References
----------

#) \C. Chang, C. Lin, LIBSVM: A library for support vector machines. ACM Transactions on Intelligent Systems and Technology. 2:27:1–27:27. 2011. Software available at http://www.csie.ntu.edu.tw/~cjlin/libsvm.
#) \A. Vedaldi, B. Fulkerson. VLFeat: An Open and Portable Library of Computer Vision Algorithms. http://www.vlfeat.org. 2008.
#) Microsoft Visual Studio Image Library. https://www.microsoft.com/en-us/download/details.aspx?id=35825
#) Microsoft Check Gesture Icon. https://i-msdn.sec.s-msft.com/dynimg/IC5936.gif
#) Microsoft Chevron Up Gesture Icon. https://i-msdn.sec.s-msft.com/dynimg/IC100787.gif
#) Microsoft Chevron Down Gesture Icon. https://i-msdn.sec.s-msft.com/dynimg/IC109874.gif
#) Microsoft Chevron Left Gesture Icon. https://i-msdn.sec.s-msft.com/dynimg/IC137959.gif
#) Microsoft Chevron Right Gesture Icon. https://i-msdn.sec.s-msft.com/dynimg/IC40744.gif
#) \L. Qiu. SketchUML: The Design of a Sketch-based Tool for UML Class Diagrams. 2007.
#) \T. Hammond, R. Davis. Tahuti: A Geometrical Sketch Recognition System for UML Class Diagrams. 2002.