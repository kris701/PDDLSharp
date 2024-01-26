# RESULTS ARE IN DEBUG MODE
# PDDLSharp Performance
Here are some general statistics about how good the PDDLSharp system performs.


These benchmarks made on the domains from [Fast Downward](https://github.com/aibasel/downward-benchmarks/):
* gripper
* blocks
* depot
* miconic
* rovers
* trucks
* zenotravel

# Core Components
## PDDL
| Name | Iterations | Total Size (MB) | Total Time (s) | Throughput (MB/s) |
| - | - | - | - | - |
| PDDL Domain Parsing | 1 | 0.01 | 0.07 | 0.14 |
| PDDL Problem Parsing | 1 | 0.05 | 0.42 | 0.12 |
| PDDL Contextualization | 1 | 0.13 | 1.62 | 0.08 |
| PDDL Analysing | 1 | 0.13 | 0.29 | 0.45 |
| PDDL Domain Code Generation | 1 | 0.01 | 0.05 | 0.21 |
| PDDL Problem Code Generation | 1 | 0.05 | 0.25 | 0.2 |


## Fast Downward SAS
| Name | Iterations | Total Size (MB) | Total Time (s) | Throughput (MB/s) |
| - | - | - | - | - |
| Fast Downward SAS Parsing | 1 | 1.5 | 2.42 | 0.62 |


## Fast Downward Plans
| Name | Iterations | Total Size (MB) | Total Time (s) | Throughput (MB/s) |
| - | - | - | - | - |
| Fast Downward Plan Parsing | 1 | 0.03 | ∞ | 0 |


## Translation
| Domain | Problems | Iterations | Total Operators | Operators / second | Total Time (s) |
| - | - | - | - | - | - |
| blocks | 6 | 1 | 1452 | 1770.73 | 0.82 |
| depot | 6 | 1 | 12168 | 1843.64 | 6.6 |
| gripper | 6 | 1 | 456 | 1036.36 | 0.44 |
| miconic | 6 | 1 | 420 | 893.62 | 0.47 |
| rovers | 6 | 1 | 908 | 945.83 | 0.96 |
| trucks | 6 | 1 | 7344 | 1995.65 | 3.68 |
| zenotravel | 6 | 1 | 1784 | 1074.7 | 1.66 |


# Toolkit Components
## Planner (Classical, 30s time limit)
| Domain | Planner | Problems | Iterations | Generated / s | Expansions / s | Evaluations / s | Solved (%) | Total Time (s) |
| - | - | - | - | - | - | - | - | - |
| blocks | Greedy Best First (hGoal) | 6 | 1 | 21900.78 | 7090.7 | 12408.53 | 100 | 1.29 |
| blocks | Greedy Best First (hFF) | 6 | 1 | 1286.99 | 359.78 | 813.21 | 100 | 10.22 |
| blocks | Greedy Best First (hMax) | 6 | 1 | 3153.71 | 427.92 | 2380.55 | 83.33 | 60.31 |
| depot | Greedy Best First (hGoal) | 6 | 1 | 5783.18 | 585.14 | 1910.48 | 83.33 | 65.28 |
| depot | Greedy Best First (hFF) | 6 | 1 | 300.26 | 31.68 | 94.74 | 66.67 | 79.25 |
| depot | Greedy Best First (hMax) | 6 | 1 | 604.69 | 52.06 | 323.98 | 33.33 | 120.76 |
| gripper | Greedy Best First (hGoal) | 6 | 1 | 12380 | 1600 | 8513.33 | 100 | 0.15 |
| gripper | Greedy Best First (hFF) | 6 | 1 | 2321.31 | 300 | 1598.36 | 100 | 0.61 |
| gripper | Greedy Best First (hMax) | 6 | 1 | 5925.76 | 616.67 | 3030.3 | 100 | 1.32 |
| miconic | Greedy Best First (hGoal) | 6 | 1 | 7843.04 | 410.76 | 2337.97 | 100 | 1.58 |
| miconic | Greedy Best First (hFF) | 6 | 1 | 921.25 | 68.75 | 527.5 | 100 | 0.8 |
| miconic | Greedy Best First (hMax) | 6 | 1 | 2391.62 | 125.72 | 767.34 | 100 | 3.46 |
| rovers | Greedy Best First (hGoal) | 6 | 1 | 17254.3 | 990.32 | 4958.6 | 100 | 1.86 |
| rovers | Greedy Best First (hFF) | 6 | 1 | 1895.17 | 132.71 | 940.52 | 100 | 2.69 |
| rovers | Greedy Best First (hMax) | 6 | 1 | 6083.97 | 376.17 | 2000.26 | 83.33 | 37.94 |
| trucks | Greedy Best First (hGoal) | 6 | 1 | 19430.77 | 6700.4 | 7193.27 | 16.67 | 163.33 |
| trucks | Greedy Best First (hFF) | 6 | 1 | 209.93 | 10.57 | 179.02 | 83.33 | 56.38 |
| trucks | Greedy Best First (hMax) | 6 | 1 | 695.2 | 29.84 | 589.5 | 33.33 | 132.93 |
| zenotravel | Greedy Best First (hGoal) | 6 | 1 | 35952.38 | 2196.83 | 15987.3 | 100 | 0.63 |
| zenotravel | Greedy Best First (hFF) | 6 | 1 | 1558.44 | 111.69 | 1192.21 | 100 | 0.77 |
| zenotravel | Greedy Best First (hMax) | 6 | 1 | 3097.51 | 153.73 | 1737.1 | 100 | 8.84 |


