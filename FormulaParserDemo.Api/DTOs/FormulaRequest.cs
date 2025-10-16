namespace FormulaParserDemo.Api.DTOs;

/// <summary>
/// Request model for formula validation
/// </summary>
public class ValidateFormulaRequest
{
    public string TableName { get; set; } = string.Empty;
    public string Formula { get; set; } = string.Empty;
}

/// <summary>
/// Request model for formula preview
/// </summary>
public class PreviewFormulaRequest
{
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string Formula { get; set; } = string.Empty;
    public int PreviewRows { get; set; } = 5; // Number of rows to preview
}

/// <summary>
/// Request model for SQL conversion
/// </summary>
public class ConvertFormulaRequest
{
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string Formula { get; set; } = string.Empty;
    public string? DataType { get; set; } // Optional: DECIMAL, INTEGER, VARCHAR, BOOLEAN (auto-detect if null)
}

/// <summary>
/// Request model for applying formula to database
/// </summary>
public class ApplyFormulaRequest
{
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string Formula { get; set; } = string.Empty;
    public string? DataType { get; set; } // Optional: auto-detect if null
    public bool UseStoredGenerated { get; set; } = true; // Use GENERATED ALWAYS AS ... STORED
}
