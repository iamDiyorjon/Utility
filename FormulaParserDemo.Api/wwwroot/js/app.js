// API Base URL
const API_BASE = '/api/formula';

// Global state
let currentPreviewData = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', async () => {
    await loadTables();
});

// Load available tables
async function loadTables() {
    try {
        const response = await fetch(`${API_BASE}/tables`);
        const tables = await response.json();

        const select = document.getElementById('tableSelect');
        tables.forEach(table => {
            const option = document.createElement('option');
            option.value = table;
            option.textContent = table;
            select.appendChild(option);
        });
    } catch (error) {
        showError('Failed to load tables: ' + error.message);
    }
}

// Load columns for selected table
async function loadColumns() {
    const tableName = document.getElementById('tableSelect').value;
    if (!tableName) {
        alert('Please select a table first');
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/tables/${tableName}/columns`);
        const columns = await response.json();

        const columnsDisplay = document.getElementById('columnsDisplay');
        const columnsList = document.getElementById('columnsList');

        columnsList.innerHTML = columns.map(col =>
            `<span class="column-tag" onclick="insertColumn('${col}')">${col}</span>`
        ).join('');

        columnsDisplay.style.display = 'block';
    } catch (error) {
        showError('Failed to load columns: ' + error.message);
    }
}

// Insert column into formula
function insertColumn(columnName) {
    const formulaInput = document.getElementById('formulaInput');
    const cursorPos = formulaInput.selectionStart;
    const textBefore = formulaInput.value.substring(0, cursorPos);
    const textAfter = formulaInput.value.substring(cursorPos);

    formulaInput.value = textBefore + columnName + textAfter;
    formulaInput.focus();
    formulaInput.setSelectionRange(cursorPos + columnName.length, cursorPos + columnName.length);
}

// Validate formula
async function validateFormula() {
    const tableName = document.getElementById('tableSelect').value;
    const formula = document.getElementById('formulaInput').value;

    if (!tableName || !formula) {
        alert('Please select a table and enter a formula');
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/validate`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ tableName, formula })
        });

        const result = await response.json();
        displayValidationResults(result);
    } catch (error) {
        showError('Validation failed: ' + error.message);
    }
}

// Display validation results
function displayValidationResults(result) {
    const section = document.getElementById('validationSection');
    const resultsDiv = document.getElementById('validationResults');

    let html = '';

    if (result.isValid) {
        html += '<div class="alert alert-success">✓ Formula is valid!</div>';
    } else {
        html += '<div class="alert alert-error">✗ Formula has errors:</div>';
        result.errors.forEach(error => {
            html += `<div class="alert alert-error">${error}</div>`;
        });
    }

    if (result.warnings && result.warnings.length > 0) {
        result.warnings.forEach(warning => {
            html += `<div class="alert alert-warning">⚠️ ${warning}</div>`;
        });
    }

    if (result.referencedColumns && result.referencedColumns.length > 0) {
        html += `<div class="alert alert-info">
            <strong>Referenced Columns:</strong> ${result.referencedColumns.join(', ')}
        </div>`;
    }

    if (result.detectedDataType) {
        html += `<div class="alert alert-info">
            <strong>Detected Data Type:</strong> ${result.detectedDataType}
        </div>`;
    }

    resultsDiv.innerHTML = html;
    section.style.display = 'block';
}

// Preview formula
async function previewFormula() {
    const tableName = document.getElementById('tableSelect').value;
    const columnName = document.getElementById('columnName').value;
    const formula = document.getElementById('formulaInput').value;

    if (!tableName || !columnName || !formula) {
        alert('Please fill in all required fields');
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/preview`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                tableName,
                columnName,
                formula,
                previewRows: 5
            })
        });

        const result = await response.json();
        currentPreviewData = result;
        displayPreviewResults(result);
    } catch (error) {
        showError('Preview failed: ' + error.message);
    }
}

// Display preview results
function displayPreviewResults(result) {
    const previewSection = document.getElementById('previewSection');
    const statsDiv = document.getElementById('previewStats');
    const tableDiv = document.getElementById('previewTable');
    const sqlDiv = document.getElementById('previewSql');

    if (!result.success) {
        let errorHtml = '<div class="alert alert-error">Preview failed:</div>';
        result.errors.forEach(error => {
            errorHtml += `<div class="alert alert-error">${error}</div>`;
        });
        previewSection.innerHTML = errorHtml;
        previewSection.style.display = 'block';
        return;
    }

    // Display statistics
    statsDiv.innerHTML = `
        <div class="stat-card">
            <div class="stat-value">${result.totalRowsAffected}</div>
            <div class="stat-label">Total Rows</div>
        </div>
        <div class="stat-card">
            <div class="stat-value">${result.successfulCalculations}</div>
            <div class="stat-label">Successful</div>
        </div>
        <div class="stat-card">
            <div class="stat-value">${result.failedCalculations}</div>
            <div class="stat-label">Failed</div>
        </div>
        <div class="stat-card">
            <div class="stat-value">${result.detectedDataType}</div>
            <div class="stat-label">Data Type</div>
        </div>
    `;

    // Display preview data table
    if (result.previewData && result.previewData.length > 0) {
        const columns = Object.keys(result.previewData[0]);
        let tableHtml = '<table><thead><tr>';

        columns.forEach(col => {
            tableHtml += `<th>${col}</th>`;
        });
        tableHtml += '</tr></thead><tbody>';

        result.previewData.forEach(row => {
            tableHtml += '<tr>';
            columns.forEach(col => {
                const value = row[col];
                const displayValue = value !== null && value !== undefined ? value : 'NULL';
                tableHtml += `<td>${displayValue}</td>`;
            });
            tableHtml += '</tr>';
        });

        tableHtml += '</tbody></table>';
        tableDiv.innerHTML = tableHtml;
    }

    // Display generated SQL
    sqlDiv.innerHTML = `
        <h4>Generated SQL Expression:</h4>
        <pre>${result.generatedSql}</pre>
    `;

    previewSection.style.display = 'block';
    document.getElementById('confirmSection').style.display = 'block';
}

// Apply formula
async function applyFormula() {
    if (!confirm('Are you sure you want to create this computed column? This action cannot be undone through this interface.')) {
        return;
    }

    const tableName = document.getElementById('tableSelect').value;
    const columnName = document.getElementById('columnName').value;
    const formula = document.getElementById('formulaInput').value;
    const dataType = document.getElementById('dataType').value;

    try {
        const response = await fetch(`${API_BASE}/apply`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                tableName,
                columnName,
                formula,
                dataType: dataType || null,
                useStoredGenerated: true
            })
        });

        const result = await response.json();
        displayApplyResults(result);
    } catch (error) {
        showError('Apply failed: ' + error.message);
    }
}

// Display apply results
function displayApplyResults(result) {
    const applyResults = document.getElementById('applyResults');

    let html = '';

    if (result.success) {
        html = `
            <div class="alert alert-success">
                <strong>✓ Success!</strong> ${result.message}
            </div>
            <div class="alert alert-info">
                <strong>Rows Affected:</strong> ${result.rowsAffected}
            </div>
            <div class="sql-display">
                <h4>Executed SQL:</h4>
                <pre>${result.executedSql}</pre>
            </div>
        `;

        // Clear form after successful application
        setTimeout(() => {
            if (confirm('Column created successfully! Do you want to reload the page to create another formula?')) {
                location.reload();
            }
        }, 1000);
    } else {
        html = '<div class="alert alert-error">✗ Failed to apply formula:</div>';
        result.errors.forEach(error => {
            html += `<div class="alert alert-error">${error}</div>`;
        });
    }

    applyResults.innerHTML = html;
}

// Cancel apply
function cancelApply() {
    document.getElementById('confirmSection').style.display = 'none';
    currentPreviewData = null;
}

// Load and display examples
async function loadExamples() {
    try {
        const response = await fetch(`${API_BASE}/examples`);
        const examples = await response.json();

        const modal = document.getElementById('examplesModal');
        const content = document.getElementById('examplesContent');

        let html = '';
        for (const [category, formulas] of Object.entries(examples)) {
            html += `<div class="example-group">
                <h3>${category}</h3>`;
            formulas.forEach(formula => {
                html += `<div class="example-item" onclick="useExample('${escapeHtml(formula)}')">
                    ${formula}
                </div>`;
            });
            html += '</div>';
        }

        content.innerHTML = html;
        modal.style.display = 'flex';
    } catch (error) {
        showError('Failed to load examples: ' + error.message);
    }
}

// Use example formula
function useExample(formula) {
    document.getElementById('formulaInput').value = formula;
    closeExamples();
}

// Close examples modal
function closeExamples() {
    document.getElementById('examplesModal').style.display = 'none';
}

// Helper: Show error message
function showError(message) {
    alert('Error: ' + message);
    console.error(message);
}

// Helper: Escape HTML
function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
}
