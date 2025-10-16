# Utility Function Workflow Diagrams

Visual representation of how the Formula Parser Demo works.

---

## Complete User Workflow

```
┌─────────────────────────────────────────────────────────────────┐
│                        USER STARTS                              │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             v
                    ┌────────────────┐
                    │ Select Table   │
                    │ (sales_data)   │
                    └────────┬───────┘
                             │
                             v
                    ┌────────────────┐
                    │ Load Columns   │◄─────── GET /api/formula/tables/{name}/columns
                    └────────┬───────┘
                             │
                             v
                    ┌────────────────────────┐
                    │ Enter Formula Details  │
                    │ - Column Name          │
                    │ - Formula Expression   │
                    │ - Data Type (optional) │
                    └────────┬───────────────┘
                             │
                             v
                    ┌────────────────┐
                    │ Click Validate │
                    └────────┬───────┘
                             │
                             v
               ┌─────────────────────────┐
               │ POST /api/formula/      │
               │       validate          │
               └────────┬────────────────┘
                        │
            ┌───────────┴──────────┐
            v                      v
    ┌───────────┐          ┌──────────┐
    │  Invalid  │          │  Valid   │
    │  Errors   │          │  ✓       │
    └───────┬───┘          └────┬─────┘
            │                   │
            v                   v
    ┌───────────┐      ┌────────────────┐
    │ Fix       │      │ Click Preview  │
    │ Formula   │      └────────┬───────┘
    └───────────┘               │
                                v
                  ┌─────────────────────────┐
                  │ POST /api/formula/      │
                  │       preview           │
                  └────────┬────────────────┘
                           │
                           v
              ┌────────────────────────┐
              │ Show Preview Results:  │
              │ - Sample calculations  │
              │ - Statistics           │
              │ - Generated SQL        │
              └────────┬───────────────┘
                       │
           ┌───────────┴───────────┐
           v                       v
   ┌───────────────┐       ┌──────────────┐
   │ Not Satisfied │       │  Satisfied   │
   │ Click Cancel  │       │ Click Apply  │
   └───────┬───────┘       └──────┬───────┘
           │                      │
           v                      v
   ┌───────────┐      ┌──────────────────────┐
   │ Start     │      │ POST /api/formula/   │
   │ Over      │      │       apply          │
   └───────────┘      └──────────┬───────────┘
                                 │
                                 v
                      ┌──────────────────────┐
                      │ ALTER TABLE executed │
                      │ Column created       │
                      └──────────┬───────────┘
                                 │
                                 v
                      ┌──────────────────────┐
                      │ Show Success Message │
                      │ Display SQL executed │
                      └──────────┬───────────┘
                                 │
                                 v
                      ┌──────────────────────┐
                      │ Reload to create     │
                      │ another formula?     │
                      └──────────────────────┘
```

---

## Formula Parsing Flow

```
User Formula String
        │
        v
┌───────────────┐
│ Input Stream  │  "((product_sales + service_sales))"
└───────┬───────┘
        │
        v
┌───────────────┐
│ ANTLR Lexer   │  Tokenization
└───────┬───────┘
        │
        v
    Token Stream
    ├── LPAREN
    ├── LPAREN
    ├── IDENTIFIER (product_sales)
    ├── PLUS
    ├── IDENTIFIER (service_sales)
    ├── RPAREN
    └── RPAREN
        │
        v
┌───────────────┐
│ ANTLR Parser  │  Syntax Analysis
└───────┬───────┘
        │
        v
   Abstract Syntax Tree (AST)
        │
        formula
         └── expr (Parenthesized)
              └── expr (Additive)
                   ├── expr (Column)
                   │    └── product_sales
                   ├── operator (+)
                   └── expr (Column)
                        └── service_sales
        │
        v
┌────────────────────┐
│ FormulaToSqlVisitor│  Tree Walking
└────────┬───────────┘
         │
         v
    Visit each node:
    1. VisitParenthesizedExpression
    2. VisitAdditiveExpression
    3. VisitColumnReference (product_sales)
    4. VisitColumnReference (service_sales)
         │
         v
┌────────────────────┐
│ SQL Expression     │  "((product_sales + service_sales))"
└────────────────────┘
```

---

## Validation Process

```
ValidateFormulaRequest
        │
        v
┌─────────────────────┐
│ 1. Parse Formula    │
│    using ANTLR      │
└────────┬────────────┘
         │
         ├─ Syntax Error? ──Yes──> Return errors
         │
         No
         │
         v
┌─────────────────────┐
│ 2. Get Table Schema │
│    from PostgreSQL  │
└────────┬────────────┘
         │
         v
┌─────────────────────────┐
│ 3. Validate Columns     │
│    For each referenced  │
│    column in formula:   │
│    - Check if exists    │
│    - Get data type      │
│    - Check if computed  │
└────────┬────────────────┘
         │
         ├─ Column not found? ──Yes──> Add to errors
         │
         No
         │
         v
┌─────────────────────┐
│ 4. Detect Data Type │
│    - Math → NUMERIC │
│    - String → TEXT  │
│    - IF → TEXT      │
│    - Comparison → BOOLEAN
└────────┬────────────┘
         │
         v
┌─────────────────────┐
│ 5. Check for        │
│    Aggregations     │
│    - Add warning if │
│      aggregate used │
└────────┬────────────┘
         │
         v
┌─────────────────────┐
│ Return Validation   │
│ Response:           │
│ - isValid           │
│ - errors []         │
│ - warnings []       │
│ - referencedColumns │
│ - detectedDataType  │
└─────────────────────┘
```

---

## Preview Process

```
PreviewFormulaRequest
        │
        v
┌─────────────────────┐
│ 1. Validate Formula │
└────────┬────────────┘
         │
         ├─ Invalid? ──Yes──> Return errors
         │
         No
         │
         v
┌─────────────────────┐
│ 2. Parse to SQL     │
│    Expression       │
└────────┬────────────┘
         │
         v
┌─────────────────────────────┐
│ 3. Build Preview Query      │
│                             │
│    SELECT                   │
│      *,                     │
│      {sqlExpression}        │
│        as {columnName}      │
│    FROM {tableName}         │
│    LIMIT 5                  │
└────────┬────────────────────┘
         │
         v
┌─────────────────────┐
│ 4. Execute Query    │
│    on PostgreSQL    │
└────────┬────────────┘
         │
         ├─ SQL Error? ──Yes──> Return error details
         │
         No
         │
         v
┌─────────────────────┐
│ 5. Process Results  │
│    - Convert to JSON│
│    - Count stats    │
│    - Detect failures│
└────────┬────────────┘
         │
         v
┌─────────────────────┐
│ 6. Get Total Rows   │
│    SELECT COUNT(*)  │
│    FROM {table}     │
└────────┬────────────┘
         │
         v
┌─────────────────────────┐
│ Return Preview Response:│
│ - success               │
│ - previewData []        │
│ - generatedSql          │
│ - detectedDataType      │
│ - totalRowsAffected     │
│ - successfulCalculations│
│ - failedCalculations    │
└─────────────────────────┘
```

---

## Apply Process

```
ApplyFormulaRequest
        │
        v
┌─────────────────────┐
│ 1. Convert to SQL   │
└────────┬────────────┘
         │
         ├─ Conversion failed? ──Yes──> Return errors
         │
         No
         │
         v
┌─────────────────────────────┐
│ 2. Check Column Existence   │
│    SELECT EXISTS (          │
│      SELECT 1               │
│      FROM information_      │
│           schema.columns    │
│      WHERE                  │
│        table_name = ...     │
│        AND column_name = ...│
│    )                        │
└────────┬────────────────────┘
         │
         ├─ Already exists? ──Yes──> Return error
         │
         No
         │
         v
┌─────────────────────────────┐
│ 3. Execute ALTER TABLE      │
│                             │
│    ALTER TABLE {tableName}  │
│    ADD COLUMN {columnName}  │
│      {dataType}             │
│    GENERATED ALWAYS AS      │
│      ({sqlExpression})      │
│    STORED;                  │
└────────┬────────────────────┘
         │
         ├─ SQL Error? ──Yes──> Return error
         │
         No
         │
         v
┌─────────────────────┐
│ 4. PostgreSQL       │
│    Automatically:   │
│    - Creates column │
│    - Calculates     │
│      values for ALL │
│      existing rows  │
│    - Stores values  │
└────────┬────────────┘
         │
         v
┌─────────────────────┐
│ 5. Get Row Count    │
│    SELECT COUNT(*)  │
│    FROM {table}     │
└────────┬────────────┘
         │
         v
┌─────────────────────┐
│ Return Apply        │
│ Response:           │
│ - success           │
│ - message           │
│ - executedSql       │
│ - rowsAffected      │
└─────────────────────┘
```

---

## Formula Type Processing

### Type 2: Math Operations

```
Formula: (product_sales + service_sales)
    │
    v
Parser creates: AdditiveExpression
    ├── Left: ColumnReference(product_sales)
    ├── Op: +
    └── Right: ColumnReference(service_sales)
    │
    v
Visitor generates: ((product_sales + service_sales))
    │
    v
SQL Result: ALTER TABLE sales_data
            ADD COLUMN total_revenue NUMERIC
            GENERATED ALWAYS AS
              ((product_sales + service_sales))
            STORED;
```

### Type 4: IF Logic

```
Formula: IF(employee_count > 50, "Large", "Small")
    │
    v
Parser creates: IfExpression
    ├── Condition: ComparisonExpression
    │   ├── Left: ColumnReference(employee_count)
    │   ├── Op: >
    │   └── Right: NumberLiteral(50)
    ├── TrueValue: StringLiteral("Large")
    └── FalseValue: StringLiteral("Small")
    │
    v
Visitor generates:
    CASE WHEN (employee_count > 50)
         THEN 'Large'
         ELSE 'Small'
    END
    │
    v
SQL Result: ALTER TABLE sales_data
            ADD COLUMN company_size TEXT
            GENERATED ALWAYS AS
              (CASE WHEN (employee_count > 50)
                    THEN 'Large'
                    ELSE 'Small'
               END)
            STORED;
```

### Type 5: Aggregations

```
Formula: (product_sales / SUM(product_sales)) * 100
    │
    v
Parser creates: MultiplicativeExpression
    ├── Left: ParenthesizedExpression
    │   └── Division
    │       ├── Left: ColumnReference(product_sales)
    │       └── Right: AggregateExpression
    │           └── SUM(ColumnReference(product_sales))
    ├── Op: *
    └── Right: NumberLiteral(100)
    │
    v
Visitor generates:
    ((product_sales / SUM(product_sales)) * 100)
    │
    v
SQL Result: ALTER TABLE sales_data
            ADD COLUMN sales_percentage NUMERIC
            GENERATED ALWAYS AS
              ((product_sales / SUM(product_sales)) * 100)
            STORED;
```

---

## Error Handling Flow

```
Any Operation
    │
    v
Try {
    Execute operation
}
    │
    v
Catch errors:
    │
    ├─ ANTLR Syntax Error
    │   └─> Return: "Syntax error at line X:Y - {message}"
    │
    ├─ Column Not Found
    │   └─> Return: "Column '{name}' does not exist in table"
    │
    ├─ Type Mismatch
    │   └─> Return: "Invalid operation between {type1} and {type2}"
    │
    ├─ Division by Zero
    │   └─> Handle: Set result to NULL, add warning
    │
    ├─ Column Already Exists
    │   └─> Return: "Column '{name}' already exists"
    │
    ├─ SQL Execution Error
    │   └─> Return: "Database error: {message}"
    │
    └─ Unexpected Error
        └─> Log error, Return: "Unexpected error occurred"
```

---

## Database Computed Column Behavior

```
When ALTER TABLE executes:

┌────────────────────────────────────────────────┐
│ PostgreSQL Database                            │
│                                                │
│ 1. Schema Modification Phase                  │
│    ├─> Add column definition to catalog       │
│    ├─> Store expression in pg_attrdef         │
│    └─> Mark column as GENERATED               │
│                                                │
│ 2. Data Population Phase (automatic)          │
│    FOR EACH existing row:                     │
│      ├─> Evaluate expression                  │
│      ├─> Calculate value                      │
│      └─> Store in new column                  │
│                                                │
│ 3. Future Behavior                            │
│    ON INSERT:                                 │
│      └─> Auto-calculate value                 │
│                                                │
│    ON UPDATE (of base columns):               │
│      └─> Auto-recalculate value               │
│                                                │
│    ON UPDATE (of computed column):            │
│      └─> ERROR: Cannot update generated column│
└────────────────────────────────────────────────┘
```

---

## Formula Reusability Example

```
Step 1: Create base column
┌────────────────────────────────┐
│ Formula: (product_sales +      │
│           service_sales)       │
│ Column: total_revenue          │
└───────────┬────────────────────┘
            │
            v
    ALTER TABLE sales_data
    ADD COLUMN total_revenue NUMERIC
    GENERATED ALWAYS AS
      ((product_sales + service_sales))
    STORED;
            │
            v
    ┌───────────────┐
    │ Column exists │
    │ in schema     │
    └───────┬───────┘
            │
Step 2: Use in new formula
            │
            v
┌────────────────────────────────────┐
│ Formula: (total_revenue -          │
│           operating_cost) /        │
│          total_revenue * 100       │
│ Column: profit_margin              │
└───────────┬────────────────────────┘
            │
            v
    Schema validation:
    ✓ total_revenue exists
    ✓ operating_cost exists
            │
            v
    ALTER TABLE sales_data
    ADD COLUMN profit_margin NUMERIC
    GENERATED ALWAYS AS
      ((total_revenue - operating_cost) /
       total_revenue * 100)
    STORED;
            │
            v
    Both columns now in table:
    - total_revenue (references base columns)
    - profit_margin (references total_revenue)
```

---

## Performance Characteristics

```
Operation               Time Complexity    Notes
─────────────────────────────────────────────────────────
Parse Formula           O(n)               n = formula length
Validate Columns        O(m)               m = columns referenced
Preview Query           O(5)               Fixed 5 rows
Apply (Create Column)   O(r)               r = total rows in table
                                          (PostgreSQL calculates
                                           all values)

Subsequent Queries      O(1)               Values pre-calculated
                                          and stored
```

---

## Concurrent User Handling

```
User A                      User B
  │                          │
  ├─ Select table            ├─ Select table
  ├─ Enter formula           ├─ Enter formula
  ├─ Preview                 ├─ Preview
  │  (Independent)           │  (Independent)
  ├─ Apply                   │
  │  CREATE COLUMN A         ├─ Apply
  │  [LOCKED]                │  CREATE COLUMN B
  └─ Success                 │  [WAITS for lock]
                             └─ Success

PostgreSQL handles:
- Schema-level locking
- Transaction isolation
- Concurrent reads
- Sequential writes
```

---

**These diagrams illustrate the complete workflow from user input to database execution.**

**For code implementation details, see the source files and ARCHITECTURE.md.**
