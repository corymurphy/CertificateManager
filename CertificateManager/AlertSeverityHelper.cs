using CertificateManager.Entities;
using Microsoft.AspNetCore.Html;

namespace CertificateManager
{
    public static class AlertSeverityHelper
    {
        public static IHtmlContent AlertSeverityHtmlString(ConfigurationAlert alert)
        {

            string html = @"<span class='{#css-class#}'>
                            <strong>
                                <i class='fa {#icon-class#}'></i>
                                {#AlertSeverity#}
                            </strong>
                        </span>";

            if(alert.AlertSeverity == Entities.Enumerations.AlertSeverity.Error)
            {
                html = html.Replace("{#css-class#}", "text-danger");
                html = html.Replace("{#icon-class#}", "fa-exclamation-triangle");
            }
            else
            {
                html = html.Replace("{#css-class#}", "text-success");
                html = html.Replace("{#icon-class#}", "fa-info");
            }

            html = html.Replace("{#AlertSeverity#}", alert.AlertSeverity.ToString());

            return new HtmlString(html);
        }

    }
}
