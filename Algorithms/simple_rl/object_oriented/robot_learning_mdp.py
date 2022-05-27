#!/usr/bin/env python

'''
Make an MDP subclass, which needs:

A static variable, ACTIONS, which is a list of strings denoting each action.

Implement a reward and transition function and pass them to MDP constructor (along with ACTIONS).

I also suggest overwriting the "__str__" method of the class, and adding a "__init__.py" file to the directory.

Create a State subclass for your MDP (if necessary). I suggest overwriting the "__hash__", "__eq__", and "__str__" for the class to play along well with the agents.
'''

# Python import.
from collections import defaultdict
import copy
from os import stat
import random

# Other imports.
from simple_rl.mdp.MDPClass import MDP
from robot_learning_state import RobotLearningState
from robot import Robot


class RobotLearningMDP(MDP):
    ACTIONS = ["up", "down", "left", "right", "pickup", "putdown"]

    def __init__(self, task, init_loc=(0, 0), blocks=[], rand_init=False, cells=[], gamma=0.99,
                 init_state=None, step_cost=0.0):
        '''
        :param task: The given task for this MDP
        :param init_loc: Initial agent location
        :param blocks: List of blocks
        :param cells: List of grid cells
        :param rand_init: random initialization boolean
        :param gamma: gamma factor
        :param init_state: Initial state if given
        :param step_cost: step cost
        '''

        legal_states = [(x, y)
                        for cell in cells for x, y in cell.coors]
        self.legal_states = set(legal_states)
        self.width = max(self.legal_states, key=lambda tup: tup[0])[0] + 1
        self.height = max(self.legal_states, key=lambda tup: tup[1])[1] + 1
        self.step_cost = step_cost
        self.init_loc = init_loc

        self.task = task
        self.rand_init = rand_init
        if rand_init:
            init_loc = random.randint(
                1, self.width), random.randint(1, self.height)

        self.agent = Robot(init_loc[0], init_loc[1])
        init_state = RobotLearningState(
            task, agent=self.agent, blocks=blocks, cells=cells)
        self.cur_state = init_state
        MDP.__init__(self, self.ACTIONS, self._transition_func,
                     self._reward_func, init_state=init_state, gamma=gamma)

    def _reward_func(self, state, action, next_state):
        '''
        :param state: The state robot is in before performing the action
        :param action: The action robot would like to perform in the state
        :param next_state: next state.
        :return: A double indicating how much reward to assign to that state. 
                If robot has the block, then return 1000 - step_cost for the terminal state; 
                -1.0 for every other state. If robot, doesn't have the block, 
                then return 500 - step_cost when robot finds block; -1.0 for every other state
        '''
        if next_state.agent.has_block:
            if not self.is_terminal_state(next_state):
                return 0 - self.step_cost
            return 1000 - self.step_cost

        for block in next_state.blocks:
            if next_state.agent.x == block.x and next_state.agent.y == block.y and block.color == next_state.task.block_color:
                return 500 - self.step_cost
        return 0 - self.step_cost

    def _transition_func(self, state, action):
        '''
        :param state: The state
        :param action: The action
        :return: The next state robot gets if in state and perform action
        '''
        if action == "up" and state.agent.x < self.height:
            next_state = self.move_agent(state, dy=1)
        elif action == "down" and state.agent.y > 1:
            next_state = self.move_agent(state, dy=-1)
        elif action == "right" and state.agent.x < self.width:
            next_state = self.move_agent(state, dx=1)
        elif action == "left" and state.agent.x > 1:
            next_state = self.move_agent(state, dx=-1)
        elif action == "putdown":
            next_state = self.agent_putdown(state)
        elif action == "pickup":
            next_state = self.agent_pickup(state)
        else:
            next_state = state

        if self.is_terminal_state(next_state):
            next_state.set_terminal(True)

        return next_state

    # ----------------------------
    # -- Action Implementations --
    # ----------------------------

    def is_terminal_state(self, state):
        '''
        :param state: The state we want to check is terminal
        :return: A boolean indicating whether the state is terminal or not
        '''
        for block in state.blocks:
            if block.is_held or block.x != state.task.goal_x or block.y != state.task.goal_y\
                    or block.color != state.task.block_color or block.id != state.task.block_id:
                return False
        return True

    def move_agent(self, state, dx=0, dy=0):
        '''
        :param state: the state
        :param dx: the distance to move in the x direction
        :param dy: the distance to move in the y direction
        :return: the next state you get after robot moves (with or without the block)
        '''

        next_state = copy.deepcopy(state)

        next_agent = next_state.agent
        next_agent.move(dx, dy)

        blocks = state.blocks
        for block in blocks:
            if block.is_held:
                block.x += dx
                block.y += dy

        return next_state

    def agent_pickup(self, state):
        '''
        :param state: the state
        :return: the next state you get after robot picks up the block
        '''
        next_state = copy.deepcopy(state)
        next_agent = next_state.agent

        if next_agent.has_block == False:
            for block in state.blocks:
                if next_agent.x == block.x and next_agent.y == block.y:
                    next_agent.pick_up()
                    block.is_held = True

        return next_state

    def agent_putdown(self, state):
        '''
        :param state: the state
        :return: the next state you get after robot puts down the block
        '''
        next_state = copy.deepcopy(state)
        next_agent = next_state.agent

        blocks = next_state.blocks
        for block in blocks:
            if block.is_held:
                block.is_held = False
                next_agent.put_down()

        return next_state

    @staticmethod
    def find_block(blocks, x, y):
        '''
        :param blocks: The list of blocks
        :param x: x coordinate in question
        :param y: y coordinate in question
        :return: The block (x, y) is associated with.  Or False if no association found.
        '''
        for block in blocks:
            if x == block.x and y == block.y:
                return block
        return False

    @staticmethod
    def check_in_cell(cells, x, y):
        '''
        :param rooms: A list of cells
        :param x: x coordinate
        :param y: y coordinate
        :return: Checks which cell (x, y) is in.  Returns the cell if the cell is found.
                 Returns False otherwise.
        '''
        for cell in cells:
            if (x, y) in cell.coors:
                return cell
        return False
