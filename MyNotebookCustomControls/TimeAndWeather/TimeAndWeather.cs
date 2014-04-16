using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyNotebookCustomControls.TimeAndWeather
{
    public class TimeAndWeather : Control
    {
        //(38.53,115.28)(经度， 纬度)保定的经纬度
        const string GetCityNameUrl = "http://api.map.baidu.com/geocoder?location={0},{1}&id={3}";

        #region constructor

        public TimeAndWeather()
        {
            DefaultStyleKey = typeof(TimeAndWeather);
        }

        #endregion        
    }
}
