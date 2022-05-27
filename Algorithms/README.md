# ML Algorithms for Robot Learning Interface

This project is built upon previous work that created a robot teaching interface where
goals can be given to the robot arm to have it go through a task. The aim is to add learning algorithm support to this interface, so multiple demonstrations can be given by the human partner, and the robot will learn the constraints and goals of the task over time.

## Sequential Prediction with LSTM Recurrent Neural Network

Long Short-Term Memory (LSTM) networks are a type of recurrent neural network capable of learning order dependence in sequence prediction problems. In this project, they are used to predict the trajectory of the robotic arm given a sequence of past coordinates and joint degrees. Using the data generation function in the Unity interface, users can create csv files with sequences of robotic joint paramters and target coordinates as inputs to this model (note: to specify the number of steps in the sequence and the number of sequences, edit the paramaters in the script FullButton.cs).

**[The model is located in this Google Colab notebook](https://colab.research.google.com/drive/1YxsktR52RiBYRSBAcFBra5PdNn0dpEmY?usp=sharing)**

### Running the Colab notebook:

1. Import a csv file containing the robotic joint degrees
2. Rename it to "joints.csv"
3. In the top menu, navigate to Runtime -> Run all
4. Each section in the notebook represents a step in processing the data and creating the model. To run a step individually, hover over a section and click the run button in the top left corner.

### Next Steps:

- Currently the model only trains on robotic joint degrees. A next step would be to include the target coordinates corresponding to the joint degrees as an input. The target coordinates are auto-generated along side the joint degrees as a csv file under Assets, but you would need to combine the two csv files.
- As part of data pre-processing, the splitSequence function currently splits the dataset incrementally (ex. [1, 2, 3, 4] -> [[[1, 2], [2, 3]], [3, 4]]), but the data generation function in Unity records n sequences of n_past steps between random coordinates. These n sequences are not connected (they do not create a larger sequence), and so there should be changes made to this function to reflect that separation of sequences. (this to do is noted in the colab notebook)

## Reinforcement Learning

The reinforcement learning environment is developed with the simple_rl library, which is a framework for running experiments where RL agents interact with an MDP.

There are two mdps created to simulate the robot learning environment.

One is a simplified version, in which the robot is assumed to be holding the block already and the task is to navigate to the goal location. You can specify the goal coordinates as a parameter to the mdp.

The other is an object-oriented version. The robot is assigned a task to pick up a block of a certain color and move it to a specific grid cell. You can create a task object specifying the block and the location to move it to in the parameters. See an example in _simple_rl/object_oriented/main.py_.

The general instructions for experimenting with the simple_rl library are:

1. Create an MDP.
2. Create agents.
3. Set experiment parameters (instances, episodes, steps).
4. Call run_agents_on_mdp(agents, mdp) -> runs all experiments and will open a plot with results when finished.

For more information, check out the [simple_rl repository](https://github.com/david-abel/simple_rl).

### Set Up:

simple_rl requires numpy and matplotlib. You can install numpy following the guide [here](https://numpy.org/install/) and matplotlib [here](https://matplotlib.org/stable/users/installing/index.html)

### Running simple_rl:

1. cd into the _simple_rl/object_oriented_ or _simple_rl/simplified_ folder
2. run `python main.py` in terminal

### Next Steps:

- The object-oriented mdp currently isn't compatible with the RMaxAgent. There is an error regarding the dictionary changing size during the iteration of updating the state.
- There can only be one task assigned to the robot. A future task would be implement the ability to add multiple tasks to the robot.
