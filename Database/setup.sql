-- Formula Parser Demo Database Setup
-- PostgreSQL 12+

-- Create database (run this first if database doesn't exist)
-- CREATE DATABASE formulademo;

-- Connect to the database
-- \c formulademo

-- Drop existing tables if they exist
DROP TABLE IF EXISTS sales_data CASCADE;

-- Create sample sales_data table
CREATE TABLE sales_data (
    company_id SERIAL PRIMARY KEY,
    company_name VARCHAR(100) NOT NULL,
    product_sales NUMERIC(12, 2) NOT NULL DEFAULT 0,
    service_sales NUMERIC(12, 2) NOT NULL DEFAULT 0,
    operating_cost NUMERIC(12, 2) NOT NULL DEFAULT 0,
    employee_count INTEGER NOT NULL DEFAULT 1,
    region VARCHAR(50) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert sample data (20 rows)
INSERT INTO sales_data (company_name, product_sales, service_sales, operating_cost, employee_count, region) VALUES
('Tech Solutions Inc', 150000.00, 80000.00, 120000.00, 45, 'North'),
('Global Services Ltd', 95000.00, 45000.00, 85000.00, 28, 'South'),
('Innovation Corp', 220000.00, 110000.00, 180000.00, 75, 'East'),
('Digital Ventures', 75000.00, 35000.00, 65000.00, 22, 'West'),
('Smart Systems', 180000.00, 92000.00, 145000.00, 58, 'North'),
('Enterprise Hub', 125000.00, 68000.00, 102000.00, 38, 'South'),
('Cloud Dynamics', 195000.00, 105000.00, 162000.00, 68, 'East'),
('Data Insights', 88000.00, 42000.00, 75000.00, 25, 'West'),
('Tech Innovators', 165000.00, 88000.00, 135000.00, 52, 'North'),
('Future Systems', 112000.00, 58000.00, 92000.00, 32, 'South'),
('Quantum Solutions', 245000.00, 125000.00, 198000.00, 85, 'East'),
('Nexus Technologies', 98000.00, 48000.00, 82000.00, 29, 'West'),
('Alpha Enterprises', 172000.00, 91000.00, 142000.00, 55, 'North'),
('Beta Corporation', 135000.00, 72000.00, 110000.00, 42, 'South'),
('Gamma Industries', 208000.00, 112000.00, 172000.00, 72, 'East'),
('Delta Services', 82000.00, 38000.00, 68000.00, 24, 'West'),
('Epsilon Group', 158000.00, 85000.00, 130000.00, 50, 'North'),
('Zeta Solutions', 118000.00, 62000.00, 96000.00, 36, 'South'),
('Eta Technologies', 192000.00, 102000.00, 158000.00, 65, 'East'),
('Theta Systems', 92000.00, 46000.00, 78000.00, 27, 'West');

-- Create indexes for better performance
CREATE INDEX idx_sales_data_region ON sales_data(region);
CREATE INDEX idx_sales_data_employee_count ON sales_data(employee_count);

-- Display sample data
SELECT
    company_id,
    company_name,
    product_sales,
    service_sales,
    operating_cost,
    employee_count,
    region
FROM sales_data
ORDER BY company_id
LIMIT 10;

-- Example: Create a computed column manually (this is what our API will do)
-- ALTER TABLE sales_data
-- ADD COLUMN total_revenue NUMERIC
-- GENERATED ALWAYS AS (product_sales + service_sales) STORED;

-- Verification queries
SELECT COUNT(*) as total_companies FROM sales_data;
SELECT region, COUNT(*) as company_count, AVG(product_sales + service_sales) as avg_revenue
FROM sales_data
GROUP BY region
ORDER BY region;

-- Display table structure
SELECT
    column_name,
    data_type,
    is_nullable,
    column_default
FROM information_schema.columns
WHERE table_name = 'sales_data'
ORDER BY ordinal_position;

COMMIT;
