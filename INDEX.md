# Formula Parser Demo - Complete Index

## 📁 Directory Structure

```
formula-parser-demo/
│
├── 📂 FormulaParserDemo.Api/           # Main .NET 8 Web API Project
│   │
│   ├── 📂 ANTLR/
│   │   └── Formula.g4                  # ANTLR grammar (all 5 formula types)
│   │
│   ├── 📂 Controllers/
│   │   └── FormulaController.cs        # 7 API endpoints
│   │
│   ├── 📂 Services/
│   │   ├── IFormulaService.cs          # Service interface
│   │   └── FormulaService.cs           # Business logic (400+ lines)
│   │
│   ├── 📂 Visitors/
│   │   └── FormulaToSqlVisitor.cs      # AST → SQL converter (200+ lines)
│   │
│   ├── 📂 Models/
│   │   └── FormulaMetadata.cs          # Domain models
│   │
│   ├── 📂 DTOs/
│   │   ├── FormulaRequest.cs           # Request DTOs (4 models)
│   │   └── FormulaResponse.cs          # Response DTOs (4 models)
│   │
│   ├── 📂 wwwroot/                     # Static Web UI
│   │   ├── index.html                  # SPA interface
│   │   ├── 📂 css/
│   │   │   └── style.css               # Responsive CSS (400+ lines)
│   │   └── 📂 js/
│   │       └── app.js                  # Frontend logic (300+ lines)
│   │
│   ├── Program.cs                      # Application entry point
│   ├── appsettings.json                # Configuration
│   └── FormulaParserDemo.Api.csproj    # Project file
│
├── 📂 Database/
│   └── setup.sql                       # PostgreSQL schema + 20 sample rows
│
├── 📄 START_HERE.md                    # 👈 START HERE - Quick navigation
├── 📄 INDEX.md                         # This file - Complete file index
├── 📄 QUICK_START.md                   # 5-minute setup guide
├── 📄 README.md                        # Complete documentation (500+ lines)
├── 📄 TEST_SCENARIOS.md                # 20 comprehensive test cases
├── 📄 ARCHITECTURE.md                  # Technical architecture (500+ lines)
├── 📄 WORKFLOW_DIAGRAM.md              # Visual workflow diagrams
├── 📄 PROJECT_SUMMARY.md               # Project overview and stats
└── 📄 FormulaParserDemo.sln            # Solution file
```

---

## 📚 Documentation Files

### Getting Started

| File | Lines | Purpose | When to Read |
|------|-------|---------|--------------|
| **[START_HERE.md](START_HERE.md)** | 400 | Navigation & overview | **First file to read!** |
| **[QUICK_START.md](QUICK_START.md)** | 250 | 5-minute setup guide | Before running the app |
| **[INDEX.md](INDEX.md)** | 150 | This file - Complete index | For navigation |

### Feature Documentation

| File | Lines | Purpose | When to Read |
|------|-------|---------|--------------|
| **[README.md](README.md)** | 500+ | Complete features & examples | After basic setup |

### Testing & Validation

| File | Lines | Purpose | When to Read |
|------|-------|---------|--------------|
| **[TEST_SCENARIOS.md](TEST_SCENARIOS.md)** | 600+ | 20 detailed test cases | When testing the app |

### Technical Documentation

| File | Lines | Purpose | When to Read |
|------|-------|---------|--------------|
| **[ARCHITECTURE.md](ARCHITECTURE.md)** | 500+ | System architecture | For understanding design |
| **[WORKFLOW_DIAGRAM.md](WORKFLOW_DIAGRAM.md)** | 400+ | Visual diagrams | For understanding flow |

### Project Information

| File | Lines | Purpose | When to Read |
|------|-------|---------|--------------|
| **[PROJECT_SUMMARY.md](PROJECT_SUMMARY.md)** | 500+ | Overview & statistics | For project details |

---

## 🎯 Quick Navigation by Task

### I Want To...

**...Get Started Quickly**
→ Read: [START_HERE.md](START_HERE.md) → [QUICK_START.md](QUICK_START.md)

**...Understand What This Does**
→ Read: [START_HERE.md](START_HERE.md) → [README.md](README.md)

**...Set Up the Application**
→ Follow: [QUICK_START.md](QUICK_START.md)

**...See Example Formulas**
→ Read: [README.md](README.md) → Section "Formula Examples"

**...Test All Features**
→ Follow: [TEST_SCENARIOS.md](TEST_SCENARIOS.md)

**...Understand the Architecture**
→ Read: [ARCHITECTURE.md](ARCHITECTURE.md) → [WORKFLOW_DIAGRAM.md](WORKFLOW_DIAGRAM.md)

**...Extend the Application**
→ Read: [ARCHITECTURE.md](ARCHITECTURE.md) → Section "Extension Points"

**...See Project Statistics**
→ Read: [PROJECT_SUMMARY.md](PROJECT_SUMMARY.md)

**...Troubleshoot Issues**
→ Read: [QUICK_START.md](QUICK_START.md) → Section "Common Issues"

---

## 💻 Source Code Files

### Core Application (FormulaParserDemo.Api/)

| File | Lines | Purpose |
|------|-------|---------|
| **Program.cs** | 50 | Application entry point & configuration |
| **appsettings.json** | 15 | Database connection string |

### ANTLR Grammar (ANTLR/)

| File | Lines | Purpose |
|------|-------|---------|
| **Formula.g4** | 80 | Grammar definition for all 5 formula types |

### API Layer (Controllers/)

| File | Lines | Purpose |
|------|-------|---------|
| **FormulaController.cs** | 150 | REST API endpoints (7 endpoints) |

### Business Logic (Services/)

| File | Lines | Purpose |
|------|-------|---------|
| **IFormulaService.cs** | 15 | Service interface |
| **FormulaService.cs** | 400+ | Core business logic implementation |

### Parsing & SQL Generation (Visitors/)

| File | Lines | Purpose |
|------|-------|---------|
| **FormulaToSqlVisitor.cs** | 200+ | AST to PostgreSQL SQL converter |

### Data Models (Models/)

| File | Lines | Purpose |
|------|-------|---------|
| **FormulaMetadata.cs** | 40 | Domain models for formula metadata |

### Request/Response (DTOs/)

| File | Lines | Purpose |
|------|-------|---------|
| **FormulaRequest.cs** | 50 | API request models (4 classes) |
| **FormulaResponse.cs** | 60 | API response models (4 classes) |

### Frontend (wwwroot/)

| File | Lines | Purpose |
|------|-------|---------|
| **index.html** | 150 | Single-page web application |
| **css/style.css** | 400+ | Responsive styling |
| **js/app.js** | 300+ | Client-side logic |

### Database (Database/)

| File | Lines | Purpose |
|------|-------|---------|
| **setup.sql** | 100 | Schema creation + 20 sample rows |

---

## 🔌 API Endpoints Reference

Base URL: `https://localhost:5001/api/formula`

| Endpoint | File | Line | Purpose |
|----------|------|------|---------|
| `POST /validate` | FormulaController.cs | ~25 | Validate formula |
| `POST /preview` | FormulaController.cs | ~40 | Preview results |
| `POST /convert` | FormulaController.cs | ~55 | Generate SQL |
| `POST /apply` | FormulaController.cs | ~70 | Create column |
| `GET /tables` | FormulaController.cs | ~85 | List tables |
| `GET /tables/{name}/columns` | FormulaController.cs | ~95 | List columns |
| `GET /examples` | FormulaController.cs | ~105 | Get examples |

---

## 📖 Code Examples by Type

### Grammar (Formula.g4)

**Location**: `FormulaParserDemo.Api/ANTLR/Formula.g4`

**What it defines**:
- Math operators: `+`, `-`, `*`, `/`, `%`
- IF conditions: `IF(condition, true_value, false_value)`
- Aggregations: `SUM()`, `AVG()`, `MIN()`, `MAX()`, `COUNT()`
- Comparisons: `>`, `<`, `>=`, `<=`, `=`, `!=`
- Literals: Numbers, Strings
- Identifiers: Column names

### SQL Visitor (FormulaToSqlVisitor.cs)

**Location**: `FormulaParserDemo.Api/Visitors/FormulaToSqlVisitor.cs`

**Key methods**:
- `VisitAdditiveExpression()` - Handles `+`, `-`
- `VisitMultiplicativeExpression()` - Handles `*`, `/`, `%`
- `VisitComparisonExpression()` - Handles comparisons
- `VisitIfExpression()` - Converts IF to CASE
- `VisitAggregateExpression()` - Handles aggregate functions
- `DetectDataType()` - Auto-detects result type

### Business Logic (FormulaService.cs)

**Location**: `FormulaParserDemo.Api/Services/FormulaService.cs`

**Key methods**:
- `ValidateFormulaAsync()` - Validates syntax & columns
- `PreviewFormulaAsync()` - Executes preview query
- `ConvertFormulaAsync()` - Generates ALTER TABLE SQL
- `ApplyFormulaAsync()` - Executes SQL on database
- `GetTablesAsync()` - Lists available tables
- `GetColumnsAsync()` - Lists table columns

---

## 🧪 Test Files Reference

**Location**: [TEST_SCENARIOS.md](TEST_SCENARIOS.md)

### Test Categories

| Test Type | Count | Starting Line |
|-----------|-------|---------------|
| Type 1: Column Creation | 1 | ~15 |
| Type 2: Math Operations | 3 | ~30 |
| Type 3: Fixed Digits | 3 | ~150 |
| Type 4: IF Logic | 3 | ~270 |
| Type 5: Aggregations | 3 | ~390 |
| Combined Formulas | 2 | ~510 |
| Formula Reusability | 1 | ~580 |
| Error Handling | 4 | ~620 |
| Performance Tests | 1 | ~700 |

---

## 📊 Statistics Summary

### Code Statistics

| Category | Lines | Files |
|----------|-------|-------|
| C# Source Code | ~1,500 | 11 |
| ANTLR Grammar | ~80 | 1 |
| JavaScript | ~300 | 1 |
| HTML | ~150 | 1 |
| CSS | ~400 | 1 |
| SQL | ~100 | 1 |
| **Total Code** | **~2,530** | **16** |
| Documentation | ~2,000 | 7 |
| **Grand Total** | **~4,530** | **23** |

### Feature Statistics

| Feature | Count |
|---------|-------|
| Formula Types | 5 |
| API Endpoints | 7 |
| Test Scenarios | 20 |
| Documentation Pages | 7 |
| Sample Database Rows | 20 |
| Supported Operators | 12+ |
| Supported Functions | 10+ |

---

## 🔍 Search Index

### By Technology

**ANTLR**:
- Grammar: `ANTLR/Formula.g4`
- Visitor: `Visitors/FormulaToSqlVisitor.cs`
- Docs: ARCHITECTURE.md (line ~240)

**PostgreSQL**:
- Connection: `appsettings.json`
- Queries: `Services/FormulaService.cs`
- Setup: `Database/setup.sql`
- Docs: README.md (line ~450)

**.NET/C#**:
- Entry Point: `Program.cs`
- Controllers: `Controllers/FormulaController.cs`
- Services: `Services/FormulaService.cs`
- Models: `Models/`, `DTOs/`

**Frontend**:
- HTML: `wwwroot/index.html`
- CSS: `wwwroot/css/style.css`
- JavaScript: `wwwroot/js/app.js`

### By Feature

**Formula Types**:
- Type 1: Column Creation - README.md (line ~125)
- Type 2: Math Operations - README.md (line ~135)
- Type 3: Fixed Digits - README.md (line ~145)
- Type 4: IF Logic - README.md (line ~155)
- Type 5: Aggregations - README.md (line ~165)

**Validation**:
- Code: `Services/FormulaService.cs` (line ~40)
- Docs: ARCHITECTURE.md (line ~350)
- Tests: TEST_SCENARIOS.md (line ~620)

**Preview**:
- Code: `Services/FormulaService.cs` (line ~130)
- Docs: WORKFLOW_DIAGRAM.md (line ~180)
- Tests: TEST_SCENARIOS.md (all tests)

---

## 🚀 Getting Started Checklist

Based on this index:

- [ ] Read [START_HERE.md](START_HERE.md) for overview
- [ ] Follow [QUICK_START.md](QUICK_START.md) for setup
- [ ] Run the application
- [ ] Try examples from [README.md](README.md)
- [ ] Execute tests from [TEST_SCENARIOS.md](TEST_SCENARIOS.md)
- [ ] Study architecture in [ARCHITECTURE.md](ARCHITECTURE.md)
- [ ] Review diagrams in [WORKFLOW_DIAGRAM.md](WORKFLOW_DIAGRAM.md)

---

## 📞 Quick Reference

| Need | Go To |
|------|-------|
| **Setup** | QUICK_START.md |
| **Examples** | README.md → "Formula Examples" |
| **API Docs** | https://localhost:5001 (Swagger) |
| **Tests** | TEST_SCENARIOS.md |
| **Architecture** | ARCHITECTURE.md |
| **Troubleshooting** | QUICK_START.md → "Common Issues" |
| **Extension Guide** | ARCHITECTURE.md → "Extension Points" |

---

## 🎓 Learning Path

### Beginner (Just Use It)
1. START_HERE.md
2. QUICK_START.md
3. README.md (examples section)
4. Try in Web UI

### Intermediate (Understand It)
1. ARCHITECTURE.md
2. WORKFLOW_DIAGRAM.md
3. Read source code
4. TEST_SCENARIOS.md (run all tests)

### Advanced (Extend It)
1. Study Formula.g4 grammar
2. Study FormulaToSqlVisitor.cs
3. Modify grammar
4. Add new features

---

**This index provides complete navigation for the Formula Parser Demo project.**

**Start Here**: [START_HERE.md](START_HERE.md)

**Run Command**: `cd FormulaParserDemo.Api && dotnet run`
