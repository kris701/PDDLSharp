# PDDLSharp Performance
Here are some general statistics about how good the PDDLSharp system performs.


These benchmarks are based on a single gripper domain
[BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) is used to generated the time results.
# Core Components
## PDDL
| Name | Time | Size | Throughput (MB/s) |
| - | - | - | - |
| Domain Parsing | 118.4 μs | 932 B | 7.87 |
| Problem Parsing | 146.9 μs | 1401 B | 9.54 |
| Contextualization | 205.3 μs | 2333 B | 11.36 |
| Analyzation | 527.3 μs | 2333 B | 4.42 |
| Domain Code Generation | 16.3 μs | 932 B | 57.18 |
| Problem Code Generation | 24 μs | 1401 B | 58.37 |


## Fast Downward
| Name | Time | Size | Throughput (MB/s) |
| - | - | - | - |
| SAS Parsing | 682.6 μs | 11634 B | 17.04 |
| Plan Parsing | 27.8 μs | 863 B | 31.04 |


## Translation and Planners
| Name | Time | Size | Throughput (MB/s) |
| - | - | - | - |
| Translation | 5542.6 μs | 2333 B | 0.42 |
| Solve hGoal | 983.9 μs | 2333 B | 2.37 |
| Solve hFF | 10237.6 μs | 2333 B | 0.23 |


