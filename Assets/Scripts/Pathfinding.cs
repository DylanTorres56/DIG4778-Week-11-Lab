using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private List<Vector2Int> path = new List<Vector2Int>();

    [Header("Start and Goal Positions")]
    [Tooltip("Location of Start (green square)")]
    // The starting position of the player. Users can manually set this in the Unity editor.
    [SerializeField] private Vector2Int start = new Vector2Int(0, 1);

    [Tooltip("Location of Goal (red square)")]
    // The position of the goal. Users can manually set this in the Unity editor.
    [SerializeField] private Vector2Int goal = new Vector2Int(4, 4);

    private Vector2Int next;
    private Vector2Int current;

    private Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    // This int array creates a grid, with open spaces represented with a value of 0 and obstacles as 1.
    private int[,] grid = new int[,]
    {
        { 0, 1, 0, 0, 0 },
        { 0, 1, 0, 1, 0 },
        { 0, 0, 0, 1, 0 },
        { 0, 1, 1, 1, 0 },
        { 0, 0, 0, 0, 0 }
    };

    [Tooltip("Must be between 0 and 100")]
    [Header("Chance of Square Being an Obstacle")]
    public float obstacleProbability = 25f;

    [SerializeField]
    [HideInInspector]
    private float obstacleProbChanged;

    [Header("Add New Obstacle Location")]
    public Vector2Int obstacleLocation;

    private void Start()
    {
        GenerateRandomGrid(grid.GetLength(0), grid.GetLength(1), obstacleProbability);
        path.Clear();
        FindPath(start, goal);
    }

    // When a value in the inspector is changed, clear the path and find a new one.
    private void OnValidate()
    {
        if (obstacleProbChanged != obstacleProbability)
        {
            RandomGridSpawn();
            obstacleProbChanged = obstacleProbability;
            GenerateRandomGrid(grid.GetLength(0), grid.GetLength(1), obstacleProbability);
        }

        AddObstacle(obstacleLocation);

        path.Clear();
        FindPath(start, goal);
    }

    private void OnDrawGizmos()
    {
        float cellSize = 1f;

        // Draw grid cells
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.color = grid[y, x] == 1 ? Color.black : Color.white;
                Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
            }
        }

        // Draw path
        foreach (var step in path)
        {
            Vector3 cellPosition = new Vector3(step.x * cellSize, 0, step.y * cellSize);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(cellPosition, new Vector3(cellSize, 0.1f, cellSize));
        }

        // Draw start
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(start.x * cellSize, 0, start.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));

        // Draw goal
        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goal.x * cellSize, 0, goal.y * cellSize), new Vector3(cellSize, 0.1f, cellSize));
    }

    // This bool checks if a grid space is viable by way of it staying in the bounds of the grid.
    private bool IsInBounds(Vector2Int point)
    {
        return point.x >= 0 && point.x < grid.GetLength(1) && point.y >= 0 && point.y < grid.GetLength(0);
    }

    // This function draws a path from start to finishm, enqueueing spaces where the player already came from so it doesn't backtrack.
    private void FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            current = frontier.Dequeue();

            if (current == goal)
            {
                break;
            }

            foreach (Vector2Int direction in directions)
            {
                next = current + direction;

                if (IsInBounds(next) && grid[next.y, next.x] == 0 && !cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        if (!cameFrom.ContainsKey(goal))
        {
            Debug.Log("Path not found.");
            return;
        }

        // Trace path from goal to start
        Vector2Int step = goal;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
    }

    // Randomly generates obstacles around the grid based on a probability.
    private void GenerateRandomGrid(int height, int width, float obstacleProbability)
    {
        grid = new int[height, width];

        // Go through each cell and randomly assign an obstacle based on our probability.
        for (int y = 0; y < grid.GetLength(0); y++)
        {
            for (int x = 0; x < grid.GetLength(1); x++)
            {
                // If the coordinate is equal to the start or the goal, set it to 0 and continue to the next iteration.
                if (x == start.x && y == start.y || x == goal.x && y == goal.y)
                {
                    grid[y, x] = 0;
                    continue;
                }

                // If the square has a higher probability of being an obstacle than our random percentage, make it an obstacle.
                // Else, make it an open space.
                grid[y, x] = obstacleProbability > Random.Range(1, 100) ? 1 : 0;
            }
        }
    }

    private void RandomGridSpawn()
    {
        int width = Random.Range(4, 23);
        int height = Random.Range(4, 23);

        grid = new int[height, width];

    }

    // Adds an obstacle at the inputted position.
    private void AddObstacle(Vector2Int position)
    {
        // If the coordinate is equal to the start or the goal, set it to 0 and continue to the next iteration.
        if (position.x == start.x && position.y == start.y || position.x == goal.x && position.y == goal.y)
        {
            grid[position.y, position.x] = 0;
        }
        else
        {
            // Set the inputted grid position to an obstacle (1).
            grid[position.y, position.x] = 1;
        }        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Pathfinding)), CanEditMultipleObjects]
public class PathfindingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Use the base layout of the avoider script.
        base.OnInspectorGUI();

        RequirementChecks();
    }

    private void RequirementChecks()
    {
        float obstacleProbability = Selection.activeGameObject.GetComponent<Pathfinding>().obstacleProbability;

        if (obstacleProbability > 100)
        {
            Selection.activeGameObject.GetComponent<Pathfinding>().obstacleProbability = 100;
        }
        else if (obstacleProbability < 0)
        {
            Selection.activeGameObject.GetComponent<Pathfinding>().obstacleProbability = 0;
        }
    }
}
#endif