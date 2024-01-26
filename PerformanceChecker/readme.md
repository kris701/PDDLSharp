# PDDLSharp Performance
Here are some general statistics about how good the PDDLSharp system performs.


These benchmarks made on the following domains from [Fast Downward](https://github.com/aibasel/downward-benchmarks/):
* gripper
* blocks
* depot
* miconic
* rovers
* trucks
* zenotravel

For each of these domains, the first 5 problems are selected.
Each component is executed 3 times to get a better average.
# Core Components
## PDDL
| Name | Iterations | Total Size (MB) | Throughput (MB/s) | Total Time (s) |
| - | - | - | - | - |
| PDDL Domain Parsing | 3 | 0.036 | 0.275 | 0.131 |
| PDDL Problem Parsing | 3 | 0.162 | 0.931 | 0.174 |
| PDDL Contextualization | 3 | 0.38 | 1.863 | 0.204 |
| PDDL Analysing | 3 | 0.38 | 0.488 | 0.778 |
| PDDL Domain Code Generation | 3 | 0.033 | 0.189 | 0.175 |
| PDDL Problem Code Generation | 3 | 0.152 | 0.822 | 0.185 |


## Fast Downward SAS
| Name | Iterations | Total Size (MB) | Throughput (MB/s) | Total Time (s) |
| - | - | - | - | - |
| Fast Downward SAS Parsing | 3 | 4.511 | 4.67 | 0.966 |


## Fast Downward Plans
| Name | Iterations | Total Size (MB) | Throughput (MB/s) | Total Time (s) |
| - | - | - | - | - |
| Fast Downward Plan Parsing | 3 | 0.101 | ∞ | 0 |


## Translation
| Domain | Problems | Iterations | Total Operators | Operators / second | Total Time (s) |
| - | - | - | - | - | - |
| blocks | 6 | 3 | 4356 | 2779.834 | 1.567 |
| depot | 6 | 3 | 36504 | 2429.712 | 15.024 |
| gripper | 6 | 3 | 1368 | 1772.021 | 0.772 |
| miconic | 6 | 3 | 1260 | 1831.395 | 0.688 |
| rovers | 6 | 3 | 2724 | 1529.478 | 1.781 |
| trucks | 6 | 3 | 22032 | 2600.567 | 8.472 |
| zenotravel | 6 | 3 | 5352 | 1684.078 | 3.178 |


# Toolkit Components
## Planner (Classical, 30s time limit)
| Domain | Planner | Problems | Iterations | Generated / s | Expansions / s | Evaluations / s | Solved (%) | Total Time (s) |
| - | - | - | - | - | - | - | - | - |
| blocks | Greedy Best First (hGoal) | 18 | 3 | 26486.25 | 8575.312 | 15006.562 | 100 | 3.2 |
| blocks | Greedy Best First (hFF) | 18 | 3 | 2938.78 | 821.554 | 1856.93 | 100 | 13.427 |
| depot | Greedy Best First (hGoal) | 18 | 3 | 12664.309 | 1271.738 | 3841.508 | 83.333 | 126.795 |
| depot | Greedy Best First (hFF) | 18 | 3 | 670.954 | 71.796 | 203.97 | 66.667 | 200.373 |
| gripper | Greedy Best First (hGoal) | 18 | 3 | 27308.824 | 3529.412 | 18779.412 | 100 | 0.204 |
| gripper | Greedy Best First (hFF) | 18 | 3 | 3165.425 | 409.091 | 2179.583 | 100 | 1.342 |
| miconic | Greedy Best First (hGoal) | 18 | 3 | 8335.426 | 436.547 | 2484.753 | 100 | 4.46 |
| miconic | Greedy Best First (hFF) | 18 | 3 | 987.054 | 73.661 | 565.179 | 100 | 2.24 |
| rovers | Greedy Best First (hGoal) | 18 | 3 | 21743.225 | 1247.967 | 6248.645 | 100 | 4.428 |
| rovers | Greedy Best First (hFF) | 18 | 3 | 2861.366 | 200.374 | 1420.019 | 100 | 5.345 |
| trucks | Greedy Best First (hGoal) | 18 | 3 | 42128.997 | 13626.812 | 14286.722 | 16.667 | 467.917 |
| trucks | Greedy Best First (hFF) | 18 | 3 | 562.383 | 28.251 | 481.151 | 100 | 67.326 |
| zenotravel | Greedy Best First (hGoal) | 18 | 3 | 40207.101 | 2456.805 | 17879.29 | 100 | 1.69 |
| zenotravel | Greedy Best First (hFF) | 18 | 3 | 1679.104 | 120.336 | 1284.515 | 100 | 2.144 |


