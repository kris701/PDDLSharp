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

# PDDL
| Name | Iterations | Total Size (MB) | Total Time (s) | Throughput (MB/s) |
| - | - | - | - | - |
| PDDL Domain Parsing | 1 | 0.01 | 0.09 | 0.11 |
| PDDL Problem Parsing | 1 | 0.01 | 0.17 | 0.06 |
| PDDL Contextualization | 1 | 0.04 | 4 | 0.01 |
| PDDL Analysing | 1 | 0.04 | 0.18 | 0.22 |
| PDDL Domain Code Generation | 1 | 0.01 | 0.07 | 0.15 |
| PDDL Problem Code Generation | 1 | 0.01 | 0.07 | 0.14 |


# Fast Downward SAS
| Name | Iterations | Total Size (MB) | Total Time (s) | Throughput (MB/s) |
| - | - | - | - | - |
| Fast Downward SAS Parsing | 1 | 0.18 | 3 | 0.06 |


# Fast Downward Plans
| Name | Iterations | Total Size (MB) | Total Time (s) | Throughput (MB/s) |
| - | - | - | - | - |
| Fast Downward Plan Parsing | 1 | 0.01 | 0 | 2.78 |


# Translation
| Domain | Problems | Iterations | Total Operators | Operators / second | Total Time (s) |
| - | - | - | - | - | - |
| blocks | 2 | 1 | 440 | 1571.43 | 0.28 |
| depot | 2 | 1 | 936 | 1613.79 | 0.58 |
| gripper | 2 | 1 | 88 | 733.33 | 0.12 |
| miconic | 2 | 1 | 8 | 114.29 | 0.07 |
| rovers | 2 | 1 | 154 | 616 | 0.25 |
| trucks | 2 | 1 | 849 | 1601.89 | 0.53 |
| zenotravel | 2 | 1 | 264 | 733.33 | 0.36 |


