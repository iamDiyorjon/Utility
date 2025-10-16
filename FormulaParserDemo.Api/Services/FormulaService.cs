using Antlr4.Runtime;
using Dapper;
using FormulaParserDemo.Api.ANTLR;
using FormulaParserDemo.Api.DTOs;
using FormulaParserDemo.Api.Models;
using FormulaParserDemo.Api.Visitors;
using Npgsql;
using System.Data;

namespace FormulaParserDemo.Api.Services;

public class FormulaService : IFormulaService
{
    private readonly string _connectionString;
    private readonly ILogger<FormulaService> _logger;

    public FormulaService(IConfiguration configuration, ILogger<FormulaService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentException("Connection string 'DefaultConnection' not found");
        _logger = logger;
    }

    private IDbConnection CreateConnection() => new NpgsqlConnection(_connectionString);

    /// <summary>
    /// Validate formula syntax and column references
    /// </summary>
    public async Task<ValidateFormulaResponse> ValidateFormulaAsync(ValidateFormulaRequest request)
    {
        var response = new ValidateFormulaResponse();

        try
        {
            // Step 1: Parse the formula
            var (sqlExpression, metadata, parseErrors) = ParseFormula(request.Formula);

            if (parseErrors.Any())
            {
                response.IsValid = false;
                response.Errors.AddRange(parseErrors);
                return response;
            }

            // Step 2: Get table schema and validate column references
            var schema = await GetTableSchemaAsync(request.TableName);

            foreach (var columnName in metadata.ReferencedColumns)
            {
                var column = schema.Columns.FirstOrDefault(c =>
                    c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));

                if (column == null)
                {
                    response.Errors.Add($"Column '{columnName}' does not exist in table '{request.TableName}'");
                }
            }

            // Step 3: Warnings for aggregations
            if (metadata.HasAggregation)
            {
                response.Warnings.Add("Formula contains aggregate functions. This will compute across all rows in the slice.");
            }

            // Step 4: Detect data type
            var visitor = new FormulaToSqlVisitor();
            response.DetectedDataType = visitor.DetectDataType();

            response.IsValid = !response.Errors.Any();
            response.ReferencedColumns = metadata.ReferencedColumns;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating formula");
            response.IsValid = false;
            response.Errors.Add($"Validation error: {ex.Message}");
            return response;
        }
    }

    /// <summary>
    /// Preview formula results on sample data
    /// </summary>
    public async Task<PreviewFormulaResponse> PreviewFormulaAsync(PreviewFormulaRequest request)
    {
        var response = new PreviewFormulaResponse();

        try
        {
            // Step 1: Validate first
            var validation = await ValidateFormulaAsync(new ValidateFormulaRequest
            {
                TableName = request.TableName,
                Formula = request.Formula
            });

            if (!validation.IsValid)
            {
                response.Success = false;
                response.Errors.AddRange(validation.Errors);
                return response;
            }

            // Step 2: Parse formula to SQL
            var (sqlExpression, metadata, parseErrors) = ParseFormula(request.Formula);
            response.GeneratedSql = sqlExpression;
            response.DetectedDataType = new FormulaToSqlVisitor().DetectDataType();

            // Step 3: Build preview query
            string previewQuery;

            if (metadata.HasAggregation)
            {
                // For aggregations, we need a different query structure
                previewQuery = $@"
                    SELECT
                        *,
                        {sqlExpression} as {request.ColumnName}
                    FROM {request.TableName}
                    LIMIT {request.PreviewRows}";
            }
            else
            {
                // For row-by-row calculations
                previewQuery = $@"
                    SELECT
                        *,
                        {sqlExpression} as {request.ColumnName}
                    FROM {request.TableName}
                    LIMIT {request.PreviewRows}";
            }

            // Step 4: Execute preview query
            using var connection = CreateConnection();
            var results = await connection.QueryAsync(previewQuery);

            // Convert to dictionary list
            foreach (var row in results)
            {
                var dict = new Dictionary<string, object?>();
                var rowDict = (IDictionary<string, object?>)row;

                foreach (var kvp in rowDict)
                {
                    dict[kvp.Key] = kvp.Value;
                }

                response.PreviewData.Add(dict);
            }

            // Step 5: Calculate statistics
            response.SuccessfulCalculations = response.PreviewData.Count;
            response.TotalRowsAffected = await connection.ExecuteScalarAsync<int>(
                $"SELECT COUNT(*) FROM {request.TableName}");

            response.Success = true;
            response.Warnings.AddRange(validation.Warnings);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error previewing formula");
            response.Success = false;
            response.Errors.Add($"Preview error: {ex.Message}");
            return response;
        }
    }

    /// <summary>
    /// Convert formula to SQL without executing
    /// </summary>
    public async Task<ConvertFormulaResponse> ConvertFormulaAsync(ConvertFormulaRequest request)
    {
        var response = new ConvertFormulaResponse();

        try
        {
            // Step 1: Validate
            var validation = await ValidateFormulaAsync(new ValidateFormulaRequest
            {
                TableName = request.TableName,
                Formula = request.Formula
            });

            if (!validation.IsValid)
            {
                response.Success = false;
                response.Errors.AddRange(validation.Errors);
                return response;
            }

            // Step 2: Parse formula
            var (sqlExpression, metadata, parseErrors) = ParseFormula(request.Formula);

            if (parseErrors.Any())
            {
                response.Success = false;
                response.Errors.AddRange(parseErrors);
                return response;
            }

            // Step 3: Determine data type
            var dataType = request.DataType ?? validation.DetectedDataType ?? "NUMERIC";
            response.DetectedDataType = dataType;

            // Step 4: Generate ALTER TABLE SQL
            response.GeneratedSql = $@"ALTER TABLE {request.TableName}
ADD COLUMN {request.ColumnName} {dataType}
GENERATED ALWAYS AS ({sqlExpression}) STORED;";

            response.Success = true;
            response.Warnings.AddRange(validation.Warnings);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting formula");
            response.Success = false;
            response.Errors.Add($"Conversion error: {ex.Message}");
            return response;
        }
    }

    /// <summary>
    /// Apply formula to database (create computed column)
    /// </summary>
    public async Task<ApplyFormulaResponse> ApplyFormulaAsync(ApplyFormulaRequest request)
    {
        var response = new ApplyFormulaResponse();

        try
        {
            // Step 1: Convert formula to SQL
            var conversion = await ConvertFormulaAsync(new ConvertFormulaRequest
            {
                TableName = request.TableName,
                ColumnName = request.ColumnName,
                Formula = request.Formula,
                DataType = request.DataType
            });

            if (!conversion.Success)
            {
                response.Success = false;
                response.Errors.AddRange(conversion.Errors);
                return response;
            }

            // Step 2: Check if column already exists
            using var connection = CreateConnection();
            var columnExists = await connection.ExecuteScalarAsync<bool>($@"
                SELECT EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_name = '{request.TableName}'
                    AND column_name = '{request.ColumnName}'
                )");

            if (columnExists)
            {
                response.Success = false;
                response.Errors.Add($"Column '{request.ColumnName}' already exists in table '{request.TableName}'");
                return response;
            }

            // Step 3: Execute ALTER TABLE
            await connection.ExecuteAsync(conversion.GeneratedSql);

            // Step 4: Get row count
            response.RowsAffected = await connection.ExecuteScalarAsync<int>(
                $"SELECT COUNT(*) FROM {request.TableName}");

            response.Success = true;
            response.Message = $"Column '{request.ColumnName}' created successfully";
            response.ExecutedSql = conversion.GeneratedSql;

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying formula");
            response.Success = false;
            response.Errors.Add($"Application error: {ex.Message}");
            return response;
        }
    }

    /// <summary>
    /// Get list of tables in the database
    /// </summary>
    public async Task<List<string>> GetTablesAsync()
    {
        using var connection = CreateConnection();
        var tables = await connection.QueryAsync<string>(@"
            SELECT table_name
            FROM information_schema.tables
            WHERE table_schema = 'public'
            AND table_type = 'BASE TABLE'
            ORDER BY table_name");

        return tables.ToList();
    }

    /// <summary>
    /// Get columns for a specific table
    /// </summary>
    public async Task<List<string>> GetColumnsAsync(string tableName)
    {
        using var connection = CreateConnection();
        var columns = await connection.QueryAsync<string>($@"
            SELECT column_name
            FROM information_schema.columns
            WHERE table_name = '{tableName}'
            ORDER BY ordinal_position");

        return columns.ToList();
    }

    #region Private Helper Methods

    /// <summary>
    /// Parse formula using ANTLR and convert to SQL
    /// </summary>
    private (string SqlExpression, FormulaMetadata Metadata, List<string> Errors) ParseFormula(string formula)
    {
        var errors = new List<string>();

        try
        {
            // Create ANTLR input stream
            var inputStream = new AntlrInputStream(formula);
            var lexer = new FormulaLexer(inputStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new FormulaParser(tokenStream);

            // Custom error listener
            var errorListener = new FormulaErrorListener();
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);

            // Parse the formula
            var tree = parser.formula();

            // Check for syntax errors
            if (errorListener.Errors.Any())
            {
                return (string.Empty, new FormulaMetadata(), errorListener.Errors);
            }

            // Visit the parse tree and generate SQL
            var visitor = new FormulaToSqlVisitor();
            var sqlExpression = visitor.Visit(tree);
            var metadata = visitor.GetMetadata();

            return (sqlExpression, metadata, errors);
        }
        catch (Exception ex)
        {
            errors.Add($"Parse error: {ex.Message}");
            return (string.Empty, new FormulaMetadata(), errors);
        }
    }

    /// <summary>
    /// Get table schema including column definitions
    /// </summary>
    private async Task<TableSchema> GetTableSchemaAsync(string tableName)
    {
        using var connection = CreateConnection();

        var columns = await connection.QueryAsync<ColumnDefinition>($@"
            SELECT
                column_name as ColumnName,
                data_type as DataType,
                is_nullable = 'YES' as IsNullable,
                is_generated = 'ALWAYS' as IsComputed
            FROM information_schema.columns
            WHERE table_name = '{tableName}'
            ORDER BY ordinal_position");

        return new TableSchema
        {
            TableName = tableName,
            Columns = columns.ToList()
        };
    }

    #endregion
}

/// <summary>
/// Custom error listener for ANTLR parsing
/// </summary>
public class FormulaErrorListener : BaseErrorListener
{
    public List<string> Errors { get; } = new();

    public override void SyntaxError(
        TextWriter output,
        IRecognizer recognizer,
        IToken offendingSymbol,
        int line,
        int charPositionInLine,
        string msg,
        RecognitionException e)
    {
        Errors.Add($"Syntax error at line {line}:{charPositionInLine} - {msg}");
    }
}
