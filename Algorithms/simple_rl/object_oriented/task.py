class Task:
    def __init__(self, block_color, goal_cell_id, goal_x, goal_y, block_id):
        '''
        You can choose which attributes you would like to have represent the blocks and the rooms
        '''
        self.goal_cell_id = goal_cell_id
        self.block_color = block_color
        self.block_id = block_id
        self.goal_x = goal_x
        self.goal_y = goal_y

    def __str__(self):
        return "The " + self.block_color + "block named " + self.block_id + " to the room named " + self.goal_cell_id
