import json
import sys
import logging
import os
import getopt
import os.path
from InstructionManager import InstructionManager
from NaoManager import NaoManager

instructionManager = InstructionManager()

if __name__ == "__main__":

    logging.basicConfig(stream=sys.stderr, level=logging.DEBUG)
    logger = logging.getLogger("Main")

    # Start by handling command line arguments
    robotIP = "127.0.0.1"
    robotPort = 9559
    recordPath = None
    helpInfo = "How to use:\n" \
               "Main.py --recording <file path to recording> [--ip <ip address>] [--port <port>]" \
               "The recording flag is mandatory. If no ip or port is specified, 127.0.0.1 and 9559 will be used."
    try:
        opts, args = getopt.getopt(sys.argv[1:], "", ["ip", "port", "recording="])
    except getopt.GetoptError:
        logger.info(helpInfo)
        sys.exit(2)
    for opt, arg in opts:
        if opt == ("-h", "--help"):
            print helpInfo
            sys.exit()
        elif opt in "--ip":
            robotIP = arg
        elif opt in "--port":
            robotPort = arg
        elif opt in "--recording":
            recordPath = arg

    if recordPath is None:
        logger.error("You need to specify to file path to the program you want to run!")
        logger.info(helpInfo)
        sys.exit(2)

    # make sure it's a json file
    if not recordPath.endswith(".json"):
        logger.error("The record file should have .json extension")
        sys.exit(1)

    # make sure file exists
    if not os.path.isfile(recordPath):
        logger.error("The file at %s doesn't exist!", recordPath)
        sys.exit(1)

    naoManager = NaoManager(str(robotIP), int(robotPort))

    # Read file
    jsonFile = open(recordPath, 'r')
    dict = json.loads(jsonFile.read())

    # Create instructions from JSON file
    instructions = instructionManager.extract_commands(dict)

    # Turn on stiffness
    naoManager.stiffness_on()

    # Send NAO to Pose Init
    naoManager.postureProxy.goToPosture("StandInit", 0.5)

    # todo Validate instructions: bodypart exists, time < 5 mins, angle in range...

    # we distinguish between a move command and other commands.
    # If it's a move command then we only run move commands during programming.
    # We can execute multiple other commands
    if instructions[0].command == 0:
        # move

        # Organise instructions
        bodyParts = []
        angles = []
        timers = []

        # -- Sort instructions by bodypart
        instructionManager.organise_instructions(instructions, bodyParts, angles, timers)

        # Execute instructions
        naoManager.motionProxy.angleInterpolation(bodyParts, angles, timers, True)

    else:
        # only commands, no movement
        for instruction in instructions:

            # move
            if instruction.command == 1:
                naoManager.move(instruction.direction, 3)

            # turn
            elif instruction.command == 2:
                naoManager.turn(instruction.direction)

            # sing
            elif instruction.command == 3:
			    # Removed to prevent copyright infringement
                # naoManager.load_audio("Portal_Still_Alive")

            # sit
            elif instruction.command == 4:
                naoManager.postureProxy.goToPosture("SitRelax", 1.0)

            # stand
            elif instruction.command == 5:
                naoManager.postureProxy.goToPosture("Stand", 1.0)

            # wave
            elif instruction.command == 6:
                naoManager.load_behavior("Wave")

            # dance
            elif instruction.command == 7:
                naoManager.load_behavior("TaiChi")

            # Baymax fist bump
            elif instruction.command == 8:
				# Removed to prevent copyright infringement
                # naoManager.load_behavior("Balalala")

            # Thank you for your attention
            elif instruction.command == 9:
                naoManager.say("Thank you for your attention")
