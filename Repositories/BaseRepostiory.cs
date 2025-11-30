using HarborMaster.Data;
using Postgrest.Models;
using Supabase;
using System.Collections.Generic;
using System.Linq; // Dibutuhkan untuk .FirstOrDefault()
using System.Threading.Tasks;

namespace HarborMaster.Repositories
{
    public class BaseRepository<T, K> where T : BaseModel, new()
    {
        protected readonly Client _client = SupabaseManager.Client;

        public virtual async Task<List<T>> GetAllAsync()
        {
            var response = await _client.From<T>().Get();
            return response.Models; 
        }

        public async Task<T> GetByIdAsync(K id)
        {

            var response = await _client.From<T>()
                                        .Filter("id", Postgrest.Constants.Operator.Equals, id)
                                        .Single();

            return response;
        }

        public async Task<T> InsertAsync(T model)
        {

            var response = await _client.From<T>().Insert(model);


            return response.Models.FirstOrDefault();
        }

        public async Task<T> UpdateAsync(T model)
        {

            var response = await _client.From<T>().Update(model);

            return response.Models.FirstOrDefault(); 
        }

        public async Task DeleteAsync(T model)
        {
            await _client.From<T>().Delete(model); 
        }
    }
}