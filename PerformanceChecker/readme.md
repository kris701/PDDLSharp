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
| Name | Time | Size | Throughput (MB/s) |
| - | - | - | - |
| DomainParsing | 33.2 μs | 448 B | 13.493975903614457 |
| ProblemParsing | 39.9 μs | 448 B | 11.228070175438596 |
| Contextualization | 63.7 μs | 448 B | 7.032967032967033 |
| Analyse | 142.8 μs | 448 B | 3.1372549019607843 |
| DomainCodeGeneration | 3.6 μs | 448 B | 124.44444444444444 |
| ProblemCodeGeneration | 6 μs | 448 B | 74.66666666666666 |

