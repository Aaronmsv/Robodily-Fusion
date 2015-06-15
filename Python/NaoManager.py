from naoqi import ALProxy
import os
import logging
import sys
import time
import math

logging.basicConfig(stream=sys.stderr, level=logging.DEBUG)
logger = logging.getLogger("Main")


class NaoManager(object):

    def __init__(self, ip, port):
        self.ip = ip
        self.port = port

        # Create proxies
        try:
            self.motionProxy = ALProxy("ALMotion", ip, port)
            self.postureProxy = ALProxy("ALRobotPosture", ip, port)
            self.speechProxy = ALProxy("ALTextToSpeech", ip, port)
            self.audioProxy = ALProxy("ALAudioPlayer", self.ip, self.port)
            self.frameProxy = ALProxy("ALFrameManager", self.ip, self.port)
        except Exception, e:
            logger.critical("Could not create a proxy. Error was: %s", e)
            sys.exit(0)

    def load_audio(self, file_name):
        try:
            file_name = os.path.join(os.path.dirname(__file__), 'music\\' + file_name + '.wav')
            self.audioProxy.playFile(file_name)
        except Exception, ex:
            logger.critical("Could not create an audio proxy. Error was: %s", ex)
            sys.exit(0)

    def load_behavior(self, file_name):
        try:
            filename = os.path.join(os.path.dirname(__file__), 'behavior\\' + file_name + '.xar')
            behavior_id = self.frameProxy.newBehaviorFromFile(filename, "")
            self.frameProxy.completeBehavior(behavior_id)
        except Exception, ex:
            logger.critical("Could not create a behavior proxy or load file. Error was: %s", ex)
            sys.exit(0)

    def move(self, direction, t):

        self.motionProxy.setWalkArmsEnabled(True, True)
        self.motionProxy.setMotionConfig([["ENABLE_FOOT_CONTACT_PROTECTION", True]])

        x = 0.0  # Use negative for backwards. [-1.0 to 1.0]
        y = 0.0  # Use negative for right. [-1.0 to 1.0]
        theta = 0.0  # Use negative for clockwise [-1.0 to 1.0]
        frequency = 0.0  # Fraction of MaxStepFrequency [0.0 to 1.0] (Speed)

        # forward
        if direction == 0:
            x = 1.0
        # backward
        elif direction == 5:
            x = -1.0
        # right
        elif direction == 3:
            y = -1.0
        # left
        elif direction == 2:
            y = -1.0

        self.motionProxy.setWalkTargetVelocity(x, y, theta, frequency)

        # walk for t seconds
        time.sleep(int(t))

        # stop walking
        self.motionProxy.stopMove()

    def turn(self, direction):
        # right
        if direction == 3:
            self.motionProxy.moveTo(0.0, 0.0, -math.pi / 2)
        # left
        elif direction == 2:
            self.motionProxy.moveTo(0.0, 0.0, math.pi / 2)
        # around
        elif direction == 4:
            self.motionProxy.moveTo(0.0, 0.0, math.pi)

    def stiffness_on(self):
        # We use the "Body" name to signify the collection of all joints
        names = "Body"
        stiffness_lists = 1.0
        time_lists = 1.0
        self.motionProxy.stiffnessInterpolation(names, stiffness_lists, time_lists)

    def say(self, text):
        self.speechProxy.say(text)



