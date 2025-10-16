# Formula Parser Test Scenarios

This document provides comprehensive test cases for all 5 formula types supported by the application.

## Prerequisites

1. Database `formulademo` is set up with the `sales_data` table
2. Application is running on `https://localhost:5001`
3. You have 20 sample companies in the database

---

## Test Type 1: Column Creation (Empty Column)

This type creates columns with NULL values - handled at the application level, not through formulas.

**Manual Test in PostgreSQL:**

```sql
-- Create an empty column for notes
ALTER TABLE sales_data ADD COLUMN notes TEXT DEFAULT NULL;

-- Verify
SELECT company_name, notes FROM sales_data LIMIT 5;

-- Cleanup
ALTER TABLE sales_data DROP COLUMN notes;
```

---

## Test Type 2: Inter-Column Mathematical Operations

### Test 2.1: Simple Addition

**Formula:** `(product_sales + service_sales)`

**Expected:**
- Column Name: `total_revenue`
- Data Type: NUMERIC
- Sample: Company 1 should have 230000.00 (150000 + 80000)

**Steps:**
1. Web UI: Select `sales_data` table
2. Column Name: `total_revenue`
3. Formula: `(product_sales + service_sales)`
4. Click Validate → Should show "Valid"
5. Click Preview → Check calculated values
6. Click Apply → Creates column

**Verification:**
```sql
SELECT company_name, product_sales, service_sales, total_revenue
FROM sales_data
WHERE company_id = 1;
-- Expected: 150000 + 80000 = 230000
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN total_revenue;
```

---

### Test 2.2: Complex Calculation (Profit)

**Formula:** `(product_sales + service_sales) - operating_cost`

**Expected:**
- Column Name: `profit`
- Data Type: NUMERIC
- Sample: Company 1 should have 110000.00 (230000 - 120000)

**Steps:**
1. Column Name: `profit`
2. Formula: `(product_sales + service_sales) - operating_cost`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name, product_sales, service_sales, operating_cost, profit
FROM sales_data
WHERE company_id = 1;
-- Expected: 150000 + 80000 - 120000 = 110000
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN profit;
```

---

### Test 2.3: Division (Per-Employee Revenue)

**Formula:** `(product_sales + service_sales) / employee_count`

**Expected:**
- Column Name: `revenue_per_employee`
- Data Type: NUMERIC
- Sample: Company 1 should have ~5111.11 (230000 / 45)

**Steps:**
1. Column Name: `revenue_per_employee`
2. Formula: `(product_sales + service_sales) / employee_count`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name, product_sales, service_sales, employee_count, revenue_per_employee
FROM sales_data
WHERE company_id = 1;
-- Expected: 230000 / 45 = 5111.11
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN revenue_per_employee;
```

---

## Test Type 3: Fixed Digit Operations

### Test 3.1: Percentage Increase (15% raise)

**Formula:** `product_sales * 1.15`

**Expected:**
- Column Name: `projected_product_sales`
- Data Type: NUMERIC
- Sample: Company 1 should have 172500.00 (150000 * 1.15)

**Steps:**
1. Column Name: `projected_product_sales`
2. Formula: `product_sales * 1.15`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name, product_sales, projected_product_sales
FROM sales_data
WHERE company_id = 1;
-- Expected: 150000 * 1.15 = 172500
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN projected_product_sales;
```

---

### Test 3.2: Monthly Average

**Formula:** `(product_sales + service_sales) / 12`

**Expected:**
- Column Name: `monthly_avg_revenue`
- Data Type: NUMERIC
- Sample: Company 1 should have ~19166.67 (230000 / 12)

**Steps:**
1. Column Name: `monthly_avg_revenue`
2. Formula: `(product_sales + service_sales) / 12`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name, product_sales, service_sales, monthly_avg_revenue
FROM sales_data
WHERE company_id = 1;
-- Expected: 230000 / 12 = 19166.67
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN monthly_avg_revenue;
```

---

### Test 3.3: Discount (15% off)

**Formula:** `operating_cost * 0.85`

**Expected:**
- Column Name: `discounted_cost`
- Data Type: NUMERIC
- Sample: Company 1 should have 102000.00 (120000 * 0.85)

**Steps:**
1. Column Name: `discounted_cost`
2. Formula: `operating_cost * 0.85`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name, operating_cost, discounted_cost
FROM sales_data
WHERE company_id = 1;
-- Expected: 120000 * 0.85 = 102000
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN discounted_cost;
```

---

## Test Type 4: IF Logic (Conditional Expressions)

### Test 4.1: Company Size Classification

**Formula:** `IF(employee_count > 50, "Large", "Small")`

**Expected:**
- Column Name: `company_size`
- Data Type: TEXT
- Company 1 (45 employees) → "Small"
- Company 3 (75 employees) → "Large"

**Steps:**
1. Column Name: `company_size`
2. Formula: `IF(employee_count > 50, "Large", "Small")`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name, employee_count, company_size
FROM sales_data
WHERE company_id IN (1, 3);
-- Expected:
-- Company 1 (45) = Small
-- Company 3 (75) = Large
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN company_size;
```

---

### Test 4.2: Revenue Classification

**Formula:** `IF((product_sales + service_sales) > 100000, "High Revenue", "Standard")`

**Expected:**
- Column Name: `revenue_category`
- Data Type: TEXT
- Company 1 (230000) → "High Revenue"
- Company 4 (110000) → "High Revenue"

**Steps:**
1. Column Name: `revenue_category`
2. Formula: `IF((product_sales + service_sales) > 100000, "High Revenue", "Standard")`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name,
       (product_sales + service_sales) as total_revenue,
       revenue_category
FROM sales_data
ORDER BY company_id
LIMIT 5;
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN revenue_category;
```

---

### Test 4.3: Profitability Check

**Formula:** `IF(operating_cost > (product_sales + service_sales), "Loss", "Profit")`

**Expected:**
- Column Name: `profitability`
- Data Type: TEXT
- All sample companies should show "Profit" (they all have positive margins)

**Steps:**
1. Column Name: `profitability`
2. Formula: `IF(operating_cost > (product_sales + service_sales), "Loss", "Profit")`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name,
       product_sales,
       service_sales,
       operating_cost,
       profitability
FROM sales_data
LIMIT 5;
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN profitability;
```

---

## Test Type 5: Aggregations

### Test 5.1: Percentage of Total Sales

**Formula:** `(product_sales / SUM(product_sales)) * 100`

**Expected:**
- Column Name: `sales_percentage`
- Data Type: NUMERIC
- All percentages should sum to 100%

**Steps:**
1. Column Name: `sales_percentage`
2. Formula: `(product_sales / SUM(product_sales)) * 100`
3. Validate → Should show warning about aggregation
4. Preview → Apply

**Verification:**
```sql
SELECT company_name, product_sales, sales_percentage
FROM sales_data
ORDER BY sales_percentage DESC
LIMIT 5;

-- Verify sum equals 100
SELECT SUM(sales_percentage) FROM sales_data;
-- Expected: ~100.00
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN sales_percentage;
```

---

### Test 5.2: Deviation from Average

**Formula:** `product_sales - AVG(product_sales)`

**Expected:**
- Column Name: `sales_deviation`
- Data Type: NUMERIC
- Positive values = above average
- Negative values = below average

**Steps:**
1. Column Name: `sales_deviation`
2. Formula: `product_sales - AVG(product_sales)`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name,
       product_sales,
       sales_deviation,
       CASE WHEN sales_deviation > 0 THEN 'Above Average' ELSE 'Below Average' END as performance
FROM sales_data
ORDER BY sales_deviation DESC
LIMIT 10;
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN sales_deviation;
```

---

### Test 5.3: Above/Below Average Classification

**Formula:** `IF(product_sales > AVG(product_sales), "Above Average", "Below Average")`

**Expected:**
- Column Name: `performance_category`
- Data Type: TEXT
- ~50% should be "Above Average", ~50% "Below Average"

**Steps:**
1. Column Name: `performance_category`
2. Formula: `IF(product_sales > AVG(product_sales), "Above Average", "Below Average")`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT performance_category, COUNT(*) as count
FROM sales_data
GROUP BY performance_category;
-- Expected: roughly equal distribution

SELECT company_name, product_sales, performance_category
FROM sales_data
ORDER BY product_sales DESC;
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN performance_category;
```

---

## Test Complex Combined Formulas

### Test 6.1: High Performer with Aggregation and IF

**Formula:** `IF((product_sales + service_sales) > AVG(product_sales + service_sales), "High Performer", "Standard Performer")`

**Expected:**
- Column Name: `overall_performance`
- Data Type: TEXT
- Classification based on total revenue vs average

**Steps:**
1. Column Name: `overall_performance`
2. Formula: `IF((product_sales + service_sales) > AVG(product_sales + service_sales), "High Performer", "Standard Performer")`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name,
       (product_sales + service_sales) as total_revenue,
       overall_performance
FROM sales_data
ORDER BY total_revenue DESC;
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN overall_performance;
```

---

### Test 6.2: Contribution Percentage

**Formula:** `((product_sales + service_sales - operating_cost) / SUM(product_sales + service_sales - operating_cost)) * 100`

**Expected:**
- Column Name: `profit_contribution_pct`
- Data Type: NUMERIC
- Shows each company's contribution to total profit

**Steps:**
1. Column Name: `profit_contribution_pct`
2. Formula: `((product_sales + service_sales - operating_cost) / SUM(product_sales + service_sales - operating_cost)) * 100`
3. Validate → Preview → Apply

**Verification:**
```sql
SELECT company_name,
       (product_sales + service_sales - operating_cost) as profit,
       profit_contribution_pct
FROM sales_data
ORDER BY profit_contribution_pct DESC
LIMIT 10;

-- Verify sum equals 100
SELECT SUM(profit_contribution_pct) FROM sales_data;
-- Expected: ~100.00
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN profit_contribution_pct;
```

---

## Test Formula Reusability

### Test 7.1: Use Previously Created Column

**Step 1:** Create base column
```
Column: total_revenue
Formula: (product_sales + service_sales)
```

**Step 2:** Create dependent column
```
Column: profit_margin
Formula: (total_revenue - operating_cost) / total_revenue * 100
```

**Expected:** Second formula should recognize `total_revenue` as a valid column

**Verification:**
```sql
SELECT company_name,
       product_sales,
       service_sales,
       total_revenue,
       profit_margin
FROM sales_data
LIMIT 5;
```

**Cleanup:**
```sql
ALTER TABLE sales_data DROP COLUMN profit_margin;
ALTER TABLE sales_data DROP COLUMN total_revenue;
```

---

## Error Handling Tests

### Test 8.1: Invalid Column Reference

**Formula:** `(nonexistent_column + product_sales)`

**Expected:** Validation should fail with error: "Column 'nonexistent_column' does not exist"

---

### Test 8.2: Syntax Error

**Formula:** `(product_sales + )`

**Expected:** Validation should fail with syntax error

---

### Test 8.3: Type Mismatch

**Formula:** `product_sales + "text"`

**Expected:** Should parse but may fail during execution

---

### Test 8.4: Division by Zero

Create test data with zero employee_count first:

```sql
INSERT INTO sales_data (company_name, product_sales, service_sales, operating_cost, employee_count, region)
VALUES ('Test Company', 100000, 50000, 80000, 0, 'Test');
```

**Formula:** `(product_sales + service_sales) / employee_count`

**Expected:** Preview should show NULL for the test company and warning about division by zero

**Cleanup:**
```sql
DELETE FROM sales_data WHERE company_name = 'Test Company';
```

---

## Performance Tests

### Test 9.1: Large Dataset Performance

1. Insert 1000 additional rows
2. Create formula with aggregation
3. Measure preview and apply time

```sql
-- Insert test data
INSERT INTO sales_data (company_name, product_sales, service_sales, operating_cost, employee_count, region)
SELECT
    'Test Company ' || generate_series,
    RANDOM() * 200000 + 50000,
    RANDOM() * 100000 + 20000,
    RANDOM() * 150000 + 40000,
    (RANDOM() * 80 + 20)::INTEGER,
    CASE (RANDOM() * 3)::INTEGER
        WHEN 0 THEN 'North'
        WHEN 1 THEN 'South'
        WHEN 2 THEN 'East'
        ELSE 'West'
    END
FROM generate_series(1, 1000);
```

**Test Formula:** `(product_sales / SUM(product_sales)) * 100`

**Expected:** Should complete within reasonable time (< 5 seconds for preview)

**Cleanup:**
```sql
DELETE FROM sales_data WHERE company_name LIKE 'Test Company %';
```

---

## Automated Test Script

Create `test-all.sh` (or `test-all.ps1` for Windows):

```bash
#!/bin/bash

API_URL="https://localhost:5001/api/formula"

echo "Testing Formula Parser Demo - All 5 Types"
echo "=========================================="

# Test Type 2: Math
echo "Test 2.1: Simple Addition"
curl -X POST "$API_URL/validate" \
  -H "Content-Type: application/json" \
  -d '{"tableName":"sales_data","formula":"(product_sales + service_sales)"}' \
  | jq .

# Test Type 3: Fixed Digits
echo "Test 3.1: Percentage Increase"
curl -X POST "$API_URL/validate" \
  -H "Content-Type: application/json" \
  -d '{"tableName":"sales_data","formula":"product_sales * 1.15"}' \
  | jq .

# Test Type 4: IF Logic
echo "Test 4.1: Company Size"
curl -X POST "$API_URL/validate" \
  -H "Content-Type: application/json" \
  -d '{"tableName":"sales_data","formula":"IF(employee_count > 50, \"Large\", \"Small\")"}' \
  | jq .

# Test Type 5: Aggregations
echo "Test 5.1: Sales Percentage"
curl -X POST "$API_URL/validate" \
  -H "Content-Type: application/json" \
  -d '{"tableName":"sales_data","formula":"(product_sales / SUM(product_sales)) * 100"}' \
  | jq .

echo "All tests completed!"
```

---

## Summary Checklist

- [ ] Type 1: Column Creation (manual PostgreSQL test)
- [ ] Type 2: Inter-Column Math (3 tests)
- [ ] Type 3: Fixed Digits (3 tests)
- [ ] Type 4: IF Logic (3 tests)
- [ ] Type 5: Aggregations (3 tests)
- [ ] Combined Formulas (2 tests)
- [ ] Formula Reusability (1 test)
- [ ] Error Handling (4 tests)
- [ ] Performance Test (1 test)

**Total: 20 Test Scenarios**

---

**Note:** Remember to clean up test columns after each test to maintain a clean database state!
