using System.Collections.Generic;

namespace Unity1week202508.Data
{
    public class PrizeMasterDataRepository
    {
        private readonly PrizeMasterDataSource _data;

        public PrizeMasterDataRepository(PrizeMasterDataSource data)
        {
            _data = data;
        }

        public PrizeMasterData Get(int id) => _data.Get(id);

        public IReadOnlyList<PrizeMasterData> GetAll() => _data.Data;
    }
}