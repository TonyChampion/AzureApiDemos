using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Xml;

namespace FrontDoorAndCaching.HealthChecks
{
    public static class HealthCheckHelper
    {
        public static Task WriteResponse(HttpContext httpContext, HealthReport result)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (httpContext.Request.Method == HttpMethods.Head)
            {
                return Task.CompletedTask;
            }

            httpContext.Response.ContentType = "application/json";

            var json = new JObject(
                new JProperty("status", result.Status.ToString()),
                new JProperty("version", Assembly.GetExecutingAssembly().GetName().Version.ToString()));

            if (result.Entries != null && result.Entries.Count > 0)
            {
                foreach (KeyValuePair<string, HealthReportEntry> entry in result.Entries)
                {
                    HealthReportEntry healthReport = entry.Value;
                    var entryJson = new JObject();
                    if (healthReport.Data != null && healthReport.Data.Count > 0)
                    {
                        if (healthReport.Data.Count > 1)
                        {
                            var dataJson = new JObject();
                            foreach (KeyValuePair<string, object> dataKvp in healthReport.Data)
                            {
                                dataJson.Add(new JProperty(dataKvp.Key, JObject.Parse(dataKvp.Value.ToString())));
                            }

                            entryJson.Add(new JProperty(nameof(healthReport.Data), dataJson));
                        }
                        else
                        {
                            var firstData = healthReport.Data.Values.First();
                            entryJson = JObject.Parse(firstData.ToString());
                        }
                    }
                    else
                    {
                        entryJson.Add(new JProperty(nameof(result.Status).ToLowerInvariant(), healthReport.Status.ToString()));
                    }

                    if (healthReport.Exception != null)
                    {
                        entryJson.Add(new JProperty(nameof(healthReport.Exception),
                            healthReport.Exception.ToString()));
                    }

                    json.Add(new JProperty(entry.Key, entryJson));
                }
            }

            return httpContext.Response.WriteAsync(
                json.ToString(Newtonsoft.Json.Formatting.Indented));
        }

    }
}
