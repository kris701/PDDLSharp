AST Generators are intended to make a abstract syntax of a given file.
This is usually to make the parsing later a lot easier and clearer.

# PDDL AST Generators
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.ASTGenerators.PDDL-orange)

The AST generator for PDDL files generally makes [ASTNode](../Models/AST/ASTNode.cs)s from sets of "(" and ")".
Additionally it also saves the line context of where the nodes start at.

# FastDownward SAS AST Generators
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.ASTGenerators.FastDownward.SAS-orange)

This AST Generator generates [ASTNode](../Models/AST/ASTNode.cs)s from the node structure in the [Fast Downward Translator Format](https://www.fast-downward.org/TranslatorOutputFormat).
Basically it matches nodes between "?" marks.
