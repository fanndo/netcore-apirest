﻿namespace ApiRest.Dto
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
