using FormulaParserDemo.Api.DTOs;
using FormulaParserDemo.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FormulaParserDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormulaController : ControllerBase
{
    private readonly IFormulaService _formulaService;
    private readonly ILogger<FormulaController> _logger;

    public FormulaController(IFormulaService formulaService, ILogger<FormulaController> logger)
    {
        _formulaService = formulaService;
        _logger = logger;
    }

    /// <summary>
    /// Validate formula syntax and column references
    /// </summary>
    /// <param name="request">Formula validation request</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidateFormulaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ValidateFormulaResponse>> ValidateFormula([FromBody] ValidateFormulaRequest request)
    {
        _logger.LogInformation("Validating formula for table: {TableName}", request.TableName);

        var response = await _formulaService.ValidateFormulaAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Preview formula results on sample data
    /// </summary>
    /// <param name="request">Formula preview request</param>
    /// <returns>Preview data with calculated values</returns>
    [HttpPost("preview")]
    [ProducesResponseType(typeof(PreviewFormulaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PreviewFormulaResponse>> PreviewFormula([FromBody] PreviewFormulaRequest request)
    {
        _logger.LogInformation("Previewing formula for table: {TableName}, column: {ColumnName}",
            request.TableName, request.ColumnName);

        var response = await _formulaService.PreviewFormulaAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Convert formula to SQL without executing
    /// </summary>
    /// <param name="request">Formula conversion request</param>
    /// <returns>Generated SQL statement</returns>
    [HttpPost("convert")]
    [ProducesResponseType(typeof(ConvertFormulaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ConvertFormulaResponse>> ConvertFormula([FromBody] ConvertFormulaRequest request)
    {
        _logger.LogInformation("Converting formula for table: {TableName}, column: {ColumnName}",
            request.TableName, request.ColumnName);

        var response = await _formulaService.ConvertFormulaAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Apply formula to database (create computed column)
    /// </summary>
    /// <param name="request">Formula application request</param>
    /// <returns>Execution result</returns>
    [HttpPost("apply")]
    [ProducesResponseType(typeof(ApplyFormulaResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApplyFormulaResponse>> ApplyFormula([FromBody] ApplyFormulaRequest request)
    {
        _logger.LogInformation("Applying formula for table: {TableName}, column: {ColumnName}",
            request.TableName, request.ColumnName);

        var response = await _formulaService.ApplyFormulaAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Get list of available tables
    /// </summary>
    /// <returns>List of table names</returns>
    [HttpGet("tables")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<string>>> GetTables()
    {
        _logger.LogInformation("Fetching list of tables");

        var tables = await _formulaService.GetTablesAsync();
        return Ok(tables);
    }

    /// <summary>
    /// Get columns for a specific table
    /// </summary>
    /// <param name="tableName">Table name</param>
    /// <returns>List of column names</returns>
    [HttpGet("tables/{tableName}/columns")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<string>>> GetColumns(string tableName)
    {
        _logger.LogInformation("Fetching columns for table: {TableName}", tableName);

        var columns = await _formulaService.GetColumnsAsync(tableName);
        return Ok(columns);
    }

    /// <summary>
    /// Get formula examples for testing
    /// </summary>
    /// <returns>Dictionary of example formulas by type</returns>
    [HttpGet("examples")]
    [ProducesResponseType(typeof(Dictionary<string, List<string>>), StatusCodes.Status200OK)]
    public ActionResult<Dictionary<string, List<string>>> GetExamples()
    {
        var examples = new Dictionary<string, List<string>>
        {
            ["Type 2: Math Operations"] = new List<string>
            {
                "(product_sales + service_sales)",
                "(product_sales + service_sales) - operating_cost",
                "(product_sales + service_sales - operating_cost) / employee_count"
            },
            ["Type 3: Fixed Digit Operations"] = new List<string>
            {
                "product_sales * 1.15",
                "(product_sales + service_sales) / 12",
                "operating_cost * 0.85"
            },
            ["Type 4: IF Logic"] = new List<string>
            {
                "IF(employee_count > 50, \"Large\", \"Small\")",
                "IF((product_sales + service_sales) > 100000, \"High Revenue\", \"Standard\")",
                "IF(operating_cost > (product_sales + service_sales), \"Loss\", \"Profit\")"
            },
            ["Type 5: Aggregations"] = new List<string>
            {
                "(product_sales / SUM(product_sales)) * 100",
                "product_sales - AVG(product_sales)",
                "IF(product_sales > AVG(product_sales), \"Above Average\", \"Below Average\")"
            },
            ["Combined Examples"] = new List<string>
            {
                "IF((product_sales + service_sales) > AVG(product_sales + service_sales), \"High Performer\", \"Standard Performer\")",
                "((product_sales + service_sales - operating_cost) / SUM(product_sales + service_sales - operating_cost)) * 100"
            }
        };

        return Ok(examples);
    }
}
