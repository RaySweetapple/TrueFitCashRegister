
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace TrueFit.Utilities.ExceptionManagement
{
    public static class EnvironmentExceptionFormatter
    {
        #region Constants

        private const string PERMISSION_DENIED_MESSAGE = "Permission denied.";
        private const string NO_ACCESS_MESSAGE = "Information could not be accessed.";

        #endregion

        #region Format

        public static string Format(Exception ex, Dictionary<string, string>? additionalInfo = null, string? message = null)
        {
           var returnValue = GetErrorString(null, ex, additionalInfo, message);    
           return returnValue;
        }

        public static string Format(HttpContext context, string message)
        {           
           var returnValue = GetErrorString(context, null, null, message);    
           return returnValue;
        }

        public static string Format(HttpContext context, Exception? exception, Dictionary<string, string>? additionalInfo = null, string? message = null, IDictionary<string, object>? modelObjectDictionary = null)
        {
           var returnValue = GetErrorString(context, exception, additionalInfo, message, modelObjectDictionary);    
           return returnValue;
        }

        public static string Format(HttpContext context, IExceptionHandlerPathFeature? exception, Dictionary<string, string>? additionalInfo = null, string? message = null)
        {
            var returnValue = GetErrorString(context, exception?.Error, additionalInfo, message);    
            return returnValue;
        }

        #endregion

        #region Helper Methods

        #region DoScrubValue

        private static bool DoScrubValue(string key)
        {
            key = key.ToLower();

            var restrictedList = new List<string> { "card", "credit", "cvv", "password"};
          
            foreach (var r in restrictedList)
            {
                if (key.Contains(r.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region ErrorWriteLine

        private static string ErrorWriteLine(string? keyName, string? value)
        {
            var strB = new StringBuilder();
            if (string.IsNullOrEmpty(keyName))
            {
                keyName = "";
            }
            if (string.IsNullOrEmpty(value))
            {
                value = "";
            }

            strB.Append($"{Environment.NewLine}{keyName}: {value}");
            return strB.ToString();
        }

        private static string ErrorWriteLine(string keyName, object? value)
        {
            var strB = new StringBuilder();
            if (string.IsNullOrEmpty(keyName))
            {
                keyName = "";
            }
            
            value ??= "";

            strB.Append($"{Environment.NewLine}{keyName}: {value}");
            return strB.ToString();
        }
       

        #endregion

        #region GetAdditionalInfo

        private static StringBuilder GetAdditionalInfo(StringBuilder info, Dictionary<string, string>? additionalInfo)
        {
            if( additionalInfo == null)
            {
                return info;
            }

            foreach (var (key, value) in additionalInfo)
            {
                info.AppendFormat(ErrorWriteLine(key, value));
            }

            return info;
        }


        #endregion

        #region GetErrorMessages

        private static StringBuilder GetErrorMessages(StringBuilder info, Exception? exception, int innerExceptionCount = 0)
        {
            if (exception == null || innerExceptionCount > 10)
            {
                return info;
            }
            var messageLabel = "Exception Message";
            var stackTraceLabel = "Stack Trace";
            if (innerExceptionCount > 0)
            {
                messageLabel = "Inner Exception Message";
                stackTraceLabel = $"Inner Exception ({innerExceptionCount})";
            }

            var exceptionMessage = exception.Message;
            info.Append($"{messageLabel} ------");
            info.Append(Environment.NewLine);
            if (!string.IsNullOrEmpty(exceptionMessage))
            {
                info.Append(exceptionMessage);
                info.Append(Environment.NewLine);
            }

            var stackTrace = exception.StackTrace;
            info.Append($"{stackTraceLabel} ------");
            info.Append(Environment.NewLine);
            if (stackTrace != null)
            {
                info.Append(stackTrace);
                info.Append(Environment.NewLine);
                info.Append(Environment.NewLine);
            }
               
            var innerException = exception.InnerException;
            // ReSharper disable once InvertIf
            if (innerException != null)
            {
                var innerString = GetErrorMessages(info, exception.InnerException, innerExceptionCount + 1 );
                info.Append(innerString);
            }

            return info;
        }

        #endregion

        #region GetFormInfo

        /// <summary>
        /// Get Form Information
        /// </summary>
        /// <returns></returns>
        private static string GetFormInfo(HttpContext? context)
        {
            var strB = new StringBuilder();
            strB.Append(Environment.NewLine);
            strB.Append("Form Values ------");
            if (context == null)
            {
                strB.AppendFormat("HttpContext is null. No form values found.");
                return strB.ToString();
            }
            try
            {
                var dictionary = context.Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                if (dictionary.Count == 0)
                {
                    strB.AppendFormat(ErrorWriteLine("Form", "No values."));
                }
                foreach (var dict in dictionary )
                {
                    string value;
                    try
                    {
                        value = DoScrubValue(dict.Key) ? "Removed due to security considerations." : dict.Value;
                    }
                    catch (SecurityException)
                    {
                        value = PERMISSION_DENIED_MESSAGE;
                    }
                    catch
                    {
                        value = NO_ACCESS_MESSAGE;
                    }
                    strB.AppendFormat(ErrorWriteLine(dict.Key, value));
                }
            }
            catch (SecurityException)
            {
                strB.AppendFormat(ErrorWriteLine("Form", PERMISSION_DENIED_MESSAGE));
            }
            catch
            {
                strB.AppendFormat(ErrorWriteLine("Form", NO_ACCESS_MESSAGE));
            }
            strB.Append(Environment.NewLine);

            return strB.ToString();
        }

        #endregion

        #region GetIndentValue

        private static string GetIndentValue(int indent)
        {
            if (indent > 20)
            {
                indent = 20;
            }
            var indentBuilder = new StringBuilder();
            for (var i = 0; i < indent; i++)
            {
                indentBuilder.Append(" ");
            }
            return indentBuilder.ToString();
        }

        #endregion

        #region GetListValues

        private static string GetListValues(object? properties, int indent)
        {
            var message = new StringBuilder(); 
            if (properties == null || (properties.GetType() == typeof(string) && string.IsNullOrEmpty((string)properties)) )
            {
                return message.ToString();
            }            
            
            var indentValue = GetIndentValue(indent);
           
            foreach (var property in properties.GetType().GetProperties())
            {              
                if ((property.PropertyType == typeof(string)  || property.PropertyType.IsPrimitive ) && property.GetValue(properties) != null)
                {
                    var value = property.GetValue(properties)?.ToString();
                    message.Append($"{indentValue}{property.Name}: {value}{Environment.NewLine}");
                  
                }               
            }  

            return message.ToString();
        }

        #endregion

        #region GetObjectProperties

        private static string GetDictionaryValues(IDictionary<string, object>? modelProperties)
        {
            var message = new StringBuilder();            
            try 
            {
                if (modelProperties == null || modelProperties.Count == 0)
                {
                    return message.ToString();
                }
            
                foreach (var property in modelProperties)
                {  
                    message.Append($"{property.Key}: {property.Value}{Environment.NewLine}");
                    var result = GetListValues(property.Value, 5);
                    message.Append(result.ToString(CultureInfo.InvariantCulture));
                }
            }
            catch
            {
                message.Append($"Error retrieving model properties. {Environment.NewLine}");
            }
            message.Append($"{Environment.NewLine}");
            return message.ToString();
        }

        #endregion

        #region GetUrlRequestInfo

        /// <summary>
        /// Get URL Request Info
        /// </summary>
        /// <returns></returns>
        private static string GetUrlRequestInfo(HttpContext? context)
        {
            var value = "";
            if (context == null)
            {
                return value;
            }

            var strB = new StringBuilder();
            strB.Append(Environment.NewLine);
            strB.Append("Request Info ------");

            var keyName = "ContentLength";
            try
            {
                value = context.Request.ContentLength.ToString();
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

            keyName = "Identity.IsAuthenticated";
            try
            {
                value = context.User.Identity?.IsAuthenticated.ToString();
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));
          

            keyName = "Identity.Name";
            try
            {
                value = context.User.Identity?.Name;
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

            
            keyName = "LocalIpAddress";
            try
            {
                value = context.Connection.LocalIpAddress?.ToString();
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

            keyName = "RemoteIpAddress";
            try
            {
                value = context.Connection.RemoteIpAddress?.ToString();
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));


            strB.Append(Environment.NewLine);
            return strB.ToString();
        }

        #endregion

        #region GetUrlMiscInfo

        /// <summary>
        /// Get Miscellaneous Information
        /// </summary>
        /// <returns></returns>
        private static string GetUrlMiscInfo(HttpContext? context)
        {
            var value = "";
            if (context == null)
            {
                return value;
            }

            var strB = new StringBuilder();


            try
            {
                strB.AppendFormat(ErrorWriteLine(nameof(context.Request.Protocol), context.Request.Protocol)); 
            }
            catch
            {
                // ignored
            }

            try
            {
                strB.AppendFormat(ErrorWriteLine(nameof(context.Request.Path), context.Request.Path)); 
                strB.AppendFormat(ErrorWriteLine(nameof(context.Request.QueryString), context.Request.QueryString)); 
            }
            catch
            {
                // ignored
            }

            strB.Append(Environment.NewLine);

           

            strB.Append(Environment.NewLine);

            var keyName = "Url.OriginalString";
            try
            {
                value = context.Request.GetDisplayUrl(); 
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

            // ReSharper disable once StringLiteralTypo
            keyName = "REMOTE_ADDR";
            try
            {
                value = context.GetServerVariable(keyName);
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

            keyName = "HTTP_X_FORWARDED_FOR";
            try
            {
                value = context.GetServerVariable(keyName);
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

           

            keyName = "MachineName";
            try
            {
                value = Environment.MachineName;
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

            keyName = "TimeStamp";
            try
            {
                // ReSharper disable once StringLiteralTypo
                value = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture) + " (Local Time)";
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));
            strB.Append(Environment.NewLine);

            keyName = "UTC TimeStamp";
            try
            {
                // ReSharper disable once StringLiteralTypo
                value = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss.ffffff", CultureInfo.InvariantCulture) + " UTC";
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));


            keyName = "Environment.Version";
            try
            {
                value = Environment.Version.ToString();
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

            keyName = "Environment.CurrentManagedThreadId";
            try
            {
                value = Environment.CurrentManagedThreadId.ToString();
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));


            keyName = "HttpContext.Current.User.Identity.Name";
            try
            {
                value = context.User.Identity?.Name;
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

            keyName = "Thread.CurrentPrincipal.Identity.Name";
            try
            {
                value = string.Empty;
                if (string.IsNullOrEmpty(Thread.CurrentPrincipal?.Identity?.Name) == false) 
                {
                    value = Thread.CurrentPrincipal.Identity.Name;
                }
                
            }
            catch (SecurityException)
            {
                value = PERMISSION_DENIED_MESSAGE;
            }
            catch
            {
                value = NO_ACCESS_MESSAGE;
            }
            strB.AppendFormat(ErrorWriteLine(keyName, value));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                keyName = "WindowsIdentity";
                try
                {
                    var windowsIdentity = WindowsIdentity.GetCurrent();
                    value = windowsIdentity.Name;
                }
                catch (SecurityException)
                {
                    value = PERMISSION_DENIED_MESSAGE;
                }
                catch
                {
                    value = NO_ACCESS_MESSAGE;
                }
                strB.AppendFormat(ErrorWriteLine(keyName, value));
            }


           

            strB.Append(Environment.NewLine);
            strB.Append(Environment.NewLine);
            return strB.ToString();
        }

        #endregion

        #region GetSessionDetail

        /// <summary>
        /// Use Reflection to get the object properties and values. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static string GetSessionDetails(string key, HttpContext? context)
        {
            var strB = new StringBuilder();
            if (context == null)
            {
                strB.AppendFormat(ErrorWriteLine(key, "HttpContext.Current is null."));
                return strB.ToString();
            }

            if (DoScrubValue(key))
            {
                strB.AppendFormat(ErrorWriteLine(key, "Removed due to security considerations."));
                return strB.ToString();
            }

            var obj = context.Session.GetString(key);

            // If there is no value and is null, write out key name then return.
            if (obj == null)
            {
                strB.AppendFormat(ErrorWriteLine(key, string.Empty));
                return strB.ToString();
            }

            // Handle for common types
            var type = obj.GetType();
            if (type.Name.ToLower() == "string" || type.Name.ToLower() == "int32" || type.Name.ToLower() == "boolean")
            {
                strB.AppendFormat(ErrorWriteLine(key, obj));
                return strB.ToString();
            }
            var properties = type.GetProperties();

            // If there are no properties, assume the session value is not an object. Use the main name and value.
            if (!properties.Any())
            {
                strB.AppendFormat(ErrorWriteLine(key, obj));
            }

            // If there are properties, assume it is an object.
            foreach (var part in properties)
            {
                var keyName = part.Name;
                if (DoScrubValue(keyName))
                {
                    strB.AppendFormat(ErrorWriteLine(keyName, "Removed due to security considerations."));
                    continue;
                }

                var info = obj.GetType().GetProperty(keyName);
                var value = "";
                if (info != null)
                {
                    var propertyValue = info.GetValue(obj, null);
                    if (propertyValue != null)
                    {
                        value = propertyValue.ToString();
                    }
                }
                strB.AppendFormat(ErrorWriteLine(keyName, value));
            }
            return strB.ToString();
        }


        #endregion

        #region GetServerVariables

        /// <summary>
        /// Get Server Variables
        /// </summary>
        /// <returns></returns>
        private static string GetServerVariables(HttpContext? context)
        {
            if (context == null)
            {
                return string.Empty;
            }
            
            var strB = new StringBuilder();
            strB.Append(Environment.NewLine);
            strB.Append("Server Variables ------");
            try
            {
                var keyNames = GetServerVariableKeyNames();
                if (keyNames.Count == 0)
                {
                    strB.AppendFormat(ErrorWriteLine("ServerVariables", "No values."));
                }

                foreach (var key in keyNames)
                {
                    string? value;
                    try
                    {
                        value = context.GetServerVariable(key);
                    }
                    catch (SecurityException)
                    {
                        value = PERMISSION_DENIED_MESSAGE;
                    }
                    catch
                    {
                        value = NO_ACCESS_MESSAGE;
                    }
                    strB.AppendFormat(ErrorWriteLine(key, value));
                }
            }
            catch (SecurityException)
            {
                strB.AppendFormat(ErrorWriteLine("ServerVariables", PERMISSION_DENIED_MESSAGE));
            }
            catch
            {
                strB.AppendFormat(ErrorWriteLine("ServerVariables", NO_ACCESS_MESSAGE));
            }

            strB.Append(Environment.NewLine);
            return strB.ToString();
        }

        #endregion

        #region GetSessionVariables

        /// <summary>
        /// Retrieve Session Variables
        /// </summary>
        /// <returns></returns>
        private static string GetSessionVariables(HttpContext? context)
        {
            var strB = new StringBuilder();
            strB.Append(Environment.NewLine);
            strB.Append("Session Variables ------");
            if (context == null)
            {
                strB.AppendFormat("HttpContext is null. No session found.");
                return strB.ToString();
            }

            try
            {
                var sessionKeys = context.Session.Keys.ToList();
                if (!sessionKeys.Any())
                {
                    strB.AppendFormat(ErrorWriteLine("Session", "No values."));
                }

                foreach (var key in sessionKeys)
                {
                    try
                    {
                        strB.Append(GetSessionDetails(key, context));
                    }
                    catch (SecurityException)
                    {
                        strB.AppendFormat(ErrorWriteLine(key, PERMISSION_DENIED_MESSAGE));
                    }
                    catch (Exception ex)
                    {
                        var message = NO_ACCESS_MESSAGE + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace + Environment.NewLine;

                        strB.AppendFormat(ErrorWriteLine(key, message));

                    }
                }
            }
            catch (SecurityException)
            {
                strB.AppendFormat(ErrorWriteLine("SessionVariables", PERMISSION_DENIED_MESSAGE));
            }
            catch
            {
                strB.AppendFormat(ErrorWriteLine("SessionVariables", NO_ACCESS_MESSAGE));
            }

            strB.Append(Environment.NewLine);
            return strB.ToString();
        }

        #endregion       

        #region GetServerVariableKeyNames

        /// <summary>
        /// In .Net Core, there doesn't appear to be a way to pull all the existing server variables without explicitly identifying the key name.
        /// </summary>
        /// <returns></returns>
        private static List<string> GetServerVariableKeyNames()
        {
            var keyNames = new List<string> 
                {
                    "APPL_PHYSICAL_PATH", 
                    "AUTH_USER",
                    "LOGON_USER",
                    "REMOTE_USER",
                    "LOCAL_ADDR",
                    "PATH_INFO",
                    "PATH_TRANSLATED",
                    "QUERY_STRING",
                    "REMOTE_ADDR",
                    "REMOTE_HOST",
                    "REMOTE_PORT",
                    "REQUEST_METHOD",
                    "SCRIPT_NAME",
                    "SERVER_NAME",
                    "SERVER_PORT",
                    "SERVER_PORT_SECURE",
                    "SERVER_PROTOCOL",
                    "SERVER_SOFTWARE",
                    "URL",
                    "HTTP_CONNECTION",
                    "HTTP_ACCEPT",
                    "HTTP_ACCEPT_ENCODING",
                    "HTTP_ACCEPT_LANGUAGE",
                    "HTTP_COOKIE",
                    "HTTP_HOST",
                    "HTTP_REFERER",
                    "HTTP_USER_AGENT",
                    "HTTP_X_REQUESTED_WITH",
                    "HTTP_SEC_FETCH_SITE",
                    "HTTP_SEC_FETCH_MODE",
                    "HTTP_SEC_FETCH_DEST"
                };

            return keyNames;
        }

        #endregion

        #region GetErrorString

        private static string GetErrorString(HttpContext? context, Exception? exception, Dictionary<string, string>? additionalInfo = null, string? message = null, IDictionary<string, object>? modelObjectDictionary = null)
        {
            var info = new StringBuilder();
            info.Append(Environment.NewLine);
            info.Append(Environment.NewLine);
            info.Append("------------------------------------------------");
            info.Append(Environment.NewLine);
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    info.Append(message);
                    info.Append(Environment.NewLine);
                }

                // Error Messages
                info = GetErrorMessages(info, exception);

                // Get Misc Info
                info.Append(GetUrlMiscInfo(context));

                // Record Info
                info = GetAdditionalInfo(info, additionalInfo);

                if (modelObjectDictionary?.Count > 0)
                {               
                    var modelMessage = GetDictionaryValues(modelObjectDictionary);
                    info.Append(modelMessage);
                }

                // SessionInfo
                info.Append(GetSessionVariables(context));

                // FormInfo
                info.Append(GetFormInfo(context));

                // Get ServerInfo
                info.Append(GetServerVariables(context));

                // Get RequestInfo
                info.Append(GetUrlRequestInfo(context));

                
            }
            catch
            {
                info.Append($"{Environment.NewLine}Error formatting exception.");
                info.Append(Environment.NewLine);
            }

            return info.ToString();
        }


        #endregion

        #endregion


    }
}
