using Microsoft.Phone.Reactive;
using MyNotebookCustomControls.Weather;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace MyNotebookCustomControls.TimeAndWeather
{
    [TemplatePart(Name = StartIconName, Type = typeof(Image))]
    [TemplatePart(Name = EndIconName, Type = typeof(Image))]
    public class Weather : Control
    {
        //(location=38.53,115.28)(经度， 纬度)保定的经纬度
        const string GetCityNameUrl = "http://api.map.baidu.com/geocoder?location={0},{1}&id={2}";
        //const string GetCityNameUrl = "http://api.map.baidu.com/geocoder?location=38.53,115.28&id={2}";
        public const string StartIconName = "imgStart";
        public const string EndIconName = "imgEnd";

        Image imgStart, imgEnd;


        #region constructor

        public Weather()
        {
            DefaultStyleKey = typeof(Weather);
            GetWeatherInfo();


            var ob = Observable.GenerateWithTime(0, x => true, x => x, x => TimeSpan.FromMinutes(30), x => x + 1);
            ob.ObserveOnDispatcher().Subscribe(x =>
                {
                    GetWeatherInfo();
                });
        }

        #endregion

        #region GetWeatherInfo

        /*
            <GeocoderSearchResponse>
              <status>OK</status>
              <result>
                <location>
                  <lat>-71.988531</lat>
                  <lng>47.669444</lng>
                </location>
                <formatted_address></formatted_address>
                <business></business>
                <addressComponent>
                  <streetNumber></streetNumber>
                  <street></street>
                  <district></district>
                  <city></city>
                  <province></province>
                </addressComponent>
                <cityCode>0</cityCode>
              </result>
            </GeocoderSearchResponse>
         */

        protected void GetWeatherInfoHandler(double latitude, double longitude)
        {
            string url = String.Format(GetCityNameUrl, latitude, longitude, Guid.NewGuid().ToString("n"));
            Debug.WriteLine("Longitude:{0}, Latitude:{1}", longitude, latitude);

            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Method = "Get";

            Func<IObservable<WebResponse>> func = Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse);
            var result = func();
            result.ObserveOnDispatcher().Subscribe(response =>
            {
                Stream stream = response.GetResponseStream();
                if (null != stream)
                {
                    XDocument root = XDocument.Load(stream);
                    var cityInfo = from city in root.Descendants("addressComponent")
                                   select new
                                   {
                                       CityName = city.Element("city").Value,
                                       Province = city.Element("province").Value
                                   };

                    tempCityName = cityInfo.FirstOrDefault().CityName.TrimEnd('市');
                    Province = cityInfo.FirstOrDefault().Province.TrimEnd('省').TrimEnd('市');

                    if (String.IsNullOrEmpty(tempCityName.Trim()))
                    {
                        this.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        WeatherWSSoapClient client = new WeatherWSSoapClient();
                        var obWeather = Observable.FromEvent<getWeatherCompletedEventArgs>(client, "getWeatherCompleted").Select(weather => weather.EventArgs);
                        var obProvince = Observable.FromEvent<getRegionProvinceCompletedEventArgs>(client, "getRegionProvinceCompleted").Select(p => p.EventArgs);
                        var obCity = Observable.FromEvent<getSupportCityStringCompletedEventArgs>(client, "getSupportCityStringCompleted").Select(p => p.EventArgs);

                        obProvince.ObserveOnDispatcher().Subscribe(p =>
                        {
                            if (null == provinces)
                                provinces = new Dictionary<string, string>();

                            foreach (string info in p.Result)
                            {
                                string[] arr = info.Split(new string[] { "," }, StringSplitOptions.None);
                                provinces.Add(arr[0], arr[1]);
                            }

                            if (provinces.ContainsKey(Province))
                            {
                                client.getSupportCityStringAsync(provinces[Province]);
                            }
                        });

                        obCity.ObserveOnDispatcher().Subscribe(city =>
                        {
                            var ci = from c in city.Result
                                     where c.Split(new string[] { "," }, StringSplitOptions.None)[0].Equals(tempCityName)
                                     select c.Split(new string[] { "," }, StringSplitOptions.None)[1];

                            client.getWeatherAsync(ci.FirstOrDefault(), String.Empty);
                        });

                        obWeather.ObserveOnDispatcher().Subscribe(weather =>
                        {
                            if (null == weather || null != weather.Error)
                            {
                                this.Visibility = Visibility.Collapsed;
                            }

                            Temperature = weather.Result[8];
                            WeatherInfo = weather.Result[7].Split(new string[] { " " }, StringSplitOptions.None)[1];
                            WeatherIconStart = weather.Result[10];
                            WeatherIconEnd = weather.Result[11];
                            CityName = tempCityName;

                            ShownAnimation();
                        },
                        exception =>
                        {
                            this.Visibility = Visibility.Collapsed;
                        });

                        if (null == provinces)
                        {
                            client.getRegionProvinceAsync();
                        }
                        else
                        {
                            if (provinces.ContainsKey(Province))
                            {
                                client.getSupportCityStringAsync(provinces[Province]);
                            }
                        }
                    }
                }
            },
                exception =>
                {
                    this.Visibility = Visibility.Collapsed;
                });
        }

        Dictionary<string, string> provinces;
        string tempCityName;

        protected async void GetWeatherInfo()
        {
            Geolocator locator = new Geolocator();
            Geoposition position = await locator.GetGeopositionAsync(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1));            
            GetWeatherInfoHandler(position.Coordinate.Latitude, position.Coordinate.Longitude);
        }

        void locator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            var ob = Observable.Return(args.Position).Throttle(TimeSpan.FromMinutes(0.5));
            ob.ObserveOnDispatcher().Subscribe(x =>
            {
                GetWeatherInfoHandler(x.Coordinate.Latitude, x.Coordinate.Longitude);
            });
        }

        #endregion

        #region CityName

        public static readonly DependencyProperty CityNameProperty = DependencyProperty.Register("CityName", typeof(string), typeof(Weather), new PropertyMetadata(default(string)));

        public string CityName
        {
            get
            {
                return GetValue(CityNameProperty) as string;
            }
            set
            {
                SetValue(CityNameProperty, value);
            }
        }

        #endregion

        #region Province

        public static readonly DependencyProperty ProvinceProperty = DependencyProperty.Register("Province", typeof(string), typeof(Weather), new PropertyMetadata(default(string)));

        public string Province
        {
            get
            {
                return GetValue(ProvinceProperty) as string;
            }
            set
            {
                SetValue(ProvinceProperty, value);
            }
        }

        #endregion

        #region Temperature

        public static readonly DependencyProperty TemperatureProperty = DependencyProperty.Register("Temperature", typeof(string), typeof(Weather), new PropertyMetadata(default(string)));

        public string Temperature
        {
            get
            {
                return GetValue(TemperatureProperty) as string;
            }
            set
            {
                SetValue(TemperatureProperty, value);
            }
        }

        #endregion

        #region WeatherInfo

        public static readonly DependencyProperty WeatherInfoProperty = DependencyProperty.Register("WeatherInfo", typeof(string), typeof(Weather), new PropertyMetadata(default(string)));

        public string WeatherInfo
        {
            get
            {
                return GetValue(WeatherInfoProperty) as string;
            }
            set
            {
                SetValue(WeatherInfoProperty, value);
            }
        }

        #endregion

        #region WeatherIconStart

        public static readonly DependencyProperty WeatherIconStartProperty = DependencyProperty.Register("WeatherIconStart", typeof(string), typeof(Weather), new PropertyMetadata(default(string), WeatherIconChangedHandler));

        public string WeatherIconStart
        {
            get
            {
                return GetValue(WeatherIconStartProperty) as string;
            }
            set
            {
                SetValue(WeatherIconStartProperty, value);
            }
        }

        #endregion

        #region WeatherIconEnd

        public static readonly DependencyProperty WeatherIconEndProperty = DependencyProperty.Register("WeatherIconStart", typeof(string), typeof(Weather), new PropertyMetadata(default(string), WeatherIconChangedHandler));

        public string WeatherIconEnd
        {
            get
            {
                return GetValue(WeatherIconEndProperty) as string;
            }
            set
            {
                SetValue(WeatherIconEndProperty, value);
            }
        }

        #endregion

        #region WeatherIconChangedHandler

        protected static void WeatherIconChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Weather).ChangedIcon();
        }

        #endregion

        #region OnApplyTemplate

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            imgStart = GetTemplateChild(StartIconName) as Image;
            imgEnd = GetTemplateChild(EndIconName) as Image;
        }

        #endregion

        #region ChangedIcon

        /*
 * 
未知 nothing.gif 
晴 0.gif 
多云 1.gif 
阴 2.gif 
阵雨 3.gif 
雷阵雨 4.gif 
雷阵雨并伴有冰雹 5.gif 
雨夹雪 6.gif 
小雨 7.gif 
中雨 8.gif 
大雨 9.gif 
暴雨 10.gif 
大暴雨 11.gif 
特大暴雨 12.gif 
阵雪 13.gif 
小雪 14.gif 
中雪 15.gif 
大雪 16.gif 
暴雪 17.gif 
雾 18.gif 
冻雨 19.gif 
沙尘暴 20.gif 
小雨-中雨 21.gif 
中雨-大雨 22.gif 
大雨-暴雨 23.gif 
暴雨-大暴雨 24.gif 
大暴雨-特大暴雨 25.gif 
小雪-中雪 26.gif 
中雪-大雪 27.gif 
大雪-暴雪 28.gif 
浮尘 29.gif 
扬沙 30.gif 
强沙尘暴 31.gif 
小到中雨 21.gif 
中到大雨 22.gif 
大到暴雨 23.gif 
小到中雪 26.gif 
中到大雪 27.gif 
大到暴雪 28.gif 
小阵雨 3.gif 
 * */

        protected void ChangedIcon()
        {
            if ((!String.IsNullOrEmpty(WeatherIconStart) && !String.IsNullOrEmpty(WeatherIconEnd)) &&
                (null != imgStart && null != imgEnd))
            {
                string formatter = "/MyNotebookAssets;component/Assets/Weather/{0}";
                imgStart.Source = new BitmapImage(new Uri(String.Format(formatter, WeatherIconStart), UriKind.Relative));
                imgEnd.Source = new BitmapImage(new Uri(String.Format(formatter, WeatherIconEnd), UriKind.Relative));
            }
        }

        #endregion

        #region ShownAnimation

        private void ShownAnimation()
        {
            Storyboard storyboard = new Storyboard();

            DoubleAnimation f = new DoubleAnimation();
            f.From = 0;
            f.To = 1;
            f.Duration = TimeSpan.FromSeconds(1);

            Storyboard.SetTarget(f, this);
            Storyboard.SetTargetProperty(f, new PropertyPath("UIElement.Opacity"));
            storyboard.Children.Add(f);
            storyboard.Begin();
        }

        #endregion
    }
}
