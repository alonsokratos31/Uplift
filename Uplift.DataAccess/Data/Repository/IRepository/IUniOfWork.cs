using System;
using System.Collections.Generic;
using System.Text;
using Uplift.DataAccess.Data.Repository.IRepository;

namespace Uplift.DataAccess.Data.Repository
{
    public interface IUniOfWork : IDisposable
    {
        ICategoryRepository Category { get; }

        IFrequencyRepository Frequency { get; }

        IServiceRepository Service { get; }

        IOrderDetailsRepository OrderDetails { get; }

        IOrderHeaderRepository OrderHeader { get; }

        IUserRepository User { get; }

        void Save();
    }
}
