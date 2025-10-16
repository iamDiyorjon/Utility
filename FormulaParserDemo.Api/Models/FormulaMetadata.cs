namespace FormulaParserDemo.Api.Models;

/// <summary>
/// Metadata about a parsed formula
/// </summary>
public class FormulaMetadata
{
    public List<string> ReferencedColumns { get; set; } = new();
    public List<string> AggregatedFunctions { get; set; } = new();
    public bool HasIfCondition { get; set; }
    public bool HasAggregation { get; set; }
    public bool HasComparison { get; set; }
    public string DetectedDataType { get; set; } = "NUMERIC";
}

/// <summary>
/// Represents a column definition
/// </summary>
public class ColumnDefinition
{
    public string ColumnName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsComputed { get; set; } // Track if it's a computed column for reusability
}

/// <summary>
/// Represents a table schema
/// </summary>
public class TableSchema
{
    public string TableName { get; set; } = string.Empty;
    public List<ColumnDefinition> Columns { get; set; } = new();
}
