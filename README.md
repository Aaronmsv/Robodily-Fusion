#  Robodily Fusion #

Robodily Fusion means programming robots by using body movements. The project made with the Microsft Kinect and the NAO robot. It's focused on children and supports children with a disability by using voice commands.

This project was my bachelor thesis on my Erasmus internship at Halmstad University (Sweden), in collaboration with M. Geerinck. 

## Features ##

* Speech commands
* Good logging (Nlog)
* Configuration file (to change save location for recording)
* Easy, universal robot interface
* JSON output of recordings

## What can be improved ##

1. Add more body calculations
2. Not all body angle calculations are 100% correct, but at least they are close. (See unit tests)
3. Some angles probably need to be e.g. reversed on the NAO

## Creating a recording ##

1. Double click **Robodily-Fusion-Kinect.exe** to start the Kinect software.
2. The software works with voice commands. To start recording a new program, simply say ”Start programming”.
3. Program the robot by moving your arms, head, chest, legs...
4. Say "Stop programming" to stop the recording.
5. Your program will now be saved on your Desktop in a folder called "Robodily-Fusion". You program will have the date and time as file name and a .json extension.

## Other Commands ##

Aside from showing the movements yourself, you can also tell the robot to:

* **Move**: you can move limbs this way, for example: "Move left arm up".
* **Turn**: this will turn the robot as a whole, for example: "Move right" will rotate the robot 90 degrees.
* **Sing**: the robot will play "Still alive" from Portal. (Removed to prevent copyright infringement.)
* **Sit**: the robot will sit down.
* **Stand**: the robot will stand up.
* **Wave**: the robot will do a wave.
* **Fist bump**: the robot will do a Baymax first bump.

## Running a recording on the NAO robot ##

The NAO program is called Robodily-Fusion-NAO.py and it works with the console. It has one mandatory command line argument and that’s the path to the recording you want to play. You can also specify the port and IP address, but it will take 127.0.0.1 and 9559 as default if you don’t. This could be an example: 

```python D:\Projects\robodily−fusion\Python\Main.py −−recording "C:\Users\Aaron\Desktop\Robodily-Fusion\060515181420.json"``` 

If you want to change the IP address and port, then use the following command: 

```python D:\Projects\robodily−fusion\Python\Main.py −−recording "C:\Users\Aaron\Desktop\Robodily-Fusion\060515181420.json" −−ip 127.0.0.1 −−port 9559```

And the robot should execute your program. Before the robot executes any program, it will always go to a default stance.

## How to simulate the NAO robot ##

1. Make sure Naoqi and Choregraph are installed (and the versions match!)
2. Start Naoqi, for example (-d = debug logging): "D:\Programs\Aldebaran\Choregraphe 1.14.3.5\bin\naoqi-bin.exe" -d
3. Start Choregraph and connect to the "robot" started in step 2.
4. Connect to the same robot for your Python scripts

## Configuration ##

Some settings can be changed in the configuration file (Kinect project), although there is no GUI for it. The configuration file is made with XML and if you change the values there, they should be activated next time you start the program. You could change the save location of the recordings for example.

## Required software ##

* Choregraphe 1.14.3
* Python 2.7
* Naoqi 1.14.5
* Visual Studio 2013
* Kinect V1 SDK

## References ##

Windows 8 user control: [http://www.codeproject.com/Tips/546477/Creating-Metro-Windows-Loading-Animation-Using-X](http://www.codeproject.com/Tips/546477/Creating-Metro-Windows-Loading-Animation-Using-X)

Microsoft Kinect demo projects

Microsoft Facetracking toolkit

Microsoft WpfViewers project

