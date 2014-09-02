using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRacingFriendlist.ViewModels
{
    public class DriverCollection : ObservableCollection<Driver>
    {
        public DriverCollection()
        {
        }

        public DriverCollection(IEnumerable<Driver> drivers) : this()
        {
            this.AddRange(drivers);
        }

        public void AddRange(IEnumerable<Driver> drivers)
        {
            foreach (var driver in drivers)
            {
                this.Add(driver);
            }
        }

        public void Set(IEnumerable<Driver> drivers)
        {
            this.Clear();
            this.AddRange(drivers);
        }

        public Driver FromId(int custid)
        {
            return this.SingleOrDefault(d => d.CustId == custid);
        }

        public DriverCollection InSession()
        {
            var drivers = this.Where(d => d.OnlineStatus == Driver.OnlineStatuses.InSession);
            return new DriverCollection(drivers);
        }

        public DriverCollection Online()
        {
            var drivers = this.Where(d => d.OnlineStatus == Driver.OnlineStatuses.Online);
            return new DriverCollection(drivers);
        }

        public DriverCollection Offline()
        {
            var drivers = this.Where(d => d.OnlineStatus == Driver.OnlineStatuses.Offline);
            return new DriverCollection(drivers);
        }

        public bool Contains(int custid)
        {
            return this.Any(d => d.CustId == custid);
        }

        public new bool Contains(Driver driver)
        {
            return Contains(driver.CustId);
        }
    }
}
