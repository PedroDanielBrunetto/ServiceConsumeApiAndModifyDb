using ServiceConsumePUTabx.Context;
using ServiceConsumePUTabx.Models;
using ServiceConsumePUTabx.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

public class DatabaseUpdater
{
    private readonly ApiService _apiService;
    private readonly PUComexTabxDontDDDContext _context;

    public DatabaseUpdater(ApiService apiService, PUComexTabxDontDDDContext context)
    {
        _apiService = apiService;
        _context = context;
    }

    public async Task UpdateDatabaseAsync()
    {
        var tables = await _apiService.GetTablesAsync();

        foreach (var table in tables)
        {
            var metadata = await _apiService.GetTableMetadataAsync(table.Nome);
            await EnsureTableExistsAsync(metadata);
        }
    }

    private async Task EnsureTableExistsAsync(TableMetadata metadata)
    {
        var tableName = metadata.Nome;
        var tableExists = await _context.Database.SqlQuery<int>($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'").SingleAsync() > 0;

        if (!tableExists)
            await CreateTableAsync(metadata);
        else
            await UpdateTableAsync(metadata);
    }

    private async Task CreateTableAsync(TableMetadata metadata)
    {
        var columns = metadata.Campos.Select(c => $"{c.Nome} {GetSqlType(c)} {(c.Obrigatorio ? "NOT NULL" : "NULL")}");
        var createTableSql = $"CREATE TABLE {metadata.Nome} ({string.Join(", ", columns)})";
        await _context.Database.ExecuteSqlCommandAsync(createTableSql);
    }

    private async Task UpdateTableAsync(TableMetadata metadata)
    {
        foreach (var column in metadata.Campos)
        {
            var columnExists = await _context.Database.SqlQuery<int>($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{metadata.Nome}' AND COLUMN_NAME = '{column.Nome}'").SingleAsync() > 0;

            if (!columnExists)
            {
                var addColumnSql = $"ALTER TABLE {metadata.Nome} ADD {column.Nome} {GetSqlType(column)}";
                await _context.Database.ExecuteSqlCommandAsync(addColumnSql);
            }
            else
            {
                var columnInfo = await _context.Database.SqlQuery<ColumnInfo>(
                    $"SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{metadata.Nome}' AND COLUMN_NAME = '{column.Nome}'").SingleAsync();

                bool needsUpdate = false;

                // Comparar tipo de dados
                if (!IsDataTypeEqual(columnInfo, column))
                    needsUpdate = true;

                // Comparar nullability
                if (columnInfo.IS_NULLABLE == "YES" && column.Obrigatorio ||
                    columnInfo.IS_NULLABLE == "NO" && !column.Obrigatorio)
                    needsUpdate = true;

                // Comparar tamanho
                if (columnInfo.DATA_TYPE == "VARCHAR" && columnInfo.CHARACTER_MAXIMUM_LENGTH != column.Tamanho)
                    needsUpdate = true;

                if (needsUpdate)
                {
                    var alterColumnSql = $"ALTER TABLE {metadata.Nome} ALTER COLUMN {column.Nome} {GetSqlType(column)} {(column.Obrigatorio ? "NOT NULL" : "NULL")}";
                    await _context.Database.ExecuteSqlCommandAsync(alterColumnSql);
                }
            }
        }
    }

    private bool IsDataTypeEqual(ColumnInfo columnInfo, Column column)
    {
        switch (column.Tipo)
        {
            case "STRING":
                return columnInfo.DATA_TYPE == "VARCHAR";
            case "INTEIRO":
                return columnInfo.DATA_TYPE == "INT";
            case "DATA_HORA":
                return columnInfo.DATA_TYPE == "DATETIME";
            case "DECIMAL":
                return columnInfo.DATA_TYPE == "DECIMAL";
            case "BOOLEAN":
                return columnInfo.DATA_TYPE == "BIT";
            case "FLOAT":
                return columnInfo.DATA_TYPE == "FLOAT";
            case "DOUBLE":
                return columnInfo.DATA_TYPE == "DOUBLE";
            case "TEXTO":
                return columnInfo.DATA_TYPE == "TEXT";
            case "CHAR":
                return columnInfo.DATA_TYPE == "CHAR";
            case "BLOB":
                return columnInfo.DATA_TYPE == "VARBINARY";
            default:
                throw new InvalidOperationException($"Unsupported column type: {column.Tipo}");
        }
    }

    private string GetSqlType(Column column)
    {
        switch (column.Tipo)
        {
            case "STRING":
                return $"VARCHAR({column.Tamanho})";
            case "INTEIRO":
                return "INT";
            case "DATA_HORA":
                return "DATETIME";
            case "DECIMAL":
                return column.CasasDecimais.HasValue ? $"DECIMAL({column.Tamanho}, {column.CasasDecimais})" : $"DECIMAL({column.Tamanho})";
            case "BOOLEAN":
                return "BIT";
            case "FLOAT":
                return "FLOAT";
            case "DOUBLE":
                return "DOUBLE";
            case "TEXTO":
                return "TEXT";
            case "CHAR":
                return $"CHAR({column.Tamanho})";
            case "BLOB":
                return "VARBINARY(MAX)";
            default:
                throw new InvalidOperationException($"Unsupported column type: {column.Tipo}");
        }
    }
}
