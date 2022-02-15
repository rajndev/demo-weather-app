using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.Common.Models
{
    public class ApiResult<T>
    {
        public int StatusCode { get; set; }
        public T Content { get; set; }
        public string Error { get; set; }
        public bool IsSuccessStatusCode { get; set; }

        public ApiResult(T content)
        {
            Content = content;
        }


    }
}
