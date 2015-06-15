from Instruction import Instruction


class InstructionManager(object):

    def __init__(self):
        self._instructions = []
        self.keys = {
            "command": "Command",
            "bodypart": "BodyPart",
            "angle": "Angle",
            "time": "Time",
            "text": "Text",
            "direction": "Direction"
        }

    @staticmethod
    def organise_instructions(instructions, body_parts, angles, timers):
        sorted_instructions = sorted(instructions, key=lambda instruct: (instruct.bodypart, instruct.time))
        body_parts_index = -1
        for instruction in sorted_instructions:
            if instruction.bodypart not in body_parts:
                body_parts.append(str(instruction.bodypart))
                body_parts_index += 1
                angles.append([])
                timers.append([])
            angles[body_parts_index].append(instruction.angle)

            if instruction.time is None:
                raise Exception("time Can not be null")

            timers[body_parts_index].append(instruction.time + 1)

        # Check all timers for velocity
        # If there's isn't enough time between movements, some extra time will be added here
        # This is to prevent the "Body max velocity exception"
        min_movement_time = 0.2
        for timer_array in timers:
            for i in xrange(0, len(timer_array)):
                if i + 1 < len(timer_array) and timer_array[i+1] - timer_array[i] < min_movement_time:
                    # Add x seconds to all timers from i+1
                    for j in xrange(i+1, len(timer_array)):
                        timer_array[j] += min_movement_time

    def extract_commands(self, json_dict):
        for jsonBlock in json_dict:
            instruction = Instruction()
            # Handle unknown or non existing keys
            if self.keys["command"] in jsonBlock:
                instruction.command = jsonBlock[self.keys["command"]]

            if self.keys["angle"] in jsonBlock:
                instruction.angle = jsonBlock[self.keys["angle"]]

            if self.keys["text"] in jsonBlock:
                instruction.text = jsonBlock[self.keys["text"]]

            if self.keys["time"] in jsonBlock:
                instruction.time = jsonBlock[self.keys["time"]]

            if self.keys["direction"] in jsonBlock:
                instruction.direction = jsonBlock[self.keys["direction"]]

            if self.keys["bodypart"] in jsonBlock:
                instruction.bodypart = jsonBlock[self.keys["bodypart"]]

            self.instructions.append(instruction)

        return self.instructions

    @property
    def instructions(self):
        return self._instructions

    @instructions.setter
    def instructions(self, value):
        self._instructions = value

    @instructions.deleter
    def instructions(self):
        del self._instructions
