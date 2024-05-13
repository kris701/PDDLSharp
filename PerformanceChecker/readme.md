# PDDLSharp Performance
Here are some general statistics about how good the PDDLSharp system performs.


These benchmarks are based on a single gripper domain
[BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) is used to generated the time results.
# Core Components
## PDDL
| Name | Time | Size | Throughput (MB/s) |
| - | - | - | - |
| DomainParsing | 81.3 μs | 932 B | 11.46 |
| ProblemParsing | 101.6 μs | 1401 B | 13.79 |
| Contextualization | 201.8 μs | 2333 B | 11.56 |
| Analyse | 466.2 μs | 2333 B | 5 |
| DomainCodeGeneration | 16.1 μs | 932 B | 57.89 |
| ProblemCodeGeneration | 21.7 μs | 1401 B | 64.56 |


## Fast Downward
| Name | Time | Size | Throughput (MB/s) |
| - | - | - | - |
| FDSASParsing | 701.8 μs | 11634 B | 16.58 |
| SASPlanParsing | 27.7 μs | 863 B | 31.16 |


