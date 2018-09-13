using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Http.Core;

namespace FclEx.Http
{
    public static class HttpReqPropExtensions
    {
        public static HttpReq ReadResultCookie(this HttpReq req, bool read)
        {
            req.ReadResultCookie = read;
            return req;
        }

        public static HttpReq ReadResultHeader(this HttpReq req, bool read)
        {
            req.ReadResultHeader = read;
            return req;
        }

        public static HttpReq ReadResultContent(this HttpReq req, bool read)
        {
            req.ReadResultContent = read;
            return req;
        }

        public static HttpReq ThrowOnNonSuccessCode(this HttpReq req, bool ifThrow)
        {
            req.ThrowOnNonSuccessCode = ifThrow;
            return req;
        }

        public static HttpReq Host(this HttpReq req, string host)
        {
            req.Host = host;
            return req;
        }

        public static HttpReq Port(this HttpReq req, int port)
        {
            req.Port = port;
            return req;
        }

        public static HttpReq Fragment(this HttpReq req, string fragment)
        {
            req.Fragment = fragment;
            return req;
        }

        public static HttpReq UserName(this HttpReq req, string userName)
        {
            req.UserName = userName;
            return req;
        }

        public static HttpReq Password(this HttpReq req, string password)
        {
            req.Password = password;
            return req;
        }

        public static HttpReq Path(this HttpReq req, string path)
        {
            req.Path = path;
            return req;
        }

        public static HttpReq Scheme(this HttpReq req, string scheme)
        {
            req.Scheme = scheme;
            return req;
        }


    }
}
