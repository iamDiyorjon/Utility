grammar Formula;

/*
 * Parser Rules
 * Supports all 5 formula types:
 * 1. Column Creation (handled at application level)
 * 2. Inter-Column Math: (price * quantity) + tax
 * 3. Fixed Digit Operations: salary * 1.15
 * 4. IF Logic: IF(age >= 65, "Senior", "Adult")
 * 5. Aggregations: (column_a / SUM(column_a)) * 100
 */

// Main entry point
formula
    : expr EOF
    ;

// Expression with operator precedence
expr
    : ifExpr                                                    # IfExpression
    | aggregateFunction '(' expr ')'                           # AggregateExpression
    | expr op=('*' | '/' | '%') expr                           # MultiplicativeExpression
    | expr op=('+' | '-') expr                                 # AdditiveExpression
    | expr op='||' expr                                        # ConcatenationExpression
    | expr comparisonOp expr                                    # ComparisonExpression
    | '(' expr ')'                                             # ParenthesizedExpression
    | NUMBER                                                    # NumberLiteral
    | STRING                                                    # StringLiteral
    | IDENTIFIER                                               # ColumnReference
    ;

// IF condition: IF(condition, true_value, false_value)
ifExpr
    : 'IF' '(' expr ',' expr ',' expr ')'
    ;

// Comparison operators
comparisonOp
    : '>' | '<' | '>=' | '<=' | '=' | '!=' | '<>'
    ;

// Aggregate functions
aggregateFunction
    : 'SUM' | 'AVG' | 'MIN' | 'MAX' | 'COUNT'
    | 'MEDIAN' | 'MODE' | 'STDDEV' | 'VARIANCE'
    ;

/*
 * Lexer Rules
 */

// Numbers: integers and decimals
NUMBER
    : [0-9]+ ('.' [0-9]+)?
    ;

// String literals: "text" or 'text'
STRING
    : '"' (~["\r\n])* '"'
    | '\'' (~['\r\n])* '\''
    ;

// Column identifiers: column_name, Column123, _column
IDENTIFIER
    : [a-zA-Z_][a-zA-Z0-9_]*
    ;

// Whitespace
WS
    : [ \t\r\n]+ -> skip
    ;
