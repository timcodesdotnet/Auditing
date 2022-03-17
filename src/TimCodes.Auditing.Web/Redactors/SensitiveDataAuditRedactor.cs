using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TimCodes.Auditing.Web.Redactors;

/// <summary>
/// Redacts sensitive information from an audit scope based on <see cref="AuditConfiguration.SensitiveFields"/>
/// </summary>
public class SensitiveDataAuditRedactor : IAuditRedactor
{
    protected virtual string ReplaceRequestJsonData(string json) =>
        //Regex is looking for strings in the pattern "<one of the sensitive fields>" : "<some value>" and replaces the value with the redact token
        Regex.Replace(json, $"\"({string.Join("|", AuditConfiguration.SensitiveFields)})\"\\s?:\\s?\"((\\\"|.)*?)\"(,?)", "\"$1\": \"" + Redacting.RedactToken + "\"$4", RegexOptions.IgnoreCase);

    protected virtual string ReplaceResponseJsonData(string json) =>
        //Regex is looking for strings in the pattern "<one of the sensitive fields>" : "<some value>" and replaces the value with the redact token
        Regex.Replace(json, @$"\""({string.Join("|", AuditConfiguration.SensitiveFields)})\""\s?:\s?\""((\""|.)*?)\""(,?)", @"""$1"": """ + Redacting.RedactToken + @"""$4", RegexOptions.IgnoreCase);

    protected virtual string ReplaceQueryData(string query) =>
        //Regex is looking for strings in the pattern <one of the sensitive fields>=<some value> and replaces the value with the redact token
        Regex.Replace(query, $"({string.Join("|", AuditConfiguration.SensitiveFields)})=(.*?)(&|$)", "$1=" + Redacting.RedactToken + "$3", RegexOptions.IgnoreCase);

    public bool Redact(IAuditScope scope, RedactMode mode)
    {
        if (mode == RedactMode.Loose) return false;

        var apiEvent = scope.Event as AuditEventWebApi;
        if (apiEvent?.Action != null)
        {
            if (apiEvent.Action.RequestBody?.Value != null)
            {
                if (apiEvent.Action.RequestBody.Value is string body)
                {
                    body = ReplaceRequestJsonData(body);
                    apiEvent.Action.RequestBody.Value = ReplaceQueryData(body);
                }
            }

            if (apiEvent.Action.ResponseBody?.Value != null)
            {
                if (nameof(JsonResult) == apiEvent.Action.ResponseBody.Type ||
                    nameof(OkObjectResult) == apiEvent.Action.ResponseBody.Type)
                {
                    var value = apiEvent.Action.ResponseBody.Value;
                    if (value is OkObjectResult ok) value = ok.Value;
                    if (value is JsonResult json) value = json.Value;

                    try
                    {
                        var responseBody = JsonSerializer.Serialize(value);
                        apiEvent.Action.ResponseBody.Value = ReplaceResponseJsonData(responseBody);
                    }
                    catch (Exception e)
                    {
                        apiEvent.Action.ResponseBody.Value = $"Error serializing: {e.Message} {Redacting.RedactToken}";
                    }
                }
            }

            apiEvent.Action.RequestUrl = ReplaceQueryData(apiEvent.Action.RequestUrl);
            return true;
        }

        return false;
    }
}
