using Newtonsoft.Json;
using System.Collections.Generic;
using CertificateManager.Entities.Interfaces;
using System.Linq;
using System;

namespace CertificateManager.Logic.UXLogic
{
    public class DataRenderingProvider
    {
        public string RenderList(string variable, object data)
        {
            return string.Format("var {0} = {1}", variable, JsonConvert.SerializeObject(data));
        }

        public string Render2dChartData(string xvar, string yvar, List<I2dChartData> data)
        {
            string x = string.Format("var {0} = {1};", xvar, JsonConvert.SerializeObject(data.Select(item => item.XAxis)));
            string y = string.Format("var {0} = {1};", yvar, JsonConvert.SerializeObject(data.Select(item => item.YAxis)));

            return string.Concat(x, Environment.NewLine, y);
        }
    }
}
