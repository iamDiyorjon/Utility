# System Architecture

This document explains the technical architecture of the Formula Parser Demo application.

---

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                         USER                                │
└──────────────────┬──────────────────────────────────────────┘
                   │
        ┌──────────┴─────────┐
        │                    │
        v                    v
┌──────────────┐    ┌──────────────────┐
│   Web UI     │    │  External API    │
│ (HTML/CSS/JS)│    │   Consumers      │
└──────┬───────┘    └────────┬─────────┘
       │                     │
       └──────────┬──────────┘
                  │
                  v
        ┌─────────────────────┐
        │   ASP.NET Core      │
        │   Web API Layer     │
        │  (Controllers)      │
        └─────────┬───────────┘
                  │
                  v
        ┌─────────────────────┐
        │  Business Logic     │
        │   (Services)        │
        │  - Validation       │
        │  - Preview          │
        │  - Conversion       │
        │  - Application      │
        └─────────┬───────────┘
                  │
        ┌─────────┴─────────┐
        │                   │
        v                   v
┌──────────────┐    ┌──────────────┐
│ ANTLR Parser │    │  Database    │
│   & Visitor  │    │  (Npgsql +   │
│              │    │   Dapper)    │
└──────────────┘    └──────┬───────┘
                           │
                           v
                  ┌─────────────────┐
                  │   PostgreSQL    │
                  │    Database     │
                  └─────────────────┘
```

---

## Component Details

### 1. Presentation Layer

#### Web UI (wwwroot/)
- **Technology**: HTML5, CSS3, Vanilla JavaScript
- **Responsibilities**:
  - User input capture
  - Visual feedback
  - Preview display
  - Confirmation workflow

#### Files:
- `index.html` - Main UI structure
- `css/style.css` - Styling and layout
- `js/app.js` - Client-side logic

**Key Features:**
- Responsive design
- Real-time validation feedback
- Interactive column selection
- Formula examples modal

---

### 2. API Layer

#### FormulaController (Controllers/)
- **Technology**: ASP.NET Core Web API
- **Responsibilities**:
  - HTTP request/response handling
  - Input validation
  - Service orchestration
  - Error handling

**Endpoints:**
```
POST   /api/formula/validate     - Validate formula syntax
POST   /api/formula/preview      - Generate preview data
POST   /api/formula/convert      - Convert to SQL
POST   /api/formula/apply        - Execute SQL
GET    /api/formula/tables       - List tables
GET    /api/formula/tables/{name}/columns - List columns
GET    /api/formula/examples     - Get formula examples
```

---

### 3. Business Logic Layer

#### FormulaService (Services/)
- **Technology**: C# Service Classes
- **Responsibilities**:
  - Formula validation
  - Schema verification
  - SQL generation orchestration
  - Database operations
  - Error handling

**Key Methods:**

```csharp
Task<ValidateFormulaResponse> ValidateFormulaAsync(ValidateFormulaRequest)
  ↓
  1. Parse formula using ANTLR
  2. Validate column references against schema
  3. Detect data types
  4. Return validation result

Task<PreviewFormulaResponse> PreviewFormulaAsync(PreviewFormulaRequest)
  ↓
  1. Validate formula
  2. Generate SQL expression
  3. Execute preview query (limited rows)
  4. Return calculated values

Task<ConvertFormulaResponse> ConvertFormulaAsync(ConvertFormulaRequest)
  ↓
  1. Validate formula
  2. Generate SQL expression
  3. Build ALTER TABLE statement
  4. Return SQL without executing

Task<ApplyFormulaResponse> ApplyFormulaAsync(ApplyFormulaRequest)
  ↓
  1. Convert formula to SQL
  2. Check column existence
  3. Execute ALTER TABLE
  4. Return execution result
```

---

### 4. Formula Parsing Layer

#### ANTLR Components

**Formula.g4 (Grammar Definition)**
```
grammar Formula;

formula : expr EOF ;

expr
    : ifExpr                                    # IF expressions
    | aggregateFunction '(' expr ')'            # Aggregations
    | expr ('*'|'/'|'%') expr                   # Multiply/Divide
    | expr ('+'|'-') expr                       # Add/Subtract
    | expr comparisonOp expr                    # Comparisons
    | '(' expr ')'                              # Parentheses
    | NUMBER                                    # Numeric literals
    | STRING                                    # String literals
    | IDENTIFIER                                # Column references
    ;
```

**Generated Classes (ANTLR Build)**:
- `FormulaLexer` - Tokenization
- `FormulaParser` - Syntax analysis
- `FormulaBaseVisitor` - Base visitor pattern

**FormulaToSqlVisitor (Custom Visitor)**
- **Responsibility**: Convert Abstract Syntax Tree (AST) to SQL
- **Pattern**: Visitor Pattern
- **Output**: PostgreSQL-compatible SQL expressions

```csharp
Formula Input: "(product_sales + service_sales)"
       ↓
   [Lexer]
       ↓
   Tokens: [LPAREN, IDENTIFIER, PLUS, IDENTIFIER, RPAREN]
       ↓
   [Parser]
       ↓
   AST:
       AdditiveExpression
       ├── ColumnReference("product_sales")
       ├── Operator("+")
       └── ColumnReference("service_sales")
       ↓
   [Visitor]
       ↓
   SQL: "((product_sales + service_sales))"
```

---

### 5. Data Access Layer

#### Database Operations

**Technologies:**
- **Npgsql**: PostgreSQL connection driver
- **Dapper**: Micro-ORM for queries

**Responsibilities:**
- Connection management
- Schema inspection
- Query execution
- Transaction handling

**Key Operations:**

```csharp
// Schema inspection
GetTableSchemaAsync(tableName)
  ↓
  SELECT column_name, data_type, is_nullable
  FROM information_schema.columns
  WHERE table_name = @tableName

// Preview execution
ExecutePreviewAsync(sql, limit)
  ↓
  SELECT *, {formula} as {columnName}
  FROM {tableName}
  LIMIT @limit

// Column creation
ExecuteAlterTableAsync(sql)
  ↓
  ALTER TABLE {tableName}
  ADD COLUMN {columnName} {dataType}
  GENERATED ALWAYS AS ({formula}) STORED
```

---

### 6. Database Layer

#### PostgreSQL Database

**Schema: public**

**Main Table: sales_data**
```sql
CREATE TABLE sales_data (
    company_id SERIAL PRIMARY KEY,
    company_name VARCHAR(100) NOT NULL,
    product_sales NUMERIC(12, 2),
    service_sales NUMERIC(12, 2),
    operating_cost NUMERIC(12, 2),
    employee_count INTEGER,
    region VARCHAR(50),
    created_at TIMESTAMP,
    -- Computed columns added dynamically
);
```

**Features Used:**
- **GENERATED ALWAYS AS ... STORED**: Computed columns
- **NUMERIC**: High-precision decimal math
- **System Catalogs**: Schema introspection via `information_schema`

---

## Data Flow Diagrams

### Validation Flow

```
User Input
    ↓
Controller receives ValidateFormulaRequest
    ↓
FormulaService.ValidateFormulaAsync()
    ├─→ ParseFormula() using ANTLR
    │   ├─→ Lexer tokenizes input
    │   ├─→ Parser builds AST
    │   └─→ Visitor generates SQL
    │
    ├─→ GetTableSchemaAsync() from database
    │   └─→ Query information_schema.columns
    │
    └─→ Validate column references
        └─→ Return ValidateFormulaResponse
            ↓
Controller returns JSON response
    ↓
Web UI displays validation results
```

---

### Preview Flow

```
User clicks "Generate Preview"
    ↓
Controller receives PreviewFormulaRequest
    ↓
FormulaService.PreviewFormulaAsync()
    ├─→ Validate formula
    │
    ├─→ Generate SQL expression
    │
    └─→ Execute preview query
        └─→ SELECT *, {formula} as {column} FROM {table} LIMIT 5
            ↓
        Return sample rows with calculated values
            ↓
Controller returns PreviewFormulaResponse
    ↓
Web UI displays:
    ├─→ Statistics (total rows, successful, failed)
    ├─→ Preview table with calculated values
    └─→ Generated SQL
```

---

### Apply Flow

```
User clicks "Confirm & Apply"
    ↓
Controller receives ApplyFormulaRequest
    ↓
FormulaService.ApplyFormulaAsync()
    ├─→ Convert formula to SQL
    │   └─→ Generate ALTER TABLE statement
    │
    ├─→ Check if column exists
    │   └─→ Query information_schema.columns
    │
    └─→ Execute ALTER TABLE
        └─→ ALTER TABLE {table}
            ADD COLUMN {column} {type}
            GENERATED ALWAYS AS ({formula}) STORED
            ↓
        Database creates computed column
        Automatically calculates values for all rows
            ↓
Controller returns ApplyFormulaResponse
    ↓
Web UI shows success message
```

---

## Security Considerations

### Input Validation
- **ANTLR Grammar**: Restricts allowed syntax
- **Column Whitelisting**: Only existing columns allowed
- **Type Checking**: Data type compatibility verification
- **SQL Injection Prevention**: Parameterized queries via Dapper

### Database Security
- **Schema Isolation**: Operations limited to configured schema
- **Generated Columns**: Immutable, cannot be directly updated
- **Read-Only Execution Context**: Preview runs in read-only mode
- **Connection Pooling**: Managed by Npgsql

### API Security
- **CORS**: Configured for development (restrict in production)
- **HTTPS**: Enforced in production
- **Error Handling**: Sanitized error messages (no sensitive data leakage)

---

## Performance Optimizations

### Parsing Performance
- **ANTLR Caching**: Parser instances reused
- **Error Recovery**: Fast failure on syntax errors

### Database Performance
- **Stored Computed Columns**: Values pre-calculated and stored
- **Indexes**: Can be added on computed columns
- **Connection Pooling**: Managed by Npgsql
- **Preview Limits**: Only 5 rows fetched for preview

### Memory Management
- **Streaming**: Large result sets handled via streaming
- **Disposal Patterns**: Proper resource cleanup with `using` statements

---

## Extension Points

### Adding New Formula Types

1. **Extend Grammar** (`Formula.g4`)
   ```antlr
   expr
       : newFunctionType '(' expr ')'  # NewFunction
       | ...
   ```

2. **Update Visitor** (`FormulaToSqlVisitor.cs`)
   ```csharp
   public override string VisitNewFunction(context)
   {
       // Convert to SQL
   }
   ```

3. **Add Examples** (`FormulaController.GetExamples()`)

### Adding New Data Types

1. **Update Type Detection** (`FormulaToSqlVisitor.DetectDataType()`)
2. **Add Data Type Option** (Web UI dropdown)
3. **Test with Sample Data**

### Adding New Aggregate Functions

1. **Update Grammar** (`aggregateFunction` rule)
2. **Map to PostgreSQL** (`FormulaToSqlVisitor.VisitAggregateExpression()`)
3. **Add to Examples**

---

## Technology Stack Summary

| Layer | Technology | Purpose |
|-------|-----------|---------|
| **Frontend** | HTML5/CSS3/JavaScript | User interface |
| **API** | ASP.NET Core 8 | REST API endpoints |
| **Parser** | ANTLR4 | Formula parsing |
| **Business Logic** | C# | Service layer |
| **Data Access** | Dapper + Npgsql | Database operations |
| **Database** | PostgreSQL 12+ | Data storage |
| **Documentation** | Swagger/OpenAPI | API docs |

---

## Deployment Architecture

### Development
```
localhost:5001 (HTTPS)
    ↓
Single server running:
  - ASP.NET Core Web API
  - Static file serving
    ↓
localhost:5432
    ↓
PostgreSQL database
```

### Production (Recommended)
```
        [Load Balancer]
               ↓
    ┌──────────┴──────────┐
    │                     │
[Web Server 1]      [Web Server 2]
    │                     │
    └──────────┬──────────┘
               ↓
    [Database Server Pool]
               ↓
    [PostgreSQL Primary]
       ├── Replica 1
       └── Replica 2
```

---

## Monitoring & Logging

### Built-in Logging
- **ASP.NET Core Logging**: Request/response logging
- **Service Layer Logging**: Business logic events
- **Error Logging**: Exception details

### Recommended Additions
- **Application Insights**: Performance monitoring
- **Serilog**: Structured logging
- **PostgreSQL Query Logging**: Slow query detection
- **Health Checks**: Endpoint monitoring

---

## Testing Strategy

### Unit Tests
- Formula parsing logic
- SQL generation
- Data type detection

### Integration Tests
- API endpoints
- Database operations
- End-to-end formula application

### Performance Tests
- Large dataset handling
- Concurrent user scenarios
- Query optimization

---

**For implementation details, see the source code and inline documentation.**
