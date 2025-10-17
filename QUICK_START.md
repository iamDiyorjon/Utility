# Quick Start Guide

Get the Formula Parser Demo up and running in 5 minutes!

## Prerequisites Check

Before you begin, verify you have:

- [ ] **.NET 8 SDK** installed
  ```bash
  dotnet --version
  # Should show 8.x.x or higher
  ```

- [ ] **PostgreSQL** installed and running
  ```bash
  psql --version
  # Should show PostgreSQL 12.x or higher
  ```

---

## Step-by-Step Setup

### 1. Database Setup (2 minutes)

**Option A: Quick Setup (Recommended)**

```bash
# Navigate to project directory (replace with your path)
cd /path/to/formula-parser-demo

# Create database
psql -U postgres -c "CREATE DATABASE formulademo;"

# Run setup script
psql -U postgres -d formulademo -f Database/setup.sql
```

**Option B: Using pgAdmin**
1. Open pgAdmin
2. Right-click "Databases" → "Create" → "Database"
3. Name: `formulademo`
4. Open Query Tool
5. Load and execute `Database/setup.sql`

**Verify Database:**
```bash
psql -U postgres -d formulademo -c "SELECT COUNT(*) FROM sales_data;"
# Should return: 20
```

---

### 2. Configure Connection (30 seconds)

Edit `FormulaParserDemo.Api/appsettings.json` only if your password is different:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=formulademo;Username=postgres;Password=YOUR_PASSWORD_HERE"
  }
}
```

---

### 3. Build and Run (1 minute)

```bash
cd FormulaParserDemo.Api
dotnet build
dotnet run
```

Wait for this message:
```
Now listening on: https://localhost:5001
```

---

### 4. Test the Application (2 minutes)

**Option A: Web UI (Recommended)**

1. Open browser: `https://localhost:5001/index.html`
2. Select table: `sales_data`
3. Click "Load Columns"
4. Enter formula: `(product_sales + service_sales)`
5. Column name: `total_revenue`
6. Click "Validate" → Should show ✓ Valid
7. Click "Generate Preview" → See calculated values
8. Click "✓ Confirm & Apply" → Column created!

**Option B: Swagger API**

1. Open browser: `https://localhost:5001`
2. Click "Formula" section
3. Try "POST /api/formula/validate"
4. Use this JSON:
   ```json
   {
     "tableName": "sales_data",
     "formula": "(product_sales + service_sales)"
   }
   ```
5. Click "Execute"

**Option C: Command Line**

```bash
# Test validation (Windows PowerShell)
$body = @{
    tableName = "sales_data"
    formula = "(product_sales + service_sales)"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/formula/validate" `
  -Method POST `
  -Body $body `
  -ContentType "application/json" `
  -SkipCertificateCheck
```

---

## Verify Installation

### Check Database

```sql
psql -U postgres -d formulademo

-- Check data
SELECT * FROM sales_data LIMIT 5;

-- Check columns
\d sales_data

-- Exit
\q
```

### Check API

```bash
curl https://localhost:5001/api/formula/tables
# Should return: ["sales_data"]
```

### Check Generated Column

If you created `total_revenue`:

```sql
SELECT company_name, product_sales, service_sales, total_revenue
FROM sales_data
LIMIT 5;
```

---

## Common Issues

### Issue: "Connection refused"

**Solution:**
```bash
# Check PostgreSQL is running
pg_isready

# Start PostgreSQL (if not running)
# Windows: Start PostgreSQL service from Services
# Linux: sudo systemctl start postgresql
# Mac: brew services start postgresql
```

### Issue: "Database does not exist"

**Solution:**
```bash
# Create the database
psql -U postgres -c "CREATE DATABASE formulademo;"

# Run setup
psql -U postgres -d formulademo -f Database/setup.sql
```

### Issue: "Password authentication failed"

**Solution:**
Update the password in `appsettings.json` to match your PostgreSQL password.

### Issue: "Port 5001 already in use"

**Solution:**
```bash
# Edit Properties/launchSettings.json
# Change the port to 5002 or any available port
```

### Issue: Build errors with ANTLR

**Solution:**
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

---

## Next Steps

### Try Example Formulas

In the Web UI, click "Show Examples" to see all supported formula types:

1. **Math**: `(product_sales + service_sales) - operating_cost`
2. **Fixed Digits**: `product_sales * 1.15`
3. **IF Logic**: `IF(employee_count > 50, "Large", "Small")`
4. **Aggregations**: `(product_sales / SUM(product_sales)) * 100`
5. **Combined**: `IF(product_sales > AVG(product_sales), "Above Average", "Below Average")`

### Read Full Documentation

- [README.md](README.md) - Complete documentation
- [TEST_SCENARIOS.md](TEST_SCENARIOS.md) - 20 comprehensive test cases

### Explore the Code

- `ANTLR/Formula.g4` - Grammar definition
- `Visitors/FormulaToSqlVisitor.cs` - SQL converter
- `Services/FormulaService.cs` - Business logic
- `Controllers/FormulaController.cs` - API endpoints

---

## Clean Up

### Remove All Test Columns

```sql
-- Connect to database
psql -U postgres -d formulademo

-- List all columns
\d sales_data

-- Drop test columns (example)
ALTER TABLE sales_data DROP COLUMN IF EXISTS total_revenue;
ALTER TABLE sales_data DROP COLUMN IF EXISTS profit;
ALTER TABLE sales_data DROP COLUMN IF EXISTS company_size;
-- Repeat for other test columns
```

### Reset Database

```bash
# Drop and recreate
psql -U postgres -c "DROP DATABASE IF EXISTS formulademo;"
psql -U postgres -c "CREATE DATABASE formulademo;"
psql -U postgres -d formulademo -f Database/setup.sql
```

### Stop Application

Press `Ctrl+C` in the terminal where `dotnet run` is running.

---

## Success Checklist

- [ ] Database `formulademo` created
- [ ] 20 rows in `sales_data` table
- [ ] Application builds without errors
- [ ] Application runs on https://localhost:5001
- [ ] Web UI loads successfully
- [ ] Can select table and load columns
- [ ] Formula validation works
- [ ] Preview shows calculated values
- [ ] Apply creates computed column
- [ ] Swagger documentation accessible

---

## Getting Help

1. **Check Logs**: Look at the console where `dotnet run` is running
2. **Check Database**: Verify data with `psql`
3. **Test API**: Use Swagger UI at `https://localhost:5001`
4. **Review Code**: Check implementation in Visual Studio Code or your IDE

---

**🎉 Congratulations! You're ready to parse formulas and generate SQL!**

For comprehensive testing, see [TEST_SCENARIOS.md](TEST_SCENARIOS.md).
