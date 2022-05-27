class Block:

    def __init__(self, id, x=0, y=0, color="", is_held=False):
        self.id = id
        self.x = x
        self.y = y
        self.color = color
        self.is_held = is_held

    @staticmethod
    def class_name():
        return "block"

    def id(self):
        return self.id

    def __eq__(self, other):
        return isinstance(other, Block) and self.x == other.x and self.y == other.y and self.id == other.id \
            and self.color == other.color and self.is_held == other.is_held

    def __hash__(self):
        return hash(tuple([self.id, self.x, self.y, self.color, self.is_held]))

    def copy_with_name(self, new_name):
        return Block(new_name, x=self.x, y=self.y, color=self.color, is_held=self.is_held)

    def copy(self):
        return Block(id=self.id, x=self.x, y=self.y, color=self.color, is_held=self.is_held)

    def __str__(self):
        return "BLOCK. id: " + self.id + ", (x,y): (" + str(self.x) + "," + str(self.y) + "), Color: " + self.color
