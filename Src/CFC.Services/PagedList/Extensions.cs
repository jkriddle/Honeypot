﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFC.Services.PagedList
{

    /// <summary>
    /// Extension to turn an IQueryable into a PagedList object
    /// </summary>
    public static class Pagination
    {
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index, int pageSize)
        {
            return new PagedList<T>(source, index, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int index)
        {
            return new PagedList<T>(source, index, 10);
        }
    }

}