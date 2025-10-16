using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using FormulaParserDemo.Api.ANTLR;
using FormulaParserDemo.Api.Models;

namespace FormulaParserDemo.Api.Visitors;

/// <summary>
/// Converts parsed formula AST to PostgreSQL SQL expression
/// </summary>
public class FormulaToSqlVisitor : FormulaBaseVisitor<string>
{
    private readonly FormulaMetadata _metadata = new();

    public FormulaMetadata GetMetadata() => _metadata;

    public override string VisitFormula([NotNull] FormulaParser.FormulaContext context)
    {
        return Visit(context.expr());
    }

    // Type 2 & 3: Multiplicative operations (*, /, %)
    public override string VisitMultiplicativeExpression([NotNull] FormulaParser.MultiplicativeExpressionContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        var op = context.op.Text;

        return $"({left} {op} {right})";
    }

    // Type 2 & 3: Additive operations (+, -)
    public override string VisitAdditiveExpression([NotNull] FormulaParser.AdditiveExpressionContext context)
    {
        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        var op = context.op.Text;

        return $"({left} {op} {right})";
    }

    // Type 4: Comparison operations (>, <, >=, <=, =, !=)
    public override string VisitComparisonExpression([NotNull] FormulaParser.ComparisonExpressionContext context)
    {
        _metadata.HasComparison = true;

        var left = Visit(context.expr(0));
        var right = Visit(context.expr(1));
        var op = context.comparisonOp().GetText();

        // Convert = to SQL equality operator
        if (op == "=")
        {
            op = "=";
        }
        // Convert != or <> to SQL not equal
        else if (op == "!=" || op == "<>")
        {
            op = "<>";
        }

        return $"({left} {op} {right})";
    }

    // Parenthesized expressions
    public override string VisitParenthesizedExpression([NotNull] FormulaParser.ParenthesizedExpressionContext context)
    {
        return Visit(context.expr());
    }

    // Type 3: Number literals
    public override string VisitNumberLiteral([NotNull] FormulaParser.NumberLiteralContext context)
    {
        return context.GetText();
    }

    // Type 4: String literals
    public override string VisitStringLiteral([NotNull] FormulaParser.StringLiteralContext context)
    {
        // Remove surrounding quotes and escape for PostgreSQL
        var text = context.GetText();
        var value = text.Substring(1, text.Length - 2); // Remove quotes
        value = value.Replace("'", "''"); // Escape single quotes

        // Update detected data type to TEXT if we have string literals
        if (_metadata.DetectedDataType == "NUMERIC")
        {
            _metadata.DetectedDataType = "TEXT";
        }

        return $"'{value}'";
    }

    // Type 2: Column references
    public override string VisitColumnReference([NotNull] FormulaParser.ColumnReferenceContext context)
    {
        var columnName = context.GetText();

        // Track referenced columns
        if (!_metadata.ReferencedColumns.Contains(columnName))
        {
            _metadata.ReferencedColumns.Add(columnName);
        }

        return columnName;
    }

    // Type 4: IF expressions
    public override string VisitIfExpression([NotNull] FormulaParser.IfExpressionContext context)
    {
        _metadata.HasIfCondition = true;
        return Visit(context.ifExpr());
    }

    public override string VisitIfExpr([NotNull] FormulaParser.IfExprContext context)
    {
        var condition = Visit(context.expr(0));
        var trueValue = Visit(context.expr(1));
        var falseValue = Visit(context.expr(2));

        // PostgreSQL CASE expression
        return $"CASE WHEN {condition} THEN {trueValue} ELSE {falseValue} END";
    }

    // Type 5: Aggregate functions
    public override string VisitAggregateExpression([NotNull] FormulaParser.AggregateExpressionContext context)
    {
        _metadata.HasAggregation = true;

        var funcName = context.aggregateFunction().GetText().ToUpper();
        var expr = Visit(context.expr());

        // Track aggregate functions used
        if (!_metadata.AggregatedFunctions.Contains(funcName))
        {
            _metadata.AggregatedFunctions.Add(funcName);
        }

        // Map aggregate function names to PostgreSQL equivalents
        var sqlFunc = funcName switch
        {
            "STDDEV" => "STDDEV_POP",
            "VARIANCE" => "VAR_POP",
            "MEDIAN" => "PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY " + expr + ")",
            "MODE" => "MODE() WITHIN GROUP (ORDER BY " + expr + ")",
            _ => funcName
        };

        // Special handling for MEDIAN and MODE (already includes expression)
        if (funcName == "MEDIAN" || funcName == "MODE")
        {
            return sqlFunc;
        }

        return $"{sqlFunc}({expr})";
    }

    /// <summary>
    /// Detect the result data type based on the formula
    /// </summary>
    public string DetectDataType()
    {
        // If we have string literals or comparisons, it might return TEXT or BOOLEAN
        if (_metadata.HasIfCondition)
        {
            // IF can return various types, default to TEXT for flexibility
            return "TEXT";
        }

        if (_metadata.HasComparison && !_metadata.HasIfCondition)
        {
            // Pure comparison returns BOOLEAN
            return "BOOLEAN";
        }

        // Check if we have any string literals
        if (_metadata.DetectedDataType == "TEXT")
        {
            return "TEXT";
        }

        // Default to NUMERIC for mathematical operations
        return "NUMERIC";
    }
}
