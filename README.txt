How does A* pathfinding calculate and prioritize paths?
A* pathfinding is able to calculate the shortest possible paths using heuristic estimates. It prioritizes the shortest possible path based on the distance from the current position to the next unobstructed position closest to the exit. The shorter estimated distance will be prioritized first.

What challenges arise when dynamically updating obstacles in real-time?
When dynamically updating obstacles in real time, obstacles may include a path not being found. In addition, obstacles spawning on top of the start or exit will block all possible pathways, creating impossible solutions.

How could you adapt this code for larger grids or open-world settings?
This code can be adapted to larger grids by allowing for customizable entries to the width and height parameters in the array and functions that use its values. For an open-world setting, if the code cannot find a path, it would employ another pathfinding algorithm, such as Greedy Best-First. If it can find a path, it will draw it.

What would your approach be if you were to add weighted cells (e.g., "difficult terrain" areas)?
If weighted cells were to be added that slowed the player down but did not stop them altogether, a speed variable could be added alongside a new type of cell. In this code, free spaces are represented as 0 and obstacles are represented by 1. A new cell type, represented by 2, would be a cell that is traversable, but it would lower speed when crossed over. Therefore, the cell type should only be considered when other paths to the goal are blocked, which would make the algorithm prioritize the most consistently high speed when calculating the way out.