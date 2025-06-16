using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Inventario_Productos_Tecnologicos;
    
    /// <summary>
    /// Represents a paginated list of items of type T that inherits from List{T}.
    /// Provides pagination functionality for collections.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list</typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// Gets the total number of items across all pages
        /// </summary>
        public int TotalCount { get; private set; }
    
        /// <summary>
        /// Gets the current page index (zero-based)
        /// </summary>
        public int PageIndex { get; private set; }
    
        /// <summary>
        /// Gets the size of each page
        /// </summary>
        public int PageSize { get; private set; }
    
        /// <summary>
        /// Initializes a new instance of PaginatedList{T}
        /// </summary>
        /// <param name="items">The items for the current page</param>
        /// <param name="count">The total number of items across all pages</param>
        /// <param name="pageIndex">The current page index (zero-based)</param>
        /// <param name="pageSize">The number of items per page</param>
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
            this.AddRange(items);
        }
    
        /// <summary>
        /// Gets a value indicating whether there is a previous page available
        /// </summary>
        public bool HasPreviousPage => (PageIndex > 0);
    
        /// <summary>
        /// Gets a value indicating whether there is a next page available
        /// </summary>
        public bool HasNextPage => (PageIndex + 1) * PageSize < TotalCount;
    
        /// <summary>
        /// Creates a new instance of PaginatedList{T} from a queryable source
        /// </summary>
        /// <param name="source">The IQueryable source</param>
        /// <param name="pageIndex">The page index to retrieve (zero-based)</param>
        /// <param name="pageSize">The number of items per page</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list.</returns>
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }