# Day 4 Analysis: Grid Removal Problem

## Problem Summary
Given a grid where `@` represents marked cells and `.` represents empty cells:
- **Part 1:** Count cells that have fewer than 4 marked neighbors (in 8 directions)
- **Part 2:** Remove cells iteratively - when a cell has <4 neighbors, remove it and check if its neighbors now also qualify for removal. Count total removals.

---

## Your Current Solution Analysis

### Part 1: Simple Neighbor Counting
```csharp
int adjacentCount = 0;
ForEachNeighbor(row, col, (newRow, newCol) =>
{
    if (IsMarked(grid, newRow, newCol))
    {
        adjacentCount++;
    }
});

if (adjacentCount < 4)
{
    candidate++;
}
```

**Time:** O(n·m) where n×m is grid dimensions (each cell checked once, 8 neighbors per cell)  
**Space:** O(n·m) for the grid  
**Status:** ✅ Correct and straightforward

### Part 2: Queue-based Cascading Removal
```csharp
var toProcess = new Queue<Cell>();
var visited = new HashSet<Cell>();

// Initial pass: find all candidates
for (int row = 0; row < grid.Length; row++)
{
    for (int col = 0; col < grid[row].Length; col++)
    {
        if (adjacentCount < 4)
        {
            toProcess.Enqueue(new Cell(row, col));
        }
    }
}

// Process removals
while (toProcess.Count > 0)
{
    Cell cell = toProcess.Dequeue();
    if (visited.Contains(cell) || !grid[cell.Row][cell.Col])
        continue;
    
    grid[cell.Row][cell.Col] = false;
    visited.Add(cell);
    result++;
    
    // Re-evaluate neighbors
    ForEachNeighbor(cell.Row, cell.Col, (newRow, newCol) => { ... });
}
```

**Time:** O(n·m·k) where k = average re-evaluations per cell  
**Space:** O(n·m) for grid + O(removals) for queue/visited  
**Status:** ✅ Correct algorithm

---

## Issues & Inefficiencies

### 🐛 Bug #1: Potential Double Processing
**Issue:** Lines 88-91 have a check for `visited.Contains(cell)`, which is good, but there's a subtle inefficiency.

**Problem:** A cell can be enqueued multiple times before being processed:
1. Cell A is enqueued
2. Cell B is processed, cell A is enqueued again (neighbor of B)
3. Cell A is dequeued first time → processed
4. Cell A is dequeued second time → skipped via `visited` check

**Impact:** Queue can grow unnecessarily large with duplicate entries.

**Fix:** Check visited status before enqueueing:
```csharp
// BEFORE
toProcess.Enqueue(new Cell(newRow, newCol));

// AFTER
Cell neighborCell = new Cell(newRow, newCol);
if (!visited.Contains(neighborCell) && !toProcess.Contains(neighborCell))
{
    toProcess.Enqueue(neighborCell);
}
```

**However**, `Queue<T>.Contains()` is O(n), so this creates a worse problem!

**Better Fix:** Use `HashSet<Cell>` for the queue instead:
```csharp
var toProcess = new HashSet<Cell>();

// Enqueuing becomes:
if (!visited.Contains(neighborCell))
{
    toProcess.Add(neighborCell);
}

// Processing becomes:
while (toProcess.Count > 0)
{
    Cell cell = toProcess.First();
    toProcess.Remove(cell);
    // ... rest of logic
}
```

### 🔧 Optimization #1: Combine Visited and Queue
**Current approach:** Separate `visited` HashSet and `toProcess` Queue

**Problem:** Duplicate tracking - we're tracking cells in two places

**Better approach:** Use a single `HashSet<Cell>` for pending cells, mark as visited by removing from grid:
```csharp
var toProcess = new HashSet<Cell>();

// ... initial population ...

while (toProcess.Count > 0)
{
    Cell cell = toProcess.First();
    toProcess.Remove(cell);
    
    if (!grid[cell.Row][cell.Col])
        continue; // Already removed
    
    grid[cell.Row][cell.Col] = false;
    result++;
    
    // Check neighbors and add to toProcess
}
```

**Benefit:** Eliminates one HashSet, reduces memory usage

### 🔧 Optimization #2: Avoid Grid Materialization
**Current:** Materialize entire grid into memory with `ToArrayAsync()`

```csharp
private static async Task<bool[][]> ConstructGridAsync(IAsyncEnumerable<string?> lines)
{
    return [.. (await lines.ToArrayAsync()).Select(line => line!.Select(c => c == '@').ToArray())];
}
```

**Issue:** This is actually fine for this problem! Grid must be mutable and randomly accessible. The only minor improvement would be readability:

```csharp
private static async Task<bool[][]> ConstructGridAsync(IAsyncEnumerable<string?> lines)
{
    var linesList = await lines.ToListAsync();
    return linesList
        .Select(line => line!.Select(c => c == '@').ToArray())
        .ToArray();
}
```

### 🔧 Optimization #3: Cache Neighbor Count During Initial Scan
**Current:** Part 2 scans entire grid, counts neighbors for each cell

**Better:** Maintain a neighbor count array to avoid recomputing:

```csharp
int[][] neighborCounts = new int[grid.Length][];
for (int i = 0; i < grid.Length; i++)
    neighborCounts[i] = new int[grid[i].Length];

// Build neighbor counts
for (int row = 0; row < grid.Length; row++)
{
    for (int col = 0; col < grid[row].Length; col++)
    {
        if (!grid[row][col]) continue;
        
        int count = 0;
        ForEachNeighbor(row, col, (r, c) =>
        {
            if (IsMarked(grid, r, c)) count++;
        });
        neighborCounts[row][col] = count;
        
        if (count < 4)
            toProcess.Add(new Cell(row, col));
    }
}

// When removing a cell, update neighbor counts
grid[cell.Row][cell.Col] = false;
ForEachNeighbor(cell.Row, cell.Col, (r, c) =>
{
    if (IsMarked(grid, r, c))
    {
        neighborCounts[r][c]--;
        if (neighborCounts[r][c] < 4)
            toProcess.Add(new Cell(r, c));
    }
});
```

**Benefit:** Changes neighbor re-evaluation from O(8) to O(1) per update

---

## Alternative Approaches

### Approach 1: Your Current Solution (Good Balance)
**Time:** O(n·m·k) where k = average re-evaluations  
**Space:** O(n·m)

**Pros:**
- Clear and readable
- Straightforward logic
- Correct results

**Cons:**
- Redundant neighbor counting
- Potential duplicate queue entries
- Two separate tracking structures

### Approach 2: Optimized with Neighbor Count Cache
```csharp
public async Task<int> SolvePartTwoOptimized(IAsyncEnumerable<string?> lines)
{
    bool[][] grid = await ConstructGridAsync(lines);
    int[][] neighborCount = new int[grid.Length][];
    var toRemove = new HashSet<Cell>();
    
    // Initialize neighbor counts
    for (int row = 0; row < grid.Length; row++)
    {
        neighborCount[row] = new int[grid[row].Length];
        for (int col = 0; col < grid[row].Length; col++)
        {
            if (!grid[row][col]) continue;
            
            int count = CountNeighbors(grid, row, col);
            neighborCount[row][col] = count;
            
            if (count < 4)
                toRemove.Add(new Cell(row, col));
        }
    }
    
    int removed = 0;
    while (toRemove.Count > 0)
    {
        Cell cell = toRemove.First();
        toRemove.Remove(cell);
        
        if (!grid[cell.Row][cell.Col])
            continue;
        
        grid[cell.Row][cell.Col] = false;
        removed++;
        
        // Decrement neighbor counts and check for new candidates
        ForEachNeighbor(cell.Row, cell.Col, (r, c) =>
        {
            if (!IsMarked(grid, r, c)) return;
            
            neighborCount[r][c]--;
            if (neighborCount[r][c] < 4)
                toRemove.Add(new Cell(r, c));
        });
    }
    
    return removed;
}

private static int CountNeighbors(bool[][] grid, int row, int col)
{
    int count = 0;
    ForEachNeighbor(row, col, (r, c) =>
    {
        if (IsMarked(grid, r, c)) count++;
    });
    return count;
}
```

**Time:** O(n·m) - each cell processed at most once  
**Space:** O(n·m) for grid + neighbor counts  
**Pros:** More efficient, no redundant counting  
**Cons:** More complex, higher memory usage

### Approach 3: Recursive DFS (Don't Use!)
```csharp
private void RemoveCell(bool[][] grid, int row, int col, ref int count)
{
    if (!IsMarked(grid, row, col)) return;
    
    int neighbors = CountNeighbors(grid, row, col);
    if (neighbors >= 4) return;
    
    grid[row][col] = false;
    count++;
    
    // Recursively check all neighbors
    ForEachNeighbor(row, col, (r, c) => RemoveCell(grid, r, c, ref count));
}
```

**Time:** O(n·m) but with recursion overhead  
**Space:** O(n·m) call stack in worst case  
**Pros:** Simple concept  
**Cons:** Stack overflow risk, recounts neighbors repeatedly, incorrect logic (doesn't wait for removal to affect count)

### Approach 4: Priority Queue by Neighbor Count
```csharp
var pq = new PriorityQueue<Cell, int>();

// Prioritize cells with fewer neighbors first
for (int row = 0; row < grid.Length; row++)
{
    for (int col = 0; col < grid[row].Length; col++)
    {
        if (!grid[row][col]) continue;
        int count = CountNeighbors(grid, row, col);
        if (count < 4)
            pq.Enqueue(new Cell(row, col), count);
    }
}
```

**Time:** O(n·m·log(removals))  
**Space:** O(n·m)  
**Pros:** Processes cells in "most isolated" order  
**Cons:** Slower than HashSet, doesn't improve correctness or significantly change behavior

---

## Comparison Table

| Approach | Time | Space | Readability | Efficiency |
|----------|------|-------|-------------|------------|
| **Your Solution** | O(n·m·k) | O(n·m) | High | Medium |
| Neighbor Count Cache | O(n·m) | O(2·n·m) | Medium | High |
| Recursive DFS | O(n·m) | O(n·m) stack | Low | Low (overhead) |
| Priority Queue | O(n·m·log(n·m)) | O(n·m) | Medium | Low |

---

## Recommendations

### Critical Fixes:
1. ✅ **Eliminate duplicate enqueuing** - Switch from `Queue` to `HashSet` for `toProcess`:
   ```csharp
   var toProcess = new HashSet<Cell>();
   // Use First() and Remove() instead of Dequeue()
   ```

2. ✅ **Simplify visited tracking** - Remove separate `visited` HashSet, just check grid state:
   ```csharp
   if (!grid[cell.Row][cell.Col])
       continue; // Already removed
   ```

### Performance Improvements:
3. ✅ **Add neighbor count caching** - For large grids with many removals, cache counts:
   - Adds O(n·m) space
   - Reduces time from O(n·m·k) to O(n·m)
   - Worth it if k > 2 (i.e., cells are re-evaluated multiple times)

### Code Quality:
4. ✅ **Extract methods** - Your helper methods are good! Consider adding:
   ```csharp
   private static int CountMarkedNeighbors(bool[][] grid, int row, int col)
   {
       int count = 0;
       ForEachNeighbor(row, col, (r, c) =>
       {
           if (IsMarked(grid, r, c)) count++;
       });
       return count;
   }
   ```

---

## Complexity Analysis Deep Dive

### Part 1 Complexity:
- **Best/Worst/Average:** O(n·m) - must visit every cell once
- **Space:** O(n·m) for grid storage
- **Optimal:** Yes, can't do better than visiting each cell

### Part 2 Complexity:
**Your current implementation:**
- **Time:** O(n·m·k) where k = average re-evaluations per cell
  - Initial scan: O(n·m)
  - Each removal: O(8) to check neighbors
  - Each neighbor check: O(8) to recount its neighbors
  - If r cells removed: O(r·8·8) = O(64r) = O(r)
  - Worst case: r = n·m, so O(n·m)
  - **Actually O(n·m·d)** where d = average degree (neighbors) per removed cell

**With neighbor count cache:**
- **Time:** O(n·m)
  - Initial scan: O(n·m)
  - Each removal: O(8) to update neighbors
  - Total: O(n·m + 8r) = O(n·m)

**Space:**
- Your solution: O(n·m) grid + O(r) queue + O(r) visited = O(n·m)
- Optimized: O(n·m) grid + O(n·m) counts + O(r) set = O(n·m)

---

## Key Insights

1. **Queue vs HashSet:** Using `Queue<Cell>` allows duplicates, causing redundant processing. `HashSet<Cell>` prevents this.

2. **Cascading Effect:** The problem has a cascading nature - removing one cell can trigger removal of neighbors. Your BFS approach handles this correctly.

3. **Visited Tracking:** Maintaining a separate `visited` set is redundant when you're already marking cells as removed in the grid.

4. **Neighbor Counting:** Re-counting neighbors is the main inefficiency. Caching counts converts this from O(8) per check to O(1) per decrement.

5. **Grid Boundaries:** Your `IsMarked` method correctly handles boundary checks, preventing index out of bounds errors.

6. **Record Struct:** Using `record struct Cell` is perfect for this use case - value semantics with auto-equality.

---

## Test Coverage Analysis

Your test uses sample input with expected results:
- Part 1: 13 cells with <4 neighbors
- Part 2: 43 total removals

**Missing test cases:**
1. ✅ **Empty grid** - should return 0
2. ✅ **Single cell** - has 0 neighbors, should be counted/removed
3. ✅ **Full grid** - all cells have 8 neighbors, nothing qualifies
4. ✅ **Edge cells** - have fewer potential neighbors (5 neighbors)
5. ✅ **Corner cells** - have 3 potential neighbors
6. ✅ **Chain reaction** - removing one cell triggers cascade

---

## Final Verdict

**Your solution is correct and well-structured!** The main inefficiency is redundant neighbor counting in Part 2.

### Severity of Issues:
- 🟡 **Medium:** Duplicate queue entries (functional but wasteful)
- 🟢 **Low:** Redundant visited HashSet (minor memory waste)
- 🟢 **Low:** Re-counting neighbors (acceptable for small grids)

### Recommended Changes:
1. **Quick fix (5 min):** Switch `Queue` to `HashSet`, remove `visited` set
2. **Better fix (15 min):** Add neighbor count caching for larger inputs

**Overall Grade: 8.5/10**
- Correctness: ✅ Perfect
- Code Quality: ✅ Clean, readable
- Performance: 🟡 Good, could be better
- Optimization Level: Medium (optimal for small grids, improvable for large ones)
