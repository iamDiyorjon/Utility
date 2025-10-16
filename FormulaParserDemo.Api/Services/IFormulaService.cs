using FormulaParserDemo.Api.DTOs;

namespace FormulaParserDemo.Api.Services;

public interface IFormulaService
{
    Task<ValidateFormulaResponse> ValidateFormulaAsync(ValidateFormulaRequest request);
    Task<PreviewFormulaResponse> PreviewFormulaAsync(PreviewFormulaRequest request);
    Task<ConvertFormulaResponse> ConvertFormulaAsync(ConvertFormulaRequest request);
    Task<ApplyFormulaResponse> ApplyFormulaAsync(ApplyFormulaRequest request);
    Task<List<string>> GetTablesAsync();
    Task<List<string>> GetColumnsAsync(string tableName);
}
