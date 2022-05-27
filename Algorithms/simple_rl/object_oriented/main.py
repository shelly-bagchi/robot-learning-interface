#!/usr/bin/env python
from simple_rl.agents import QLearningAgent, RandomAgent, RMaxAgent
from robot_learning_mdp import RobotLearningMDP
from simple_rl.run_experiments import run_agents_on_mdp

from task import Task
from block import Block
from gridcell import GridCell

task = Task("green", "cell5", 2, 1, "block1")
block1 = Block("block1", 1, 0, color="green")
block2 = Block("block2", 0, 0, color="blue")
block3 = Block("block3", 0, 1, color="orange")
cells = []

n = 1
for x in range(1, 5):
    for y in range(1, 5):
        cell = GridCell("cell" + str(n), [(x, y)])
        cells.append(cell)
        n += 1

blocks = [block1, block2, block3]

mdp = RobotLearningMDP(task=task, cells=cells, blocks=blocks)

# Setup Agents.
ql_agent = QLearningAgent(actions=mdp.get_actions())
rmax_agent = RMaxAgent(actions=mdp.get_actions())
rand_agent = RandomAgent(actions=mdp.get_actions())

# Run experiment and make plot.
run_agents_on_mdp([ql_agent, rmax_agent, rand_agent],
                  mdp, instances=5, episodes=50, steps=10)
