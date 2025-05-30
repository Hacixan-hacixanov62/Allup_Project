using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_DataAccess.Helpers
{
    public class PaginatedList<T>:List<T>
    {
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public PaginatedList(List<T> items, int pageCount, int page)
        {
            this.AddRange(items);
            PageCount = pageCount;
            CurrentPage = page;
            HasNext = CurrentPage < pageCount;
            HasPrevious = CurrentPage > 1;
            int start = CurrentPage - 2;
            int end = CurrentPage + 2;
            if (Start <= 0)
            {
                end = end - (start - 1);
                start = 1;
            }
            if (end > PageCount)
            {
                end = PageCount;
                end = end - 4;
            }
            Start = start;
            End = end;
        }
        public static PaginatedList<T> Create(IQueryable<T> query, int take, int page)
        {
            var datas = query.Skip((page - 1) * take).Take(take).ToList();
            var pageCount = (int)Math.Ceiling((decimal)query.Count() / take);
            return new PaginatedList<T>(datas, pageCount, page);
        }
    }
}
