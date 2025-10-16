namespace FormulaParserDemo.Api.DTOs;

/// <summary>
/// Response model for formula validation
/// </summary>
public class ValidateFormulaResponse
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> ReferencedColumns { get; set; } = new();
    public string? DetectedDataType { get; set; }
}

/// <summary>
/// Response model for formula preview
/// </summary>
public class PreviewFormulaResponse
{
    public bool Success { get; set; }
    public List<Dictionary<string, object?>> PreviewData { get; set; } = new();
    public string GeneratedSql { get; set; } = string.Empty;
    public string DetectedDataType { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public int TotalRowsAffected { get; set; }
    public int SuccessfulCalculations { get; set; }
    public int FailedCalculations { get; set; }
}

/// <summary>
/// Response model for SQL conversion
/// </summary>
public class ConvertFormulaResponse
{
    public bool Success { get; set; }
    public string GeneratedSql { get; set; } = string.Empty;
    public string DetectedDataType { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Response model for applying formula
/// </summary>
public class ApplyFormulaResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ExecutedSql { get; set; } = string.Empty;
    public int RowsAffected { get; set; }
    public List<string> Errors { get; set; } = new();
}
