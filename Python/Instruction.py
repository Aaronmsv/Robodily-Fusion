class Instruction(object):
    def __init__(self):
        # This values are all set to None so we can spot uninitialised values easy
        self._command = None
        self._angle = None
        self._text = None
        self._time = None
        self._direction = None
        self._bodypart = None

    @property
    def bodypart(self):
        return self._bodypart

    @bodypart.setter
    def bodypart(self, value):
        if value is None:
            print "bodypart can not be null"
            raise Exception("BodyPart can not be null")
        self._bodypart = value

    @bodypart.deleter
    def bodypart(self):
        del self._bodypart

    @property
    def time(self):
        return self._time

    @time.setter
    def time(self, value):
        self._time = value

    @time.deleter
    def time(self):
        del self._time

    @property
    def command(self):
        return self._command

    @command.setter
    def command(self, value):
        if value is None:
            print "command can not be null"
            raise Exception("command can not be null")
        self._command = value

    @property
    def angle(self):
        return self._angle

    @angle.setter
    def angle(self, value):
        self._angle = value

    @property
    def text(self):
        return self._text

    @text.setter
    def text(self, value):
        if value is None:
            raise Exception("text can not be null")
        self._text = value

    @property
    def direction(self):
        return self._direction

    @direction.setter
    def direction(self, value):
        if value is None:
            raise Exception("direction can not be null")
        self._direction = value