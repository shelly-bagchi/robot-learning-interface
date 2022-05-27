from robot_learning_state import RobotLearningState


class GridCell:
    def __init__(self, id, coors):
        self.id = id
        self.coors = coors

    def contains(self, block):
        return (block.x, block.y) in self.coors

    def copy(self):
        return GridCell(self.id, self.coors[:])

    def __hash__(self):
        return hash(tuple([self.id, tuple(self.coors)]))

    def __eq__(self, other):
        if not isinstance(other, GridCell):
            return False

        return self.id == other.id and \
            RobotLearningState.list_eq(self.coors, other.coors)

    def __str__(self):
        return "id: " + self.id + ", points: " + str(self.coors)
