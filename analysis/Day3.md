# Algorithm Comparison for Day 3 Problem

## Problem
Select N digits from a string to form the largest N-digit number (maintaining order)
Example: `"818181911112111"`, N=12 → `"888911112111"`

---

## 1. **Recursive Greedy** (Your Current Solution - Optimized)

```csharp
public static long Solve(string input, int n)
{
    Bank[] banks = Parse(input);
    return GetLargest(banks, 0, new List<Bank>(), n);
}

private static long GetLargest(Bank[] banks, int startIndex, List<Bank> selected, int n)
{
    if (n == 0)
        return long.Parse(string.Concat(selected.Select(b => b.Power)));

    int candidatesEnd = banks.Length - n + 1;
    Bank highest = banks[startIndex];
    
    for (int i = startIndex + 1; i < candidatesEnd; i++)
        if (banks[i].Power > highest.Power)
            highest = banks[i];

    selected.Add(highest);
    return GetLargest(banks, highest.Index + 1, selected, n - 1);
}
```

**Time:** O(n·k) - For each of k positions, scan up to n candidates  
**Space:** O(k) - Recursion stack + selected list  
**Pros:** Simple, intuitive  
**Cons:** Stack overflow risk for large k

---

## 2. **Iterative Greedy** (No Recursion)

```csharp
public static long Solve(string input, int n)
{
    Bank[] banks = Parse(input);
    var selected = new List<Bank>(n);
    int startIndex = 0;

    while (selected.Count < n)
    {
        int remaining = n - selected.Count;
        int candidatesEnd = banks.Length - remaining + 1;
        
        Bank highest = banks[startIndex];
        for (int i = startIndex + 1; i < candidatesEnd; i++)
            if (banks[i].Power > highest.Power)
                highest = banks[i];

        selected.Add(highest);
        startIndex = highest.Index + 1;
    }

    return long.Parse(string.Concat(selected.Select(b => b.Power)));
}
```

**Time:** O(n·k) - Same as recursive  
**Space:** O(k) - Only selected list, no stack  
**Pros:** No stack overflow, slightly faster  
**Cons:** None - strictly better than recursive

---

## 3. **Monotonic Stack** (OPTIMAL! 🏆)

```csharp
public static long Solve(string input, int k)
{
    int n = input.Length;
    int toRemove = n - k;  // How many digits to skip
    var stack = new Stack<char>();

    for (int i = 0; i < n; i++)
    {
        // Pop smaller digits while we can afford to remove them
        while (stack.Count > 0 && stack.Peek() < input[i] && toRemove > 0)
        {
            stack.Pop();
            toRemove--;
        }
        stack.Push(input[i]);
    }

    // Remove excess from end
    while (toRemove > 0)
    {
        stack.Pop();
        toRemove--;
    }

    return long.Parse(new string(stack.Reverse().ToArray()));
}
```

**Time:** O(n) - Each element pushed/popped once  
**Space:** O(k) - Stack bounded by k  
**Pros:** LINEAR TIME! Most efficient  
**Cons:** Less intuitive

### Monotonic Stack Walkthrough
Input: `"818181911112111"`, k=12 (remove 3)

```
i=0, '8': [] → ['8']
i=1, '1': ['8'] → ['8','1']
i=2, '8': 8>'1' & canRemove → pop '1' → ['8','8']
i=3, '1': ['8','8'] → ['8','8','1']
i=4, '8': 8>'1' & canRemove → pop '1' → ['8','8','8']
i=5, '1': ['8','8','8'] → ['8','8','8','1']
i=6, '9': 9>'1' & canRemove → pop '1' → ['8','8','8','9']
i=7-14: toRemove=0, just push → ['8','8','8','9','1','1','1','1','2','1','1','1']

Result: "888911112111" ✓
```

---

## 4. **Dynamic Programming** (Overkill)

```csharp
public static long Solve(string input, int k)
{
    int n = input.Length;
    // dp[i][j] = max number using j digits from positions 0..i-1
    var dp = new string[n + 1, k + 1];
    
    dp[0, 0] = "";
    
    for (int i = 1; i <= n; i++)
    {
        for (int j = 0; j <= Math.Min(i, k); j++)
        {
            // Don't take digit i-1
            if (dp[i-1, j] != null)
                dp[i, j] = dp[i-1, j];
            
            // Take digit i-1
            if (j > 0 && dp[i-1, j-1] != null)
            {
                string candidate = dp[i-1, j-1] + input[i-1];
                if (dp[i, j] == null || Compare(candidate, dp[i, j]) > 0)
                    dp[i, j] = candidate;
            }
        }
    }
    
    return long.Parse(dp[n, k]);
}
```

**Time:** O(n²·k) - String comparisons for each state  
**Space:** O(n·k) - DP table  
**Pros:** Handles variants with complex constraints  
**Cons:** Massive overkill, very slow

---

## 5. **Priority Queue + Sliding Window**

```csharp
public static long Solve(string input, int k)
{
    int n = input.Length;
    var result = new StringBuilder();
    int startIdx = 0;

    for (int pos = 0; pos < k; pos++)
    {
        int windowEnd = n - (k - pos) + 1;
        
        // Find max in window using PQ
        var pq = new PriorityQueue<(char digit, int idx), (char, int)>(
            Comparer<(char, int)>.Create((a, b) => 
                b.Item1 != a.Item1 ? b.Item1.CompareTo(a.Item1) : a.Item2.CompareTo(b.Item2))
        );

        for (int i = startIdx; i < windowEnd; i++)
            pq.Enqueue((input[i], i), (input[i], i));

        var best = pq.Dequeue();
        result.Append(best.digit);
        startIdx = best.idx + 1;
    }

    return long.Parse(result.ToString());
}
```

**Time:** O(n·log k) - PQ operations  
**Space:** O(k) - Priority queue  
**Pros:** Works well with streaming data  
**Cons:** Unnecessary complexity

---

## **Complexity Comparison Table**

| Algorithm | Time | Space | Best For |
|-----------|------|-------|----------|
| **Monotonic Stack** | **O(n)** | O(k) | 🏆 **Production - fastest!** |
| **Iterative Greedy** | O(n·k) | O(k) | **AoC - clear & fast enough** |
| Recursive Greedy | O(n·k) | O(k) | Learning - most intuitive |
| Priority Queue | O(n·log k) | O(k) | Streaming data |
| Dynamic Programming | O(n²·k) | O(n·k) | Complex constraint variants |

---

## **Recommendation**

**For your AoC solution:** Keep **Iterative Greedy** (or current recursive) - perfect balance of clarity and performance

**To impress/optimize:** Switch to **Monotonic Stack** - it's the "correct" algorithm theorists would use, and it's O(n)!

**Why Greedy works:** Earlier digit positions have exponentially more value (98... > 89... always), so picking the best at each position guarantees global optimum.
