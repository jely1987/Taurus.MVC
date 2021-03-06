using CYQ.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Taurus.Core
{
    /// <summary>
    /// 路由系统
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// 配置模式【值为0,1或2】[默认为1]
        /// 值为0：匹配{Action}/{Para}
        /// 值为1：匹配{Controller}/{Action}/{Para}
        /// 值为2：匹配{Module}/{Controller}/{Action}/{Para}
        /// </summary>
        public static int RouteMode
        {
            get { return AppConfig.GetAppInt(AppSettings.RouteMode, 1); }
            set { AppConfig.SetApp(AppSettings.RouteMode, value.ToString()); }
        }
    }
}
