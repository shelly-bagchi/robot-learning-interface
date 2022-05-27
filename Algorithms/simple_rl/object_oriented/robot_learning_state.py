#!/usr/bin/env python
import copy
import random

from simple_rl.mdp.StateClass import State


class RobotLearningState(State):
    def __init__(self, task, agent, blocks=[], cells=[]):
        '''
        :param task: The given Task
        :param x: Agent x coordinate
        :param y: Agent y coordinate
        :param blocks: List of blocks
        :param cells: List of cells
        '''
        self.agent = agent
        self.blocks = blocks
        self.cells = cells
        self.task = task
        State.__init__(self, data=[task, blocks, cells])

    def __hash__(self):
        alod = [tuple(self.data[i]) for i in range(1, len(self.data))]
        alod.append(self.data[0])
        return hash(tuple(alod))

    def __str__(self):
        str_builder = "(" + str(self.agent.x) + ", " + \
            str(self.agent.y) + ")\n"
        str_builder += "\nBLOCKS:\n"
        for block in self.blocks:
            str_builder += str(block) + "\n"
        str_builder += "\nCELLS:\n"
        for cell in self.cells:
            str_builder += str(cell) + "\n"
        return str_builder

    @staticmethod
    def list_eq(alod1, alod2):
        '''
        :param alod1: First list
        :param alod2: Second list
        :return: A boolean indicating whether or not the lists are the same
        '''
        if len(alod1) != len(alod2):
            return False
        sa = set(alod2)
        for item in alod1:
            if item not in sa:
                return False

        return True

    def __eq__(self, other):
        return isinstance(other, RobotLearningState) and self.agent.x == other.x and self.agent.y == other.y and \
            self.list_eq(other.cells, self.cells) and \
            self.list_eq(other.blocks, self.blocks)

    def copy(self):
        new_blocks = [block.copy() for block in self.blocks]
        new_cells = [cell.copy() for cell in self.cells]
        return RobotLearningState(self.task, self.agent, new_blocks, new_cells)
