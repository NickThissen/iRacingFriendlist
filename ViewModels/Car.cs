using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRacingFriendlist.Http;
using Newtonsoft.Json.Linq;

namespace iRacingFriendlist.ViewModels
{
    public class Car
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public static List<Car> ParseJson(string json)
        {
            var cars = new List<Car>();

            var array = JArray.Parse(json);
            foreach (dynamic row in array)
            {
                var car = new Car();
                car.Id = Convert<int>(row["id"]);
                car.Name = HttpUtility.UrlDecode(Convert<string>(row["name"]));
                car.ShortName = HttpUtility.UrlDecode(Convert<string>(row["abbrevname"]));
                cars.Add(car);
            }

            return cars;
        }

        private static T Convert<T>(dynamic obj, T defaultValue = default(T))
        {
            if (obj == null) return defaultValue;
            return (T)obj;
        }
    }
}
