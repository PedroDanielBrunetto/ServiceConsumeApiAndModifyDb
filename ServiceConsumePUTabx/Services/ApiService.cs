using Newtonsoft.Json;
using ServiceConsumePUTabx.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace ServiceConsumePUTabx.Services
{
    public class ApiService
    {

        private readonly HttpClient _client;

        public ApiService()
        {
            _client = new HttpClient();
        }

        public async Task<List<Table>> GetTablesAsync()
        {
            var response = await _client.GetStringAsync("https://fake-json-gamma.vercel.app/tables-example");
            return JsonConvert.DeserializeObject<List<Table>>(response);
        }

        public async Task<TableMetadata> GetTableMetadataAsync(string tableName)
        {
            var response = await _client.GetStringAsync($"https://fake-json-gamma.vercel.app/{tableName.ToLower()}-example");
            return JsonConvert.DeserializeObject<TableMetadata>(response);
        }
    }
}