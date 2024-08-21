using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Helen.Domain;
using Helen.Domain.GenericResponse;
using OfficeOpenXml;
using Microsoft.Extensions.Logging;

namespace Helen.Service.Utility
{
    public interface IUtility
    {
        Task<GenericResponse<T>> Response<T>(string url, HttpMethod method, object request) where T : class;
        Task<GenericResponse<List<T>>> ReadFileFromExcel<T>(Stream excelStream) where T : class, new();
        public string ReplaceInvalidValue(string input, string invalidValue, string replacementValue = "");
    }

    public class Utility : IUtility
    {
        private readonly IHttpService _httpService;
        private readonly ILogger<Utility> _logger;

        public Utility(IHttpService httpService, ILogger<Utility> logger)
        {
            _httpService = httpService;
            _logger = logger;
        }

        public async Task<GenericResponse<T>> Response<T>(string url, HttpMethod method, object request) where T : class
        {
            try
            {
                var genericResponse = new GenericResponse<T>();
                GenericHttpResponse<T> response = null;

                switch (method.Method.ToUpper())
                {
                    case "GET":
                        response = await _httpService.GetRequest<T>(url);
                        break;
                    case "POST":
                        response = await _httpService.PostRequest<T>(url, request);
                        break;
                    default:
                        return new GenericResponse<T>
                        {
                            ResponseCode = 405,
                            IsSuccessful = false,
                            Message = $"Method {method.Method} not supported",
                            Data = null
                        };
                }

                return new GenericResponse<T>
                {
                    ResponseCode = response.ResponseCode.Value,
                    IsSuccessful = response.IsSuccessful.HasValue,
                    Message = response?.Content ?? "No content",
                    Data = response?.ResponseObject
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Response method");
                return new GenericResponse<T>
                {
                    ResponseCode = 500,
                    IsSuccessful = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public async Task<GenericResponse<List<T>>> ReadFileFromExcel<T>(Stream excelStream) where T : class, new()
        {
            try
            {
                var list = new List<T>();

                using var package = new ExcelPackage(excelStream);
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet == null)
                {
                    _logger.LogWarning("No worksheet found in the provided Excel file.");
                    return new GenericResponse<List<T>>
                    {
                        ResponseCode = 404,
                        IsSuccessful = false,
                        Message = "No worksheet found",
                        Data = null
                    };
                }

                var rowCount = worksheet.Dimension?.Rows ?? 0;

                if (rowCount == 0)
                {
                    _logger.LogWarning("Worksheet is empty.");
                    return new GenericResponse<List<T>>
                    {
                        ResponseCode = 404,
                        IsSuccessful = false,
                        Message = "Worksheet is empty",
                        Data = null
                    };
                }

                for (var row = 2; row <= rowCount; row++)
                {
                    var item = new T();
                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    foreach (var property in properties)
                    {
                        var columnIndex = Array.IndexOf(properties, property) + 1;
                        var cellValue = worksheet.Cells[row, columnIndex].Text;

                        if (string.IsNullOrWhiteSpace(cellValue) && property.PropertyType.IsClass && property.PropertyType != typeof(string))
                        {
                            property.SetValue(item, null);
                        }
                        else
                        {
                            var convertedValue = Convert.ChangeType(cellValue, property.PropertyType);
                            property.SetValue(item, convertedValue);
                        }
                    }

                    list.Add(item);
                }

                return new GenericResponse<List<T>>
                {
                    ResponseCode = 200,
                    IsSuccessful = true,
                    Message = "File processed successfully",
                    Data = list
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while reading the Excel file.");
                return new GenericResponse<List<T>>
                {
                    ResponseCode = 500,
                    IsSuccessful = false,
                    Message = ex.Message,
                    Data = null
                };
            }
        }

        public string ReplaceInvalidValue(string input, string invalidValue, string replacementValue = "")
        {
            if (string.IsNullOrEmpty(input) || input == invalidValue)
            {
                return input; 
            }

            return input.Replace(invalidValue, replacementValue);
        }

    }
}
