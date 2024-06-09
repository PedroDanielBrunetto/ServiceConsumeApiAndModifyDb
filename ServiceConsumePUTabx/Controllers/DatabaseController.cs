using ServiceConsumePUTabx.Context;
using ServiceConsumePUTabx.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ServiceConsumePUTabx.Controllers
{
    [RoutePrefix("api/database")]
    public class DatabaseController : ApiController
    {
        private readonly DatabaseUpdater _updater;

        public DatabaseController()
        {
            var apiService = new ApiService();
            var context = new PUComexTabxDontDDDContext();
            _updater = new DatabaseUpdater(apiService, context);
        }

        [HttpPost]
        [Route("update")]
        public async Task<IHttpActionResult> UpdateDatabase()
        {
            try
            {
                await _updater.UpdateDatabaseAsync();
                return Ok("Database update complete.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

}