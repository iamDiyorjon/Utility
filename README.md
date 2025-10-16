# Formula Parser Demo

A comprehensive .NET 8 Web API application that parses user-defined formulas and converts them to PostgreSQL expressions using ANTLR4. This project demonstrates how to implement the **Utility Functions** feature from the Data Cleansing and Enrichment Tool specification.

## 🎯 Features

### Supported Formula Types

1. **Column Creation** - Create empty columns for future data entry
2. **Inter-Column Math Operations** - `(price * quantity) + tax`
3. **Fixed Digit Operations** - `salary * 1.15`
4. **IF Logic** - `IF(age >= 65, "Senior", "Adult")`
5. **Aggregations** - `(column_a / SUM(column_a)) * 100`

### Key Capabilities

✅ **Formula Validation** - Syntax checking and column reference validation
✅ **Preview Mode** - See calculated results before committing
✅ **Auto Data Type Detection** - Automatically detect NUMERIC, TEXT, BOOLEAN
✅ **Generated Computed Columns** - PostgreSQL `GENERATED ALWAYS AS ... STORED`
✅ **Formula Reusability** - Use previously created computed columns in new formulas
✅ **Web UI** - Simple HTML/JS interface for testing
✅ **Swagger Documentation** - Complete API documentation

---

## 🏗️ Architecture

```
┌─────────────────┐
│   Web UI        │  HTML/CSS/JavaScript
│  (wwwroot/)     │
└────────┬────────┘
         │
         v
┌─────────────────┐
│  API Controller │  FormulaController.cs
└────────┬────────┘
         │
         v
┌─────────────────┐
│ Formula Service │  FormulaService.cs
│  - Validation   │  - Schema checking
│  - Preview      │  - SQL generation
│  - Conversion   │  - Execution
└────────┬────────┘
         │
         v
┌─────────────────┐
│  ANTLR Parser   │  Formula.g4
│  & Visitor      │  FormulaToSqlVisitor.cs
└────────┬────────┘
         │
         v
┌─────────────────┐
│   PostgreSQL    │  sales_data table
└─────────────────┘
```

---

## 📋 Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PostgreSQL 12+** - [Download](https://www.postgresql.org/download/)
- **IDE** (optional) - Visual Studio 2022, VS Code, or Rider

---

## 🚀 Quick Start

### 1. Clone or Navigate to the Project

```bash
cd D:\MyRepositories\Utility
```

### 2. Set Up PostgreSQL Database

**Option A: Using psql command line**

```bash
# Create database
psql -U postgres -c "CREATE DATABASE formulademo;"

# Run setup script
psql -U postgres -d formulademo -f Database/setup.sql
```

**Option B: Using pgAdmin**

1. Open pgAdmin
2. Create new database named `formulademo`
3. Open Query Tool
4. Load and execute `Database/setup.sql`

### 3. Configure Connection String

Edit `appsettings.json` if your PostgreSQL credentials differ:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=formulademo;Username=postgres;Password=YOUR_PASSWORD"
  }
}
```

### 4. Build and Run

```bash
cd FormulaParserDemo.Api
dotnet build
dotnet run
```

The application will start on `https://localhost:5001` (or check console output).

### 5. Access the Application

- **Web UI**: `https://localhost:5001/index.html`
- **Swagger API Docs**: `https://localhost:5001` (root serves Swagger)

---

## 📖 Usage Guide

### Using the Web UI

#### Step 1: Select Table
1. Choose `sales_data` from the dropdown
2. Click "Load Columns" to see available columns

#### Step 2: Enter Formula
1. **Column Name**: Enter the name for your new computed column (e.g., `total_revenue`)
2. **Formula**: Enter your formula (e.g., `(product_sales + service_sales)`)
3. **Data Type**: Leave as "Auto-detect" or choose manually
4. Click "Validate" to check syntax

#### Step 3: Preview Results
1. Click "Generate Preview"
2. Review the calculated values in the preview table
3. Check the generated SQL expression

#### Step 4: Confirm & Apply
1. Click "✓ Confirm & Apply"
2. The computed column will be created in the database
3. Reload the page to create another formula

---

## 📝 Formula Examples

### Type 2: Inter-Column Math Operations

```sql
-- Basic addition
(product_sales + service_sales)

-- Profit calculation
(product_sales + service_sales) - operating_cost

-- Per-employee revenue
(product_sales + service_sales) / employee_count
```

### Type 3: Fixed Digit Operations

```sql
-- 15% salary increase
product_sales * 1.15

-- Monthly average
(product_sales + service_sales) / 12

-- 15% discount
operating_cost * 0.85
```

### Type 4: IF Logic

```sql
-- Company size categorization
IF(employee_count > 50, "Large", "Small")

-- Revenue classification
IF((product_sales + service_sales) > 100000, "High Revenue", "Standard")

-- Profitability check
IF(operating_cost > (product_sales + service_sales), "Loss", "Profit")
```

### Type 5: Aggregations

```sql
-- Percentage of total sales
(product_sales / SUM(product_sales)) * 100

-- Deviation from average
product_sales - AVG(product_sales)

-- Performance vs average
IF(product_sales > AVG(product_sales), "Above Average", "Below Average")
```

### Combined Examples

```sql
-- Complex performance metric
IF(
  (product_sales + service_sales) > AVG(product_sales + service_sales),
  "High Performer",
  "Standard Performer"
)

-- Contribution percentage
((product_sales + service_sales - operating_cost) /
  SUM(product_sales + service_sales - operating_cost)) * 100
```

---

## 🔌 API Endpoints

### `POST /api/formula/validate`
Validate formula syntax and column references.

**Request:**
```json
{
  "tableName": "sales_data",
  "formula": "(product_sales + service_sales)"
}
```

**Response:**
```json
{
  "isValid": true,
  "errors": [],
  "warnings": [],
  "referencedColumns": ["product_sales", "service_sales"],
  "detectedDataType": "NUMERIC"
}
```

### `POST /api/formula/preview`
Preview calculated results on sample data.

**Request:**
```json
{
  "tableName": "sales_data",
  "columnName": "total_revenue",
  "formula": "(product_sales + service_sales)",
  "previewRows": 5
}
```

**Response:**
```json
{
  "success": true,
  "previewData": [...],
  "generatedSql": "((product_sales + service_sales))",
  "detectedDataType": "NUMERIC",
  "totalRowsAffected": 20,
  "successfulCalculations": 5
}
```

### `POST /api/formula/convert`
Convert formula to SQL without executing.

**Request:**
```json
{
  "tableName": "sales_data",
  "columnName": "total_revenue",
  "formula": "(product_sales + service_sales)"
}
```

**Response:**
```json
{
  "success": true,
  "generatedSql": "ALTER TABLE sales_data\nADD COLUMN total_revenue NUMERIC\nGENERATED ALWAYS AS ((product_sales + service_sales)) STORED;",
  "detectedDataType": "NUMERIC"
}
```

### `POST /api/formula/apply`
Create the computed column in the database.

**Request:**
```json
{
  "tableName": "sales_data",
  "columnName": "total_revenue",
  "formula": "(product_sales + service_sales)"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Column 'total_revenue' created successfully",
  "executedSql": "ALTER TABLE sales_data...",
  "rowsAffected": 20
}
```

---

## 🧪 Testing the Application

### 1. Test Formula Validation

```bash
curl -X POST "https://localhost:5001/api/formula/validate" \
  -H "Content-Type: application/json" \
  -d '{
    "tableName": "sales_data",
    "formula": "(product_sales + service_sales)"
  }'
```

### 2. Test Preview

```bash
curl -X POST "https://localhost:5001/api/formula/preview" \
  -H "Content-Type: application/json" \
  -d '{
    "tableName": "sales_data",
    "columnName": "total_revenue",
    "formula": "(product_sales + service_sales)",
    "previewRows": 5
  }'
```

### 3. Test Apply

```bash
curl -X POST "https://localhost:5001/api/formula/apply" \
  -H "Content-Type: application/json" \
  -d '{
    "tableName": "sales_data",
    "columnName": "total_revenue",
    "formula": "(product_sales + service_sales)"
  }'
```

### 4. Verify in PostgreSQL

```sql
-- Check if column was created
SELECT column_name, data_type, is_generated
FROM information_schema.columns
WHERE table_name = 'sales_data'
AND column_name = 'total_revenue';

-- View data with new column
SELECT company_name, product_sales, service_sales, total_revenue
FROM sales_data
LIMIT 5;
```

---

## 📚 Project Structure

```
FormulaParserDemo/
├── FormulaParserDemo.Api/
│   ├── ANTLR/
│   │   └── Formula.g4                  # ANTLR grammar definition
│   ├── Controllers/
│   │   └── FormulaController.cs        # API endpoints
│   ├── Services/
│   │   ├── IFormulaService.cs          # Service interface
│   │   └── FormulaService.cs           # Business logic
│   ├── Visitors/
│   │   └── FormulaToSqlVisitor.cs      # AST → SQL converter
│   ├── Models/
│   │   └── FormulaMetadata.cs          # Domain models
│   ├── DTOs/
│   │   ├── FormulaRequest.cs           # Request DTOs
│   │   └── FormulaResponse.cs          # Response DTOs
│   ├── wwwroot/
│   │   ├── index.html                  # Web UI
│   │   ├── css/style.css               # Styles
│   │   └── js/app.js                   # Frontend logic
│   ├── Program.cs                      # Application entry point
│   └── appsettings.json                # Configuration
├── Database/
│   └── setup.sql                       # Database schema & data
└── README.md                           # This file
```

---

## 🔧 How It Works

### 1. Formula Parsing with ANTLR

The `Formula.g4` grammar defines the syntax:

```antlr
expr
    : ifExpr                                    # IF expression
    | aggregateFunction '(' expr ')'            # Aggregation
    | expr op=('*' | '/' | '%') expr            # Multiplication
    | expr op=('+' | '-') expr                  # Addition
    | expr comparisonOp expr                    # Comparison
    | '(' expr ')'                              # Parentheses
    | NUMBER                                    # Number literal
    | STRING                                    # String literal
    | IDENTIFIER                                # Column reference
    ;
```

### 2. AST to SQL Conversion

The `FormulaToSqlVisitor` walks the parse tree and generates PostgreSQL SQL:

```csharp
public override string VisitAdditiveExpression(context)
{
    var left = Visit(context.expr(0));
    var right = Visit(context.expr(1));
    var op = context.op.Text;
    return $"({left} {op} {right})";
}
```

### 3. PostgreSQL Generated Columns

The system creates computed columns using:

```sql
ALTER TABLE sales_data
ADD COLUMN total_revenue NUMERIC
GENERATED ALWAYS AS ((product_sales + service_sales)) STORED;
```

This automatically computes values for all existing and future rows!

---

## 🐛 Troubleshooting

### Build Errors

**Error**: `Formula.g4` file not found
**Solution**: Ensure the ANTLR grammar file exists in `ANTLR/Formula.g4`

**Error**: ANTLR parser not generated
**Solution**: Clean and rebuild:
```bash
dotnet clean
dotnet build
```

### Database Connection Errors

**Error**: `Connection refused`
**Solution**:
1. Check PostgreSQL is running: `pg_isready`
2. Verify connection string in `appsettings.json`
3. Test connection: `psql -U postgres -d formulademo`

**Error**: `Database "formulademo" does not exist`
**Solution**: Create database:
```bash
psql -U postgres -c "CREATE DATABASE formulademo;"
```

### Runtime Errors

**Error**: `Column already exists`
**Solution**: Drop the existing column:
```sql
ALTER TABLE sales_data DROP COLUMN IF EXISTS column_name;
```

**Error**: `Division by zero`
**Solution**: This is expected behavior. The system sets the result to NULL for affected rows and reports it in the preview.

---

## 🎓 Learning Resources

### ANTLR4
- [ANTLR4 Documentation](https://github.com/antlr/antlr4/blob/master/doc/index.md)
- [ANTLR4 C# Target](https://github.com/antlr/antlr4/blob/master/doc/csharp-target.md)

### PostgreSQL Generated Columns
- [PostgreSQL Generated Columns](https://www.postgresql.org/docs/current/ddl-generated-columns.html)

### .NET 8
- [ASP.NET Core Web API](https://learn.microsoft.com/en-us/aspnet/core/web-api/)
- [Dapper ORM](https://github.com/DapperLib/Dapper)

---

## 🚀 Future Enhancements

- [ ] **Nested IF Statements** - Support complex conditional logic
- [ ] **More Aggregate Functions** - Add statistical functions
- [ ] **Formula Dependencies** - Visual graph of formula relationships
- [ ] **Batch Operations** - Create multiple columns at once
- [ ] **Formula Templates** - Save and reuse common formulas
- [ ] **Undo Functionality** - Remove computed columns safely
- [ ] **Performance Optimization** - Query plan analysis
- [ ] **Unit Tests** - Comprehensive test coverage
- [ ] **Docker Support** - Containerized deployment

---

## 📄 License

This is a demo project for educational purposes.

---

## 👥 Contributing

This is a demonstration project. For questions or suggestions, please open an issue in the repository.

---

## 🙏 Acknowledgments

- **ANTLR4** by Terence Parr
- **PostgreSQL** community
- **.NET Foundation**

---

**Built with ❤️ using .NET 8, ANTLR4, and PostgreSQL**
