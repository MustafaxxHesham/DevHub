namespace DevHub.Domain.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Post>? Posts { get; set; }
}
/*
| Category            | Examples 
| ------------------- | ---------------------- |
| Sorting             | Quick Sort, Merge Sort |
| Searching           | Binary Search          |
| Recursion           | Factorial, Fibonacci   |
| Divide & Conquer    | Merge Sort             |
| Dynamic Programming | Knapsack               |
| Greedy              | Dijkstra               |
| Graph Algorithms    | BFS, DFS               |
| Backtracking        | N-Queens               |
| String Algorithms   | KMP                    |
| Bit Manipulation    | XOR tricks             |
*/