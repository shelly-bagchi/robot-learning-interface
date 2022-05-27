class Robot:
    def __init__(self, x, y, has_block=False):
        self.x = x
        self.y = y
        self.has_block = has_block

    def move(self, dx, dy):
        self.x += dx
        self.y += dy

    def pick_up(self):
        self.has_block = True

    def put_down(self):
        self.has_block = False

    def __hash__(self) -> int:
        return hash(tuple([self.x, self.y, self.has_block]))

    def __str__(self) -> str:
        return "x: " + self.x + ", y: " + self.y + ", has block: " + self.has_block
