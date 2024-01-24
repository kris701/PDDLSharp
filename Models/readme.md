PDDLSharp have a set of different node types that is used as intermediate formats for whatever you intend to use PDDLSharp for.

# PDDL
![Static Badge](https://img.shields.io/badge/Namespace-PDDLSharp.Models.PDDL-orange)

The PDDL model structure relies on 7 different interfaces, each with their own special thing about them.

![abc drawio](https://github.com/kris701/PDDLSharp/assets/22596587/3a017e30-bab3-49bb-9789-11faf8d2e8d5)

The nodes are:
* [INode](./PDDL/INode.cs)
   * This is the overall node interface. It contains the basic methods that the PDDL nodes have to implement. All the other nodes have this node as its base.
* [INamedNode](./PDDL/INamedNode.cs)
   * This is an extended [INode](./PDDL/INode.cs) interface, with the added requirement of it needing a name. An example could be that an [ActionDecl](./PDDL/Domain/ActionDecl.cs) is an [INamedNode](./PDDL/INamedNode.cs), since it has a name.
* [IDecl](./PDDL/IDecl.cs)
   * This is a structural version of [INode](./PDDL/INode.cs), basically its just all the nodes that cannot be used as expressions. An example could be you cannot make a [PredicatesDecl](./PDDL/Domain/PredicatesDecl.cs) inside the effects of an [ActionDecl](./PDDL/Domain/ActionDecl.cs)
* [IExp](./PDDL/IExp.cs)
   * This is then the opposite of the [IDecl](./PDDL/IDecl.cs)
* [IParametized](./PDDL/IParametized.cs)
   * This is nodes that have a [ParameterExp](./PDDL/Expressions/ParameterExp.cs) expression in them. An example could be a [ForAllExp](./PDDL/Expressions/ForAllExp.cs) expression.
* [IWalkable](./PDDL/IWalkable.cs)
   * This is nodes that can be "walked" through. This enables you to be able to walk into almost all structures in PDDL, say you wanted to find an action, you can simply find it in a [DomainDecl](./PDDL/Domain/DomainDecl.cs) by using a foreach loop.
* [IListable](./PDDL/IListable.cs)
   * This is an extended version of [IWalkable](./PDDL/IWalkable.cs) but with some added methods for accessing list items inside the node. An example is the [AndExp](./PDDL/Expressions/AndExp.cs) is an [IListable](./PDDL/IListable.cs), since it contains a list of [IExp](./PDDL/IExp.cs) inside of it.

These are the general node interfaces used by the PDDL side of the project. You can get more insight by looking at the different models in the source code.