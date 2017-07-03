﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Riganti.Utils.Infrastructure.Core;

namespace Riganti.Utils.Infrastructure.EntityFramework
{
    /// <summary>
    /// A base implementation of query object in Entity Framework.
    /// </summary>
    public abstract class EntityFrameworkQuery<TResult> : EntityFrameworkQuery<TResult, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkQuery{TResult}"/> class.
        /// </summary>
        protected EntityFrameworkQuery(IUnitOfWorkProvider provider) : base(provider)
        {
        }

        /// <summary>
        ///     When overriden in derived class, it allows to modify the materialized results of the query before they are returned
        ///     to the caller.
        /// </summary>
        protected override IList<TResult> PostProcessResults(IList<TResult> results)
        {
            return results;
        }
    }

    /// <summary>
    /// A base implementation of query object in Entity Framework.
    /// </summary>
    public abstract class EntityFrameworkQuery<TQueryableResult, TResult> : EntityFrameworkQuery<TQueryableResult, TResult, DbContext>
    {
        public EntityFrameworkQuery(IUnitOfWorkProvider provider) : base(provider)
        {
        }
    }


    /// <summary>
    /// A base implementation of query object in Entity Framework.
    /// </summary>
    public abstract class EntityFrameworkQuery<TQueryableResult, TResult, TDbContext> : QueryBase<TQueryableResult, TResult>
        where TDbContext : DbContext
    {
        private readonly IEntityFrameworkUnitOfWorkProvider<TDbContext> provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityFrameworkQuery{TQueryableResult, TResult}"/> class.
        /// </summary>
        protected EntityFrameworkQuery(IEntityFrameworkUnitOfWorkProvider<TDbContext> provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Gets the <see cref="DbContext"/>.
        /// </summary>
        protected virtual TDbContext Context
        {
            get
            {
                var context = EntityFrameworkUnitOfWork.TryGetDbContext<TDbContext>(provider);
                if (context == null)
                {
                    throw new InvalidOperationException("The EntityFrameworkQuery must be used in a unit of work of type EntityFrameworkUnitOfWork!");
                }
                return context;
            }
        }

        protected override async Task<IList<TQueryableResult>> ExecuteQueryAsync(IQueryable<TQueryableResult> query, CancellationToken cancellationToken)
        {
            return await query.ToListAsync(cancellationToken);
        }

        public override async Task<int> GetTotalRowCountAsync(CancellationToken cancellationToken)
        {
            return await GetQueryable().CountAsync(cancellationToken);
        }
    }
}
