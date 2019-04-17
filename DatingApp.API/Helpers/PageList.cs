using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helpers
{
    public class PageList<T>: List<T>
    { // 这里是返回给客户端关于分页的数据
        public int CurrentPage { get; set; } // 当前页
        public int TotalPages { get; set; } // 总页数
        public int PageSize { get; set; } // 每页的大小

        public int TotalCount { get; set; } // 总记录数
    
    public PageList(List<T> items,int count,int pageNumber,int pageSize)
    {
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count/(double)pageSize); //向上取整
        this.AddRange(items);
    }

    public static async Task<PageList<T>> CreateAsync(IQueryable<T> source,int pageNumber,int pageSize) // 把查询到的结果分页 打包
    {
       var count = await source.CountAsync(); // 获得本次查询的总数
       var items = await source.Skip((pageNumber - 1 ) * pageSize).Take (pageSize).ToListAsync();
       return new PageList<T>(items,count,pageNumber,pageSize);
    }


    }
}