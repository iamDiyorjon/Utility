# Type 5 Aggregation Formula Limitation

## 🚨 Important Notice

**PostgreSQL GENERATED computed columns cannot use aggregate functions** that require GROUP BY or window functions without specific syntax.

---

## ❌ What Doesn't Work

### **This will FAIL:**
```sql
ALTER TABLE sales_data
ADD COLUMN sales_percentage NUMERIC
GENERATED ALWAYS AS ((product_sales / SUM(product_sales)) * 100) STORED;
```

**Error:**
```
column "sales_data.company_id" must appear in the GROUP BY clause
or be used in an aggregate function
```

**Why?** `SUM(product_sales)` needs to aggregate across ALL rows, but GENERATED columns are evaluated per row without GROUP BY capability.

---

## ✅ Working Solutions

### **Solution 1: Use Database VIEWs** (Recommended)

Create a VIEW instead of a computed column:

```sql
CREATE OR REPLACE VIEW sales_data_with_percentages AS
SELECT
    *,
    ROUND((product_sales::NUMERIC /
           NULLIF(SUM(product_sales) OVER (), 0)) * 100, 2)
    as sales_percentage
FROM sales_data;
```

**Usage:**
```sql
SELECT company_name, product_sales, sales_percentage
FROM sales_data_with_percentages
ORDER BY sales_percentage DESC;
```

**Advantages:**
- ✅ Always up-to-date (calculated on query)
- ✅ No storage overhead
- ✅ Supports all aggregate functions
- ✅ Can use window functions (OVER clause)

**Disadvantages:**
- Calculated each time (but very fast)
- Cannot index the calculated column

---

### **Solution 2: Calculate at Application Level**

Query with calculation:

```sql
SELECT
    company_name,
    product_sales,
    (product_sales::decimal / SUM(product_sales) OVER ()) * 100
    as sales_percentage
FROM sales_data;
```

---

### **Solution 3: Materialized VIEW** (For Large Datasets)

If performance is critical:

```sql
CREATE MATERIALIZED VIEW sales_data_percentages AS
SELECT
    *,
    (product_sales::NUMERIC / SUM(product_sales) OVER ()) * 100
    as sales_percentage
FROM sales_data;

-- Refresh when data changes
REFRESH MATERIALIZED VIEW sales_data_percentages;

-- Can create index on materialized view
CREATE INDEX idx_sales_pct ON sales_data_percentages(sales_percentage);
```

---

## 📋 Formula Type Compatibility Matrix

| Type | Description | Works in GENERATED Column? | Alternative |
|------|-------------|---------------------------|-------------|
| **Type 1** | Column Creation | N/A (manual ALTER TABLE) | Direct SQL |
| **Type 2** | Math Operations | ✅ Yes | - |
| **Type 3** | Fixed Digits | ✅ Yes | - |
| **Type 4** | IF Logic | ✅ Yes | - |
| **Type 5** | Aggregations | ❌ No | Use VIEWs |

---

## 🎯 What Works in GENERATED Columns

### ✅ **Perfect for GENERATED columns:**

**Basic Math:**
```sql
(product_sales + service_sales)
(revenue - cost) / revenue * 100
price * quantity
```

**Conditional Logic:**
```sql
IF(value > 100, "High", "Low")
IF(a > b, a - b, b - a)
```

**Comparisons:**
```sql
revenue > cost
employee_count >= 50
```

**String Operations:**
```sql
UPPER(company_name)
CONCAT(first_name, ' ', last_name)
```

---

### ❌ **NOT for GENERATED columns:**

**Aggregates requiring GROUP BY:**
```sql
SUM(column)
AVG(column)
COUNT(*)
```

**Window Functions (without proper syntax):**
```sql
RANK() OVER (ORDER BY sales)
ROW_NUMBER() OVER ()
LAG(price) OVER (ORDER BY date)
```

**Subqueries:**
```sql
(SELECT AVG(price) FROM products)
```

---

## 🔧 Our Application Approach

### **Current Implementation:**
- Types 1-4: Create GENERATED computed columns ✅
- Type 5: **Needs enhancement** to create VIEWs instead

### **Recommended Enhancement:**

1. **Detect aggregation functions** in formula
2. **If aggregates detected:**
   - Create VIEW instead of computed column
   - Notify user in preview
   - Show VIEW query in generated SQL
3. **If no aggregates:**
   - Create GENERATED column as normal

---

## 📝 Example User Workflow

### **Scenario: User wants sales percentage**

**User Input:**
- Formula: `(product_sales / SUM(product_sales)) * 100`
- Column: `sales_percentage`

**System Response:**
```
⚠️ Notice: This formula contains aggregation functions.
   Creating a VIEW instead of a computed column.

Generated SQL:
CREATE OR REPLACE VIEW sales_data_extended AS
SELECT
  *,
  (product_sales::NUMERIC / SUM(product_sales) OVER ()) * 100
  as sales_percentage
FROM sales_data;

To query: SELECT * FROM sales_data_extended;
```

---

## 📊 Current Workaround

**For now, users can:**

1. **Use pre-created VIEWs:**
   - `sales_data_with_percentages`
   - `sales_data_with_performance`
   - `sales_data_regional_analysis`

2. **Query with calculations:**
   ```sql
   SELECT
     *,
     (product_sales / SUM(product_sales) OVER ()) * 100
   FROM sales_data;
   ```

3. **Use only Types 1-4 formulas** in the UI

---

## 🚀 Future Enhancement Plan

1. **Detect aggregate functions** in parser
2. **Add VIEW creation** to FormulaService
3. **Update UI** to show VIEW vs COLUMN creation
4. **Add VIEW management** endpoints (list, drop views)
5. **Support materialized views** for performance

---

## 📖 References

- [PostgreSQL Generated Columns](https://www.postgresql.org/docs/current/ddl-generated-columns.html)
- [PostgreSQL Window Functions](https://www.postgresql.org/docs/current/tutorial-window.html)
- [PostgreSQL Views](https://www.postgresql.org/docs/current/sql-createview.html)

---

**Last Updated:** October 2024
**Applies to:** Formula Parser Demo v1
