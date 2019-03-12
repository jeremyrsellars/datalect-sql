# SQL Datalect

A data-oriented SQL dialect (possibly) useful for data manipulation of queries.  You know, like a DOM.

Because a good idea is a often good idea in many languages, this may become a polyglot library with packages to support different platforms using the Datalect idioms.

### Goals

* Composable queries as data structures manipulatable in your programming language
	* Functions combine arbitrary numbers of expressions using prefix notation to apply a pattern to several expressions at once.
* Provide a data-oriented DOM for platform-specific SQL queries.
	* In my experience, it is rare to target several RDBMS engines with the same query, since each RDBMS (or platform library) has its own variants of things like string concatenation, pattern matching, types, bind variable naming convention, etc.  Should you need cross-platfom query generators, that should still be made easier through platform-specific builders (Feel free to abstract over this library!)
* Natural, safe bind variable/query parameter support via BoundValue.
	* Provide functions to create arbitrary SQL with arbitrary bind variables.
	* This library seeks to prevent SQL injection by providing first-class support for bind variables.
