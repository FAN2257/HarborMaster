using HarborMaster.Models;
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice, int>
    {
        public async Task<Invoice?> GetByDockingRequestId(int dockingRequestId)
        {
            var response = await _client.From<Invoice>()
                                        .Filter("docking_request_id", Postgrest.Constants.Operator.Equals, dockingRequestId)
                                        .Single();
            return response;
        }
    }
}