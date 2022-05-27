# Python imports.
from __future__ import print_function
import random
import sys
import os
import copy
import numpy as np
from collections import defaultdict

# Other imports.
from simple_rl.mdp.MDPClass import MDP
from simple_state import SimpleState


class SimpleRobotLearningMDP(MDP):

    # Static constants.
    ACTIONS = ["up", "down", "left", "right"]

    def __init__(self,
                 width=4,
                 height=4,
                 init_loc=(0, 0),
                 rand_init=False,
                 goal_locs=[()],
                 is_goal_terminal=True,
                 gamma=0.99,
                 step_cost=0.0):

        # Setup init location.
        self.rand_init = rand_init
        if rand_init:
            init_loc = random.randint(1, width), random.randint(1, height)
        self.init_loc = init_loc
        init_state = SimpleState(init_loc[0], init_loc[1])

        MDP.__init__(self, SimpleRobotLearningMDP.ACTIONS, self._transition_func,
                     self._reward_func, init_state=init_state, gamma=gamma)

        if type(goal_locs) is not list:
            raise ValueError(
                "(simple_rl)  Error: argument @goal_locs needs to be a list of locations. For example: [(3,3), (4,3)].")
        self.step_cost = step_cost
        self.width = width
        self.height = height
        self.goal_locs = goal_locs
        self.cur_state = SimpleState(init_loc[0], init_loc[1])
        self.is_goal_terminal = is_goal_terminal

    def get_parameters(self):
        '''
        Returns:
            (dict) key=param_name (str) --> val=param_val (object).
        '''
        param_dict = defaultdict(int)
        param_dict["width"] = self.width
        param_dict["height"] = self.height
        param_dict["init_loc"] = self.init_loc
        param_dict["rand_init"] = self.rand_init
        param_dict["goal_locs"] = self.goal_locs
        param_dict["is_goal_terminal"] = self.is_goal_terminal
        param_dict["gamma"] = self.gamma
        param_dict["step_cost"] = self.step_cost

        return param_dict

    def is_goal_state(self, state):
        return (state.x, state.y) in self.goal_locs

    def _reward_func(self, state, action, next_state):
        '''
        Args:
            state (State)
            action (str)
            next_state (State)

        Returns
            (float)
        '''
        if (int(next_state.x), int(next_state.y)) in self.goal_locs:
            # self._is_goal_state_action(state, action):
            return 100 - self.step_cost
        else:
            return 0 - self.step_cost

    def _is_goal_state_action(self, state, action):
        '''
        Args:
            state (State)
            action (str)

        Returns:
            (bool): True iff the state-action pair send the agent to the goal state.
        '''
        if (state.x, state.y) in self.goal_locs and self.is_goal_terminal:
            # Already at terminal.
            return False

        if action == "left" and (state.x - 1, state.y) in self.goal_locs:
            return True
        elif action == "right" and (state.x + 1, state.y) in self.goal_locs:
            return True
        elif action == "down" and (state.x, state.y - 1) in self.goal_locs:
            return True
        elif action == "up" and (state.x, state.y + 1) in self.goal_locs:
            return True
        else:
            return False

    def _transition_func(self, state, action):
        '''
        Args:
            state (State)
            action (str)

        Returns
            (State)
        '''
        if state.is_terminal():
            return state

        if not(self._is_goal_state_action(state, action)):
            # Flip dir.
            if action == "up":
                action = random.choice(["left", "right"])
            elif action == "down":
                action = random.choice(["left", "right"])
            elif action == "left":
                action = random.choice(["up", "down"])
            elif action == "right":
                action = random.choice(["up", "down"])

        if action == "up" and state.y < self.height:
            next_state = SimpleState(state.x, state.y + 1)
        elif action == "down" and state.y > 1:
            next_state = SimpleState(state.x, state.y - 1)
        elif action == "right" and state.x < self.width:
            next_state = SimpleState(state.x + 1, state.y)
        elif action == "left" and state.x > 1:
            next_state = SimpleState(state.x - 1, state.y)
        else:
            next_state = SimpleState(state.x, state.y)

        landed_in_term_goal = (
            next_state.x, next_state.y) in self.goal_locs and self.is_goal_terminal
        if landed_in_term_goal:
            next_state.set_terminal(True)

        return next_state

    def __str__(self):
        return "simple mdp"

    def __repr__(self):
        return self.__str__()

    def get_goal_locs(self):
        return self.goal_locs

    def reset(self):
        self.cur_state = copy.deepcopy(self.init_state)
