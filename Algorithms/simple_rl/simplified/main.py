from simple_mdp import SimpleRobotLearningMDP
from simple_rl.agents import QLearningAgent, RandomAgent, RMaxAgent
from simple_rl.run_experiments import run_agents_on_mdp

mdp = SimpleRobotLearningMDP(width=4, height=4, init_loc=(1, 1), goal_locs=[(
    2, 3)], gamma=0.95, step_cost=1)

# Setup Agents.
ql_agent = QLearningAgent(actions=mdp.get_actions())
rmax_agent = RMaxAgent(actions=mdp.get_actions())
rand_agent = RandomAgent(actions=mdp.get_actions())

run_agents_on_mdp([ql_agent, rmax_agent, rand_agent], mdp)
