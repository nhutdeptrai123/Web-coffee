/*using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace manage_coffee_shop_web.Services {
    public class VnPayLibrary {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string GetRequestData(string key)
        {
            _requestData.TryGetValue(key, out var value);
            return value;
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            _responseData.TryGetValue(key, out var value);
            return value;
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            var data = new StringBuilder();
            foreach (var (key, value) in _requestData)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    data.Append(key + "=" + HttpUtility.UrlEncode(value) + "&");
                }
            }

            var queryString = data.ToString();
            var rawData = GetParamsString(_requestData);
            var secureHash = HmacSHA512(vnp_HashSecret, rawData);
            queryString = queryString.Remove(queryString.Length - 1) + "&vnp_SecureHash=" + secureHash;

            return baseUrl + "?" + queryString;
        }

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rspRaw = GetParamsString(_responseData);
            var myChecksum = HmacSHA512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetParamsString(SortedList<string, string> data)
        {
            var sb = new StringBuilder();
            foreach (var (key, value) in data.Where(kv => kv.Key.StartsWith("vnp_")))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    sb.Append(key + "=" + value + "&");
                }
            }
            return sb.ToString().TrimEnd('&');
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                var hashValue = hmac.ComputeHash(inputBytes);
                foreach (var b in hashValue)
                {
                    hash.Append(b.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }

    public class VnPayCompare : IComparer<string> {
        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}*/