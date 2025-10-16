# 🚀 START HERE - Formula Parser Demo

Welcome! This document will guide you through everything you need to know about this project.

---

## 📌 What Is This Project?

A complete .NET 8 application that parses user formulas (like Excel formulas) and converts them into PostgreSQL computed columns. Built with ANTLR4 for parsing, it supports 5 types of formulas including math operations, IF logic, and aggregations.

**Example:**
```
User enters: (product_sales + service_sales) - operating_cost
System creates: A PostgreSQL computed column that auto-calculates profit
```

---

## ⚡ Quick Start (5 Minutes)

### Prerequisites
- .NET 8 SDK installed
- PostgreSQL 12+ installed
- Default PostgreSQL password: `postgres`

### Setup Commands

```bash
# 1. Navigate to project
cd D:\MyRepositories\Utility

# 2. Create database
psql -U postgres -c "CREATE DATABASE formulademo;"
psql -U postgres -d formulademo -f Database/setup.sql

# 3. Run application
cd FormulaParserDemo.Api
dotnet run
```

### Access the Application

- **Web UI**: https://localhost:5001/index.html
- **API Docs**: https://localhost:5001 (Swagger)

### Try Your First Formula

1. Open Web UI
2. Select table: `sales_data`
3. Click "Load Columns"
4. Enter:
   - Column Name: `total_revenue`
   - Formula: `(product_sales + service_sales)`
5. Click "Validate" → "Generate Preview" → "Confirm & Apply"
6. Done! Column created with calculated values.

---

## 📚 Documentation Guide

### For First-Time Users

1. **[QUICK_START.md](QUICK_START.md)** ← Start here!
   - 5-minute setup guide
   - Common issues & fixes
   - Success checklist

2. **[README.md](README.md)**
   - Complete features overview
   - Formula examples (all 5 types)
   - API endpoint documentation

### For Testing

3. **[TEST_SCENARIOS.md](TEST_SCENARIOS.md)**
   - 20 comprehensive test cases
   - Step-by-step instructions
   - Verification queries
   - Test data cleanup

### For Understanding the System

4. **[ARCHITECTURE.md](ARCHITECTURE.md)**
   - System architecture diagrams
   - Component details
   - Data flow explanations
   - Technology stack

5. **[WORKFLOW_DIAGRAM.md](WORKFLOW_DIAGRAM.md)**
   - Visual workflow diagrams
   - Formula parsing process
   - Error handling flows

### For Project Overview

6. **[PROJECT_SUMMARY.md](PROJECT_SUMMARY.md)**
   - What was built
   - Features completed
   - Project statistics
   - Design decisions

---

## 🎯 Supported Formula Types

### Type 1: Column Creation
```sql
-- Create empty column
-- (handled via standard ALTER TABLE)
```

### Type 2: Math Operations
```sql
(product_sales + service_sales)
(product_sales + service_sales) - operating_cost
(product_sales + service_sales) / employee_count
```

### Type 3: Fixed Digits
```sql
product_sales * 1.15              -- 15% increase
(product_sales + service_sales) / 12   -- Monthly average
```

### Type 4: IF Logic
```sql
IF(employee_count > 50, "Large", "Small")
IF((product_sales + service_sales) > 100000, "High", "Standard")
```

### Type 5: Aggregations
```sql
(product_sales / SUM(product_sales)) * 100
IF(product_sales > AVG(product_sales), "Above", "Below")
```

**Click "Show Examples" in the Web UI to see more!**

---

## 🔧 Project Structure

```
D:\MyRepositories\Utility\
│
├── 📂 FormulaParserDemo.Api/        Main application
│   ├── ANTLR/                        Grammar definition
│   ├── Controllers/                  API endpoints
│   ├── Services/                     Business logic
│   ├── Visitors/                     SQL converter
│   ├── Models/                       Domain models
│   ├── DTOs/                         Request/response
│   └── wwwroot/                      Web UI
│
├── 📂 Database/
│   └── setup.sql                     Database schema
│
├── 📄 START_HERE.md                  ← You are here
├── 📄 QUICK_START.md                 Setup guide
├── 📄 README.md                      Complete docs
├── 📄 TEST_SCENARIOS.md              Test cases
├── 📄 ARCHITECTURE.md                Technical details
├── 📄 WORKFLOW_DIAGRAM.md            Visual diagrams
└── 📄 PROJECT_SUMMARY.md             Overview
```

---

## 🎬 Usage Scenarios

### Scenario 1: Business Analyst
**Goal**: Calculate total revenue per company

1. Open Web UI
2. Select `sales_data` table
3. Formula: `(product_sales + service_sales)`
4. Column: `total_revenue`
5. Preview → Apply
6. **Result**: Every company now has a `total_revenue` column that auto-updates!

### Scenario 2: Data Scientist
**Goal**: Identify high performers

1. Formula: `IF(product_sales > AVG(product_sales), "Above Average", "Below Average")`
2. Column: `performance_category`
3. Apply
4. **Result**: Each company labeled as Above/Below average automatically!

### Scenario 3: Financial Analyst
**Goal**: Calculate contribution percentage

1. Formula: `((product_sales + service_sales - operating_cost) / SUM(product_sales + service_sales - operating_cost)) * 100`
2. Column: `profit_contribution_pct`
3. Apply
4. **Result**: Each company shows its % contribution to total profit!

---

## 🔌 API Quick Reference

All endpoints use base URL: `/api/formula`

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/validate` | POST | Check formula syntax |
| `/preview` | POST | See calculated values |
| `/convert` | POST | Generate SQL only |
| `/apply` | POST | Create column |
| `/tables` | GET | List tables |
| `/tables/{name}/columns` | GET | List columns |
| `/examples` | GET | Get formula examples |

**Full API docs**: https://localhost:5001 (Swagger)

---

## 🧪 Testing Checklist

Follow these tests in order:

- [ ] **Setup Test**: Verify database has 20 companies
- [ ] **Type 2 Test**: Create `total_revenue` column
- [ ] **Type 3 Test**: Create `projected_sales` with multiplier
- [ ] **Type 4 Test**: Create `company_size` with IF
- [ ] **Type 5 Test**: Create `sales_percentage` with SUM
- [ ] **Reusability Test**: Use `total_revenue` in new formula
- [ ] **Error Test**: Try invalid column name

**Detailed test steps**: See [TEST_SCENARIOS.md](TEST_SCENARIOS.md)

---

## ❓ FAQ

### Q: What database does this use?
**A:** PostgreSQL 12 or higher. The application creates computed columns using `GENERATED ALWAYS AS ... STORED`.

### Q: Can I use this with other databases?
**A:** The current implementation is PostgreSQL-specific. You'd need to modify the SQL generation for other databases.

### Q: How do I undo a formula?
**A:** Use PostgreSQL to drop the column:
```sql
ALTER TABLE sales_data DROP COLUMN column_name;
```

### Q: Can formulas reference other computed columns?
**A:** Yes! This is called "formula reusability". Create a base formula first, then reference it in new formulas.

### Q: What happens if the formula is invalid?
**A:** The system validates before execution and shows clear error messages. Nothing is written to the database until you confirm.

### Q: How fast is it?
**A:** Formula parsing is instant. Database column creation depends on table size (PostgreSQL calculates all values immediately).

### Q: Can multiple users use it simultaneously?
**A:** Yes! Each user's preview is independent. Database writes are handled by PostgreSQL's locking mechanism.

---

## 🐛 Troubleshooting

### Issue: "Connection refused"
```bash
# Check if PostgreSQL is running
pg_isready

# If not running, start it:
# Windows: Start PostgreSQL service
# Linux: sudo systemctl start postgresql
# Mac: brew services start postgresql
```

### Issue: "Database does not exist"
```bash
# Create the database
psql -U postgres -c "CREATE DATABASE formulademo;"

# Run setup
psql -U postgres -d formulademo -f Database/setup.sql
```

### Issue: "Build failed"
```bash
# Clean and rebuild
cd FormulaParserDemo.Api
dotnet clean
dotnet build
```

### Issue: "Column already exists"
```sql
-- Drop the existing column first
ALTER TABLE sales_data DROP COLUMN column_name;
```

**More troubleshooting**: See [QUICK_START.md](QUICK_START.md)

---

## 🎓 Learning Path

### Beginner: Just Use It
1. Follow QUICK_START.md
2. Try example formulas in Web UI
3. View preview results
4. Apply and verify in PostgreSQL

### Intermediate: Understand It
1. Read ARCHITECTURE.md
2. Study the workflow diagrams
3. Examine the ANTLR grammar
4. Run all test scenarios

### Advanced: Extend It
1. Study the Visitor pattern implementation
2. Add new operators to grammar
3. Implement new aggregate functions
4. Add custom formula types

---

## 🚀 Next Steps After Setup

### Try These Formulas

**Simple Math**:
```
total_revenue = (product_sales + service_sales)
```

**Percentage**:
```
market_share = (product_sales / SUM(product_sales)) * 100
```

**Classification**:
```
size_category = IF(employee_count > 50, "Large", "Small")
```

**Combined**:
```
performance = IF(
  (product_sales + service_sales) > AVG(product_sales + service_sales),
  "High Performer",
  "Standard"
)
```

### Explore the Code

**Grammar Definition**:
- Open: `FormulaParserDemo.Api/ANTLR/Formula.g4`
- See how formulas are parsed

**SQL Converter**:
- Open: `FormulaParserDemo.Api/Visitors/FormulaToSqlVisitor.cs`
- See how AST is converted to SQL

**Business Logic**:
- Open: `FormulaParserDemo.Api/Services/FormulaService.cs`
- See validation and execution

### Read Documentation

Start with what interests you:
- 📖 Features? → README.md
- 🏗️ Architecture? → ARCHITECTURE.md
- 🧪 Testing? → TEST_SCENARIOS.md
- 📊 Overview? → PROJECT_SUMMARY.md

---

## 💡 Key Features

✅ **5 Formula Types** - Math, fixed digits, IF logic, aggregations, column creation
✅ **Smart Validation** - Syntax checking, column existence, type detection
✅ **Safe Preview** - See results before committing
✅ **Auto-Calculation** - PostgreSQL computes all values automatically
✅ **Formula Reuse** - Build on previous formulas
✅ **Web UI** - Easy-to-use interface
✅ **REST API** - Programmatic access
✅ **Swagger Docs** - Interactive API testing

---

## 📞 Getting Help

1. **Check docs** in this directory (especially QUICK_START.md)
2. **View Swagger** at https://localhost:5001 for API help
3. **Read error messages** - they're descriptive!
4. **Test in PostgreSQL** - Verify data directly

---

## 📊 Project Stats

- **Total Files**: 23 files created
- **Documentation**: 2,000+ lines across 5 docs
- **Code**: 1,500+ lines of C#
- **Test Scenarios**: 20 comprehensive tests
- **Supported Operators**: +, -, *, /, %, >, <, >=, <=, =, !=
- **Supported Functions**: IF, SUM, AVG, MIN, MAX, COUNT, etc.

---

## 🎯 Success Criteria

You'll know it's working when:
- ✅ Application runs on https://localhost:5001
- ✅ You can load tables and columns
- ✅ Validation shows ✓ Valid
- ✅ Preview shows calculated values
- ✅ Apply creates the column successfully
- ✅ PostgreSQL shows the new computed column

---

## 🎉 You're Ready!

This project demonstrates:
- ✨ ANTLR4 parser in .NET
- ✨ AST to SQL conversion
- ✨ PostgreSQL computed columns
- ✨ Clean architecture
- ✨ RESTful API design
- ✨ Interactive web UI

**Start with [QUICK_START.md](QUICK_START.md) and you'll be creating formulas in 5 minutes!**

---

## 📝 Document Map

```
START_HERE.md              ← You are here (overview & navigation)
    │
    ├─→ QUICK_START.md     (5-minute setup)
    │     └─→ README.md    (complete features)
    │
    ├─→ TEST_SCENARIOS.md  (20 test cases)
    │
    ├─→ ARCHITECTURE.md    (technical design)
    │     └─→ WORKFLOW_DIAGRAM.md  (visual diagrams)
    │
    └─→ PROJECT_SUMMARY.md (what was built)
```

---

**🚀 Ready to parse some formulas? Start with QUICK_START.md!**

**Location**: `D:\MyRepositories\Utility\`

**Run Command**: `cd FormulaParserDemo.Api && dotnet run`

**Access**: `https://localhost:5001/index.html`

---

**Built with ❤️ using .NET 8, ANTLR4, and PostgreSQL**
