# PDDLSharp Performance
Here are some general statistics about how good the PDDLSharp system performs.


These benchmarks made on the domains from [Fast Downward](https://github.com/aibasel/downward-benchmarks/):
* gripper
* blocks
* depot
* logistics98
* miconic
* rovers
* trucks
* zenotravel

# PDDL
| Name | Iterations | Total Size (MB) | Total Time (s) | Throughput (MB/s) |
| - | - | - | - | - |
| PDDL Domain Parsing | 1 | 0.01 | 0.12 | 0.08 |
| PDDL Problem Parsing | 1 | 0.02 | 0.07 | 0.29 |
| PDDL Contextualization | 1 | 0.05 | 0.03 | 1.67 |
| PDDL Analysing | 1 | 0.05 | 0.28 | 0.18 |
| PDDL Domain Code Generation | 1 | 0.01 | 0.16 | 0.06 |
| PDDL Problem Code Generation | 1 | 0.02 | 0.15 | 0.13 |


