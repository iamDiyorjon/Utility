-- Create VIEWs for aggregation-based calculations
-- These work around the limitation of GENERATED columns with aggregates

-- View 1: Sales with Percentage of Total
CREATE OR REPLACE VIEW sales_data_with_percentages AS
SELECT
    company_id,
    company_name,
    product_sales,
    service_sales,
    operating_cost,
    employee_count,
    region,
    created_at,
    -- Calculate percentage of total product sales
    ROUND((product_sales::NUMERIC / NULLIF(SUM(product_sales) OVER (), 0)) * 100, 2) as product_sales_percentage,
    -- Calculate percentage of total revenue (product + service)
    ROUND(((product_sales + service_sales)::NUMERIC / NULLIF(SUM(product_sales + service_sales) OVER (), 0)) * 100, 2) as revenue_percentage,
    -- Deviation from average
    ROUND(product_sales - AVG(product_sales) OVER (), 2) as sales_deviation_from_avg,
    -- Average sales for comparison
    ROUND(AVG(product_sales) OVER (), 2) as avg_product_sales,
    -- Total revenue (could also be a computed column)
    product_sales + service_sales as total_revenue,
    -- Profit
    (product_sales + service_sales) - operating_cost as profit
FROM sales_data;

-- View 2: Sales with Performance Categories
CREATE OR REPLACE VIEW sales_data_with_performance AS
SELECT
    s.*,
    -- Above or below average classification
    CASE
        WHEN product_sales > AVG(product_sales) OVER () THEN 'Above Average'
        ELSE 'Below Average'
    END as performance_category,
    -- Quartile ranking
    NTILE(4) OVER (ORDER BY product_sales) as sales_quartile,
    -- Rank by sales
    RANK() OVER (ORDER BY product_sales DESC) as sales_rank
FROM sales_data s;

-- View 3: Regional Analysis
CREATE OR REPLACE VIEW sales_data_regional_analysis AS
SELECT
    s.*,
    -- Percentage within region
    ROUND((product_sales::NUMERIC / NULLIF(SUM(product_sales) OVER (PARTITION BY region), 0)) * 100, 2) as pct_of_regional_sales,
    -- Regional average
    ROUND(AVG(product_sales) OVER (PARTITION BY region), 2) as regional_avg_sales,
    -- Regional rank
    RANK() OVER (PARTITION BY region ORDER BY product_sales DESC) as regional_rank
FROM sales_data s;

-- Verify the views work
SELECT
    company_name,
    product_sales,
    product_sales_percentage,
    revenue_percentage
FROM sales_data_with_percentages
ORDER BY product_sales_percentage DESC
LIMIT 5;

-- Show performance categories
SELECT
    company_name,
    product_sales,
    performance_category,
    sales_quartile,
    sales_rank
FROM sales_data_with_performance
ORDER BY sales_rank
LIMIT 5;

-- Show regional analysis
SELECT
    company_name,
    region,
    product_sales,
    pct_of_regional_sales,
    regional_rank
FROM sales_data_regional_analysis
ORDER BY region, regional_rank
LIMIT 10;
