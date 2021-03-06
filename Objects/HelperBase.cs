﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json;
using TracerX;

namespace BCM_Migration_Tool.Objects
{
    internal class HelperBase
    {
        #region Fields
        internal HttpClient _httpClient = new HttpClient();
        private static readonly Logger Log = Logger.GetLogger("HelperBase");
        internal enum ImportModes
        {
            Create,
            Update,
            UpdatePreviouslyImportedItem
            //UpdateWithActivities
        }

        internal enum RequestDataFormats
        {
            None,
            JSON,
            XML
        }
        internal enum RequestDataTypes
        {
            Activities,
            Companies,
            Contacts,
            Deals,
            EWS,
            Links,
            Templates
        }
        internal enum UpdateKeyTypes
        {
            BCMID,
            EmailAddress,
            Name
        }
        #endregion
        #region Constructors
        internal HelperBase()
        {
            
        }
        internal HelperBase(string accessToken, string connectionString)
        {
            AccessToken = accessToken;
            ConnectionString = connectionString;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //x-AnchorMailbox
            //NOTE: For optimal performance when using the new Outlook REST endpoint, add an x-AnchorMailbox header for every request and set it to the user's email address. For example: x-AnchorMailbox:john@contoso.com
            _httpClient.DefaultRequestHeaders.Add("X-AnchorMailbox", MigrationHelper.CurrentUser.EmailAddress);
        }
        #endregion
        #region Events
        internal event EventHandler<HelperEventArgs> CreateItemComplete;
        internal event EventHandler<HelperEventArgs> DisplayMessage;
        internal event EventHandler<HelperEventArgs> Error;        
        internal event EventHandler<HelperEventArgs> GetComplete;
        internal event EventHandler<HelperEventArgs> GetItemComplete;
        internal event EventHandler<HelperEventArgs> HelperComplete;
        internal event EventHandler<HelperEventArgs> IncrementProgress;
        internal event EventHandler<HelperEventArgs> PatchComplete;
        internal event EventHandler<HelperEventArgs> Started;
        internal event EventHandler<HelperEventArgs> TokenExpired;
        protected virtual void OnCreateItemComplete(object sender, HelperEventArgs e)
        {
            CreateItemComplete?.Invoke(sender, e);
        }
        protected virtual void OnDisplayMessage(object sender, HelperEventArgs e)
        {
            DisplayMessage?.Invoke(sender, e);
        }
        protected virtual void OnError(object sender, HelperEventArgs e)
        {
            Error?.Invoke(sender, e);
        }
        protected virtual void OnGetComplete(object sender, HelperEventArgs e)
        {
            GetComplete?.Invoke(sender, e);
        }
        protected virtual void OnGetItemComplete(object sender, HelperEventArgs e)
        {
            GetItemComplete?.Invoke(sender, e);
        }
        protected virtual void OnHelperComplete(object sender, HelperEventArgs e)
        {
            HelperComplete?.Invoke(sender, e);
        }
        protected virtual void OnIncrementProgress(object sender, HelperEventArgs e)
        {
            IncrementProgress?.Invoke(sender, e);
        }
        protected virtual void OnPatchComplete(object sender, HelperEventArgs e)
        {
            PatchComplete?.Invoke(sender, e);
        }

        protected virtual void OnStart(object sender, HelperEventArgs e)
        {
            Started?.Invoke(sender, e);
        }

        protected virtual void OnTokenExpired(object sender, HelperEventArgs e)
        {
            TokenExpired?.Invoke(sender, e);
        }
        #endregion
        #region Properties
        protected internal string AccessToken { get; set; }
        internal bool BCMDataLogged { get; set; }
        internal bool Cancelled { get; set; }
        internal string ConnectionString { get; private set; }
        internal bool DisableCustomFields { get; set; }
        internal bool FullRESTLogging { get; set; }        
        internal bool LogRecordNames { get; set; }
        internal int NumberCreated { get; set; }
        internal int NumberOfErrors { get; set; }
        internal int NumberToProcess { get; set; }
        internal int NumberUpdated { get; set; }
        /// <summary>
        /// Set to true after existing OCM data (for the Company, Contact or Deal) has already been retrieved and not to re-fetch if running a subsequent migration in the same session
        /// </summary>
        internal bool OCMDataRetrieved { get; set; }
        internal int RESTRetryDelay { get; set; }
        internal int RESTRetryMax { get; set; }
        internal int TestingMaximum { get; set; }
        internal bool TestMode { get; set; }
        #endregion
        #region Methods
        internal void Cancel()
        {
            Cancelled = true;
        }        
        /// <summary>
        /// Use if you want to override any testing settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="retryUntilTimeout"></param>
        /// <param name="timeOut"></param>
        /// <param name="retries"></param>
        /// <param name="logContent"></param>
        /// <param name="logMessage"></param>
        /// <param name="doNotLogRequest"></param>
        /// <param name="doNotLogErrors"></param>
        /// <returns></returns>
        public Task<RestResponse<T>> Get<T>(string request, bool retryUntilTimeout, int timeOut, int retries, bool logContent, string logMessage, bool doNotLogRequest, bool doNotLogErrors) where T : class
        {
            if (!doNotLogRequest)
                Log.VerboseFormat("GET request to {0}", request);
            return ParseJsonResponseForGET<T>(request, retryUntilTimeout, timeOut, retries, logContent, logMessage, doNotLogErrors);
        }
        public Task<RestResponse<T>> Get<T>(string request, bool logContent, string logMessage, bool doNotLogRequest, bool doNotLogErrors) where T : class
        {
            if (!doNotLogRequest)
                Log.VerboseFormat("GET request to {0}", request);
            return ParseJsonResponseForGET<T>(request, logContent, logMessage, doNotLogErrors);
        }
        public Task<RestResponse<T>> Get<T>(string request) where T : class
        {
            Log.VerboseFormat("GET request to {0}", request);
            return ParseJsonResponseForGET<T>(request, true, RESTRetryDelay, RESTRetryMax, FullRESTLogging, null, false);
        }
        /// <summary>
        /// Use if you want to override any testing settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="payload"></param>
        /// <param name="json"></param>
        /// <param name="retryUntilTimeout"></param>
        /// <param name="timeOut"></param>
        /// <param name="retries"></param>
        /// <param name="logContent"></param>
        /// <param name="logMessage"></param>
        /// <param name="doNotLogRequest"></param>
        /// <param name="doNotLogErrors"></param>
        /// <returns></returns>
        public Task<RestResponse<T>> Patch<T>(HttpRequestMessage request, bool retryUntilTimeout, int timeOut, int retries, bool logContent, string logMessage, bool doNotLogRequest, bool doNotLogErrors) where T : class
        {
            if (!doNotLogRequest)
                Log.VerboseFormat("PATCH request: {0}", request);
            return ParseJsonResponseForPATCH<T>(request, retryUntilTimeout, timeOut, retries, logContent, logMessage, doNotLogErrors);
        }
        public Task<RestResponse<T>> Patch<T>(HttpRequestMessage request, bool logContent, string logMessage, bool doNotLogRequest, bool doNotLogErrors) where T : class
        {
            if (!doNotLogRequest)
                Log.VerboseFormat("PATCH request: {0}", request);
            return ParseJsonResponseForPATCH<T>(request, logContent, logMessage, doNotLogErrors);
        }
        public Task<RestResponse<T>> Patch<T>(HttpRequestMessage request) where T : class
        {
            if (FullRESTLogging)
                Log.VerboseFormat("PATCH request to {0}", request);
            return ParseJsonResponseForPATCH<T>(request, true, RESTRetryDelay, RESTRetryMax, FullRESTLogging, null, false);
        }

        /// <summary>
        /// Use if you want to override any testing settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="payload"></param>
        /// <param name="json"></param>
        /// <param name="retryUntilTimeout"></param>
        /// <param name="timeOut"></param>
        /// <param name="retries"></param>
        /// <param name="logContent"></param>
        /// <param name="logMessage"></param>
        /// <param name="doNotLogRequest"></param>
        /// <param name="doNotLogErrors"></param>
        /// <returns></returns>
        public Task<RestResponse<T>> Post<T>(Uri request, HttpContent payload, string json, bool retryUntilTimeout, int timeOut, int retries, bool logContent, string logMessage, bool doNotLogRequest, bool doNotLogErrors) where T : class
        {
            if (!doNotLogRequest)
                Log.VerboseFormat("POST request to {0}{1}", request, json != null ? ": " + json : null);
            return ParseJsonResponseForPOST<T>(request, payload, retryUntilTimeout, timeOut, retries, logContent, logMessage, doNotLogErrors);
        }
        public Task<RestResponse<T>> Post<T>(Uri request, HttpContent payload, string json, bool logContent, string logMessage, bool doNotLogRequest, bool doNotLogErrors) where T : class
        {
            if (!doNotLogRequest)
                Log.VerboseFormat("POST request to {0}{1}", request, json != null ? ": " + json : null);
            return ParseJsonResponseForPOST<T>(request, payload, logContent, logMessage, doNotLogErrors);
        }
        public Task<RestResponse<T>> Post<T>(Uri request, HttpContent payload) where T : class
        {
            if (FullRESTLogging)
                Log.VerboseFormat("POST request to {0}", request);
            return ParseJsonResponseForPOST<T>(request, payload, true, RESTRetryDelay, RESTRetryMax, FullRESTLogging, null, false);
        }
        public Task<RestResponse<T>> Post<T>(Uri request, string requestData, RequestDataFormats format, bool logContent, string logMessage, bool doNotLogRequest, bool doNotLogErrors) where T : class
        {
            if (!doNotLogRequest)
                Log.VerboseFormat("POST request to {0}{1}", request, requestData != null ? ": " + requestData : null);
            return ParseJsonResponseForPOST<T>(request, requestData, format, logContent, logMessage, doNotLogErrors);
        }
        public Task<RestResponse<T>> Post<T>(Uri request, string requestData, RequestDataFormats format) where T : class
        {
            if (FullRESTLogging)
                Log.VerboseFormat("POST request to {0}", request);
            return ParseJsonResponseForPOST<T>(request, requestData, format, true, RESTRetryDelay, RESTRetryMax, FullRESTLogging, null, false);
        }
        private async Task<RestResponse<T>> ParseJsonResponseForGET<T>(string request, bool logContent, string logMessage, bool doNotLogErrors) where T : class
        {
            return await ParseJsonResponseForGET<T>(request, true, RESTRetryDelay, RESTRetryMax, logContent, logMessage,
                doNotLogErrors);
        }
        /// <summary>
        /// Use if you want to override any testing settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="retryUntilTimeout"></param>
        /// <param name="timeOut"></param>
        /// <param name="retries"></param>
        /// <param name="logContent"></param>
        /// <param name="logMessage"></param>
        /// <param name="doNotLogErrors"></param>
        /// <returns></returns>
        private async Task<RestResponse<T>> ParseJsonResponseForGET<T>(string request, bool retryUntilTimeout, int timeOut, int retries, bool logContent, string logMessage, bool doNotLogErrors) where T : class
        {
            using (Log.VerboseCall())
            {
                RestResponse<T> result = null;
                HttpStatusCode lastStatusCode;
                int retryCnt = 0;
                bool tokenRetry = false;

                retry:

                using (var response = await _httpClient.GetAsync(request))
                {
                    if (response == null)
                    {
                        //return new RestResponse<T> {StatusCode = HttpStatusCode.ServiceUnavailable};
                        return new RestResponse<T> {FatalError = true};
                    }
                    
                    var content = await response.Content.ReadAsStringAsync();

                    if (logContent)
                    {
                        Log.VerboseFormat("{0}{1}{2}", logMessage, String.IsNullOrEmpty(logMessage) ? "" : Environment.NewLine, content);
                    }

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            object objResponse;
                            try
                            {
                                objResponse = JsonConvert.DeserializeObject<T>(content); //jsonSerializer.ReadObject(content);
                            }
                            catch
                            {
                                objResponse = null;
                            }
                            result = new RestResponse<T>();
                            result.StatusCode = response.StatusCode;
                            result.Content = objResponse as T;
                            tokenRetry = false;
                            break;
                        case HttpStatusCode.Unauthorized:
                            //result = new RestResponse<T>() { StatusCode = response.StatusCode };
                            lastStatusCode = response.StatusCode;
                            if (!tokenRetry)
                            {
                                AuthenticationResult authenticationResult = MigrationHelper.GetAccessToken(true);
                                if (authenticationResult != null && !String.IsNullOrEmpty(authenticationResult.AccessToken))
                                {
                                    AccessToken = authenticationResult.AccessToken;
                                    Log.WarnFormat("Retrying with refreshed token {0}", AccessToken);
                                    tokenRetry = true; //Only retry once
                                    //goto retry;
                                }
                                else
                                {
                                    result = new RestResponse<T>() { StatusCode = response.StatusCode };
                                }
                            }
                            else
                            {
                                tokenRetry = false; //Only retry once
                            }
                            break;
                        case HttpStatusCode.ServiceUnavailable:
                            lastStatusCode = response.StatusCode;
                            if (retryUntilTimeout && retries != 0 && retryCnt <= retries)
                            {
                                Log.WarnFormat("ServiceUnavailable; sleeping for {0} (retry count: {1})", timeOut, retryCnt);
                                Thread.Sleep(timeOut);
                                retryCnt++;
                                //goto retry;
                                tokenRetry = true; //Only retry once
                            }
                            else
                            {
                                result = new RestResponse<T>() { StatusCode = response.StatusCode };
                                tokenRetry = false;
                            }
                            break;
                        default:
                            result = new RestResponse<T>();
                            result.StatusCode = response.StatusCode;
                            result.ErrorContent = content;
                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);
                            tokenRetry = false;
                            break;
                    }
                }

                if (tokenRetry)
                    goto retry;

                return result;
            }            
        }
        private async Task<RestResponse<T>> ParseJsonResponseForPATCH<T>(HttpRequestMessage request, bool logContent, string logMessage, bool doNotLogErrors) where T : class
        {
            return await ParseJsonResponseForPATCH<T>(request, true, RESTRetryDelay, RESTRetryMax, logContent, logMessage,
                doNotLogErrors);
        }
        /// <summary>
        /// Use if you want to override any testing settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="payload"></param>
        /// <param name="retryUntilTimeout"></param>
        /// <param name="timeOut"></param>
        /// <param name="retries"></param>
        /// <param name="logContent"></param>
        /// <param name="logMessage"></param>
        /// <param name="doNotLogErrors"></param>
        /// <returns></returns>
        private async Task<RestResponse<T>> ParseJsonResponseForPATCH<T>(HttpRequestMessage request, bool retryUntilTimeout, int timeOut, int retries, bool logContent, string logMessage, bool doNotLogErrors = false) where T : class
        {
            using (Log.VerboseCall())
            {
                RestResponse<T> result = null;
                HttpStatusCode lastStatusCode;
                int retryCnt = 0;
                bool tokenRetry = false;

                retry:

                using (var response = await _httpClient.SendAsync(request))
                {
                    if (response == null)
                    {
                        //return new RestResponse<T> {StatusCode = HttpStatusCode.ServiceUnavailable};
                        return new RestResponse<T> { FatalError = true };
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    if (logContent)
                    {
                        Log.VerboseFormat("{0}{1}{2}", logMessage, String.IsNullOrEmpty(logMessage) ? "" : Environment.NewLine, content);
                    }

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Created:
                        case HttpStatusCode.OK:
                            object objResponse;
                            try
                            {
                                objResponse = JsonConvert.DeserializeObject<T>(content); //jsonSerializer.ReadObject(content);
                            }
                            catch
                            {
                                //This is okay for some POST requests, like those that return XML that we don't need to parse/Deserialize. Only with OK responses??
                                objResponse = null;
                            }
                            try
                            {
                                result = new RestResponse<T>();
                                result.StatusCode = response.StatusCode;
                                result.Content = objResponse as T;
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error(ex);
                                result = new RestResponse<T>() { StatusCode = response.StatusCode };
                            }
                            finally
                            { tokenRetry = false; } 
                            break;
                        case HttpStatusCode.Unauthorized:
                            //result = new RestResponse<T>() { StatusCode = response.StatusCode };
                            lastStatusCode = response.StatusCode;
                            if (!tokenRetry)
                            {
                                AuthenticationResult authenticationResult = MigrationHelper.GetAccessToken(true);
                                if (authenticationResult != null && !String.IsNullOrEmpty(authenticationResult.AccessToken))
                                {
                                    AccessToken = authenticationResult.AccessToken;
                                    Log.WarnFormat("Retrying with refreshed token {0}", AccessToken);
                                    tokenRetry = true; //Only retry once
                                    //goto retry;
                                }
                                else
                                {
                                    result = new RestResponse<T>() { StatusCode = response.StatusCode };
                                }
                            }
                            else
                            {
                                tokenRetry = false; //Only retry once
                            }
                            break;
                        case HttpStatusCode.ServiceUnavailable:
                            lastStatusCode = response.StatusCode;
                            if (retryUntilTimeout && retries != 0 && retryCnt <= retries)
                            {
                                Log.WarnFormat("ServiceUnavailable; sleeping for {0} (retry count: {1})", timeOut, retryCnt);
                                Thread.Sleep(timeOut);
                                retryCnt++;
                                //goto retry;
                                tokenRetry = true; //Only retry once
                            }
                            else
                            {
                                result = new RestResponse<T>() { StatusCode = response.StatusCode };
                                tokenRetry = false;
                            }
                            break;
                        default:
                            result = new RestResponse<T>();
                            result.StatusCode = response.StatusCode;
                            result.ErrorContent = content;
                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);
                            tokenRetry = false;
                            break;
                    }
                }

                if (tokenRetry)
                    goto retry;

                return result;
            }
        }
        private async Task<RestResponse<T>> ParseJsonResponseForPOST<T>(Uri request, HttpContent payload, bool logContent, string logMessage, bool doNotLogErrors) where T : class
        {
            return await ParseJsonResponseForPOST<T>(request, payload, true, RESTRetryDelay, RESTRetryMax, logContent, logMessage,
                doNotLogErrors);
        }
        private async Task<RestResponse<T>> ParseJsonResponseForPOST<T>(Uri request, string requestData, RequestDataFormats format, bool logContent, string logMessage, bool doNotLogErrors) where T : class
        {
            return await ParseJsonResponseForPOST<T>(request, requestData, format, true, RESTRetryDelay, RESTRetryMax, logContent, logMessage,
                doNotLogErrors);
        }
        /// <summary>
        /// Use if you want to override any testing settings
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="payload"></param>
        /// <param name="retryUntilTimeout"></param>
        /// <param name="timeOut"></param>
        /// <param name="retries"></param>
        /// <param name="logContent"></param>
        /// <param name="logMessage"></param>
        /// <param name="doNotLogErrors"></param>
        /// <returns></returns>
        private async Task<RestResponse<T>> ParseJsonResponseForPOST<T>(Uri request, HttpContent payload, bool retryUntilTimeout, int timeOut, int retries, bool logContent, string logMessage, bool doNotLogErrors = false) where T : class
        {
            using (Log.VerboseCall())
            {
                RestResponse<T> result = null;
                HttpStatusCode lastStatusCode;
                int retryCnt = 0;
                bool tokenRetry = false;

                retry:

                if (tokenRetry)
                    Log.Debug("Retrying...");

                //BUG 3/17/2018 When retrying, starting this again throws Exception type: System.ObjectDisposedException on payload (HttpContent) param. Should pass json string instead and create HttpContent object here before the using loop
                
                using (var response = await _httpClient.PostAsync(request, payload))
                {
                    if (response == null)
                    {
                        //return new RestResponse<T> {StatusCode = HttpStatusCode.ServiceUnavailable};
                        return new RestResponse<T> { FatalError = true };
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    if (logContent)
                    {
                        Log.VerboseFormat("{0}{1}{2}", logMessage, String.IsNullOrEmpty(logMessage) ? "" : Environment.NewLine, content);
                    }

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Created:
                        case HttpStatusCode.OK:
                            object objResponse;
                            try
                            {
                                objResponse = JsonConvert.DeserializeObject<T>(content); //jsonSerializer.ReadObject(content);
                            }
                            catch
                            {
                                //This is okay for some POST requests, like those that return XML that we don't need to parse/Deserialize. Only with OK responses??
                                objResponse = null;
                            }
                            try
                            {
                                result = new RestResponse<T>();
                                result.StatusCode = response.StatusCode;
                                if (payload.Headers.ContentType.MediaType == "text/xml")
                                {
                                    //There must be a better way to return string in the content if the caller is passing <string> as T...
                                    result.StringContent = content;
                                }
                                else
                                {
                                    result.Content = objResponse as T;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error(ex);
                                result = new RestResponse<T>() { StatusCode = response.StatusCode };
                            }
                            finally
                            { tokenRetry = false; }
                            break;
                        case HttpStatusCode.Unauthorized:
                            //result = new RestResponse<T>() { StatusCode = response.StatusCode };

                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);

                            lastStatusCode = response.StatusCode;
                            if (!tokenRetry)
                            {
                                AuthenticationResult authenticationResult = MigrationHelper.GetAccessToken(true);
                                if (authenticationResult != null &&
                                    !String.IsNullOrEmpty(authenticationResult.AccessToken))
                                {
                                    AccessToken = authenticationResult.AccessToken;
                                    Log.WarnFormat("Retrying with refreshed token {0}", AccessToken);
                                    tokenRetry = true; //Only retry once
                                    //goto retry;
                                }
                                else
                                {
                                    result = new RestResponse<T>() {StatusCode = response.StatusCode};
                                }
                            }
                            else
                            {
                                tokenRetry = false; //Only retry once
                            }
                            break;
                        case HttpStatusCode.ServiceUnavailable:

                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);

                            lastStatusCode = response.StatusCode;
                            if (retryUntilTimeout && retries != 0 && retryCnt <= retries)
                            {                                
                                Log.WarnFormat("ServiceUnavailable; sleeping for {0} (retry count: {1})", timeOut, retryCnt);
                                Thread.Sleep(timeOut);
                                retryCnt++;
                                //goto retry;
                                tokenRetry = true; //Only retry once
                            }
                            else
                            {
                                result = new RestResponse<T>() { StatusCode = response.StatusCode };
                                tokenRetry = false;
                            }
                            break;
                        case HttpStatusCode.InternalServerError:
                            //New 3/17
                            //BUG Should grab BackOffMilliseconds count and use that for timeout val, but hardcode to 300 seconds (300000 ms) for now                                                                 
                            //InternalServerError: <?xml version="1.0" encoding="utf-8"?><s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"><s:Body><s:Fault><faultcode xmlns:a="http://schemas.microsoft.com/exchange/services/2006/types">a:ErrorServerBusy</faultcode><faultstring xml:lang="en-US">The server cannot service this request right now. Try again later.</faultstring><detail><e:ResponseCode xmlns:e="http://schemas.microsoft.com/exchange/services/2006/errors">ErrorServerBusy</e:ResponseCode><e:Message xmlns:e="http://schemas.microsoft.com/exchange/services/2006/errors">The server cannot service this request right now. Try again later.</e:Message><t:MessageXml xmlns:t="http://schemas.microsoft.com/exchange/services/2006/types"><t:Value Name="BackOffMilliseconds">299718</t:Value></t:MessageXml></detail></s:Fault></s:Body></s:Envelope>

                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);

                            lastStatusCode = response.StatusCode;
                            if (retryUntilTimeout && retries != 0 && retryCnt <= retries)
                            {
                                if (content.Contains("ErrorServerBusy"))
                                {
                                    OnError(null, new HelperEventArgs(String.Format("Server is busy/overloaded - must pause for 5 minutes (at: {0})", DateTime.Now.ToLongTimeString()), HelperEventArgs.EventTypes.Error));
                                    Log.WarnFormat("Server is busy - sleeping for {0} (retry count: {1})", 300000,
                                        retryCnt);
                                    Thread.Sleep(300000);
                                }
                                else
                                {
                                    Log.WarnFormat("InternalServerError; sleeping for {0} (retry count: {1})", timeOut,
    retryCnt);
                                    Thread.Sleep(timeOut);
                                }

                                retryCnt++;
                                //goto retry;
                                tokenRetry = true; //Only retry once
                            }
                            else
                            {
                                result = new RestResponse<T>() { StatusCode = response.StatusCode };
                                tokenRetry = false;
                            }
                            break;

                        default:
                            result = new RestResponse<T>();
                            result.StatusCode = response.StatusCode;
                            result.ErrorContent = content;
                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);
                            tokenRetry = false;
                            break;
                    }
                }

                if (tokenRetry)
                    goto retry;
                
                return result;
            }
        }
        /// <summary>
        /// Use to build HttpContent inline so it is not disposed if a retry is need. Use if you want to override any testing settings.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="requestData"></param>
        /// <param name="format"></param>
        /// <param name="retryUntilTimeout"></param>
        /// <param name="timeOut"></param>
        /// <param name="retries"></param>
        /// <param name="logContent"></param>
        /// <param name="logMessage"></param>
        /// <param name="doNotLogErrors"></param>
        /// <returns></returns>
        private async Task<RestResponse<T>> ParseJsonResponseForPOST<T>(Uri request, string requestData, RequestDataFormats format, bool retryUntilTimeout, int timeOut, int retries, bool logContent, string logMessage, bool doNotLogErrors = false) where T : class
        {
            HttpContent payload = null;

            using (Log.VerboseCall())
            {
                RestResponse<T> result = null;
                HttpStatusCode lastStatusCode;
                int retryCnt = 0;
                bool tokenRetry = false;

                retry:

                payload = null;
                switch (format)
                {
                    case RequestDataFormats.JSON:
                        payload = new StringContent(requestData, Encoding.UTF8, "application/json");
                        break;
                    case RequestDataFormats.XML:
                        payload = new StringContent(requestData, Encoding.UTF8, "text/xml");
                        break;
                }

                if (tokenRetry)
                    Log.Debug("Retrying...");

                //BUG 3/17/2018 When retrying, starting this again throws Exception type: System.ObjectDisposedException on payload (HttpContent) param. Should pass json string instead and create HttpContent object here before the using loop

                using (var response = await _httpClient.PostAsync(request, payload))
                {
                    if (response == null)
                    {
                        //return new RestResponse<T> {StatusCode = HttpStatusCode.ServiceUnavailable};
                        return new RestResponse<T> { FatalError = true };
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    if (logContent)
                    {
                        Log.VerboseFormat("{0}{1}{2}", logMessage, String.IsNullOrEmpty(logMessage) ? "" : Environment.NewLine, content);
                    }

                    switch (response.StatusCode)
                    {
                        case HttpStatusCode.Created:
                        case HttpStatusCode.OK:
                            object objResponse;
                            try
                            {
                                objResponse = JsonConvert.DeserializeObject<T>(content); //jsonSerializer.ReadObject(content);
                            }
                            catch
                            {
                                //This is okay for some POST requests, like those that return XML that we don't need to parse/Deserialize. Only with OK responses??
                                objResponse = null;
                            }
                            try
                            {
                                result = new RestResponse<T>();
                                result.StatusCode = response.StatusCode;
                                if (payload.Headers.ContentType.MediaType == "text/xml")
                                {
                                    //There must be a better way to return string in the content if the caller is passing <string> as T...
                                    result.StringContent = content;
                                }
                                else
                                {
                                    result.Content = objResponse as T;
                                }
                            }
                            catch (System.Exception ex)
                            {
                                Log.Error(ex);
                                result = new RestResponse<T>() { StatusCode = response.StatusCode };
                            }
                            finally
                            { tokenRetry = false; }
                            break;
                        case HttpStatusCode.Unauthorized:
                            //result = new RestResponse<T>() { StatusCode = response.StatusCode };

                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);

                            lastStatusCode = response.StatusCode;
                            if (!tokenRetry)
                            {
                                AuthenticationResult authenticationResult = MigrationHelper.GetAccessToken(true);
                                if (authenticationResult != null &&
                                    !String.IsNullOrEmpty(authenticationResult.AccessToken))
                                {
                                    AccessToken = authenticationResult.AccessToken;
                                    Log.WarnFormat("Retrying with refreshed token {0}", AccessToken);
                                    tokenRetry = true; //Only retry once
                                    //goto retry;
                                }
                                else
                                {
                                    result = new RestResponse<T>() { StatusCode = response.StatusCode };
                                }
                            }
                            else
                            {
                                tokenRetry = false; //Only retry once
                            }
                            break;
                        case HttpStatusCode.ServiceUnavailable:

                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);

                            lastStatusCode = response.StatusCode;
                            if (retryUntilTimeout && retries != 0 && retryCnt <= retries)
                            {
                                Log.WarnFormat("ServiceUnavailable; sleeping for {0} (retry count: {1})", timeOut, retryCnt);
                                Thread.Sleep(timeOut);
                                retryCnt++;
                                //goto retry;
                                tokenRetry = true; //Only retry once
                            }
                            else
                            {
                                result = new RestResponse<T>() { StatusCode = response.StatusCode };
                                tokenRetry = false;
                            }
                            break;
                        case HttpStatusCode.InternalServerError:
                            //New 3/17
                            //BUG Should grab BackOffMilliseconds count and use that for timeout val, but hardcode to 300 seconds (300000 ms) for now                                                                 
                            //InternalServerError: <?xml version="1.0" encoding="utf-8"?><s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"><s:Body><s:Fault><faultcode xmlns:a="http://schemas.microsoft.com/exchange/services/2006/types">a:ErrorServerBusy</faultcode><faultstring xml:lang="en-US">The server cannot service this request right now. Try again later.</faultstring><detail><e:ResponseCode xmlns:e="http://schemas.microsoft.com/exchange/services/2006/errors">ErrorServerBusy</e:ResponseCode><e:Message xmlns:e="http://schemas.microsoft.com/exchange/services/2006/errors">The server cannot service this request right now. Try again later.</e:Message><t:MessageXml xmlns:t="http://schemas.microsoft.com/exchange/services/2006/types"><t:Value Name="BackOffMilliseconds">299718</t:Value></t:MessageXml></detail></s:Fault></s:Body></s:Envelope>

                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);

                            lastStatusCode = response.StatusCode;
                            if (retryUntilTimeout && retries != 0 && retryCnt <= retries)
                            {
                                if (content.Contains("ErrorServerBusy"))
                                {
                                    OnError(null, new HelperEventArgs(String.Format("Server is busy/overloaded - must pause for 5 minutes (at: {0})", DateTime.Now.ToLongTimeString()), HelperEventArgs.EventTypes.Error));
                                    Log.WarnFormat("Server is busy - sleeping for {0} (retry count: {1})", 300000,
                                        retryCnt);
                                    Thread.Sleep(300000);
                                }
                                else
                                {
                                    Log.WarnFormat("InternalServerError; sleeping for {0} (retry count: {1})", timeOut,
    retryCnt);
                                    Thread.Sleep(timeOut);
                                }

                                retryCnt++;
                                //goto retry;
                                tokenRetry = true; //Only retry once
                            }
                            else
                            {
                                result = new RestResponse<T>() { StatusCode = response.StatusCode };
                                tokenRetry = false;
                            }
                            break;

                        default:
                            result = new RestResponse<T>();
                            result.StatusCode = response.StatusCode;
                            result.ErrorContent = content;
                            if (!doNotLogErrors)
                                Log.ErrorFormat("{0}: {1}", response.StatusCode, content);
                            tokenRetry = false;
                            break;
                    }
                }

                if (tokenRetry)
                    goto retry;

                return result;
            }
        }
        internal void PrepareRequest(RequestDataTypes requestDataType, RequestDataFormats requestDataFormat)
        {
            //TODO Cleanup enum params - some are not used
            if (DateTimeOffset.UtcNow.CompareTo(MigrationHelper.TokenExpiresAt) > 0)
            {
                Log.Warn("Token has expired!");
                AuthenticationResult authenticationResult = MigrationHelper.GetAccessToken(true);
                if (authenticationResult != null && !String.IsNullOrEmpty(authenticationResult.AccessToken))
                {
                    AccessToken = authenticationResult.AccessToken;
                }
            }

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            //NOTE: For optimal performance when using the new Outlook REST endpoint, add an x-AnchorMailbox header for every request and set it to the user's email address. For example: x-AnchorMailbox:john@contoso.com
            _httpClient.DefaultRequestHeaders.Add("X-AnchorMailbox", MigrationHelper.CurrentUser.EmailAddress);

            switch (requestDataType)
            {
                case RequestDataTypes.Activities:
                    _httpClient.DefaultRequestHeaders.Add("Prefer", "exchange.behavior=\"SocialFabricInternal\"");
                    break;
                case RequestDataTypes.Companies:
                    break;
                case RequestDataTypes.Contacts:
                    break;
                case RequestDataTypes.Deals:
                    //REVIEW Needed for Deals, but sample Fiddler requests had this header set for Orgs/Accounts and Contacts
                    _httpClient.DefaultRequestHeaders.Add("Prefer", "exchange.behavior=\"SocialFabricInternal\"");
                    break;
                case RequestDataTypes.Templates:
                    break;
                case RequestDataTypes.Links:
                    break;
            }

            switch (requestDataFormat)
            {
                case RequestDataFormats.JSON:
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    break;
                case RequestDataFormats.XML://NOT USED
                    break;
            }
        }
        #endregion
    }
    public class HelperEventArgs : EventArgs
    {
        #region Fields
        private string msg;
        public enum EventTypes
        {
            Status,
            Error,
            FatalError
        }

        public enum ProcessingModes
        {
            Mapping,
            Migrating
        }
        #endregion
        #region Constructors
        public HelperEventArgs(string s, bool error)
        {
            Error = error;
            msg = s;
            ProcessingMode = ProcessingModes.Migrating;
        }
        public HelperEventArgs(string s, EventTypes eventType)
        {
            if (eventType != EventTypes.Status)
                Error = true;
            msg = s;
            EventType = eventType;
            ProcessingMode = ProcessingModes.Migrating;
        }
        public HelperEventArgs(string s, EventTypes eventType, Exception e)
        {
            Error = true;
            msg = s;
            EventType = eventType;
            ErrorException = e;
            ProcessingMode = ProcessingModes.Migrating;
        }
        public HelperEventArgs(string s, EventTypes eventType, ProcessingModes processingMode)
        {
            Error = true;
            msg = s;
            EventType = eventType;            
            ProcessingMode = processingMode;
        }
        public HelperEventArgs(string s, EventTypes eventType, ProcessingModes processingMode, Exception e)
        {
            Error = true;
            msg = s;
            EventType = eventType;
            ErrorException = e;
            ProcessingMode = processingMode;
        }
        #endregion
        #region Properties
        public bool Error { get; set; }
        public Exception ErrorException { get; private set; }
        public EventTypes EventType { get; set; }
        public string Message
        {
            get { return msg; }
        }
        public ProcessingModes ProcessingMode { get; set; }

        #endregion
    }
}
