using System;

namespace TaxiBookingApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            InputHandler input = new InputHandler();
            
            
            ITaxiService service = new TaxiService(4);
            ITaxiFactory factory = new TaxiFactory(input,service);
            TaxiOperation operation = new TaxiOperation(input,factory,service);
            operation.Start();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingApplication
{
    internal class InputHandler
    {
        private readonly List<char> positions = new List<char>
        {
            'A','B','C','D','E','F'
        };
        public char CheckPickingPoint()
        {
            foreach(char c in positions)
            {
                Console.Write(c + ",");
            }
            bool isContinue = true;
            while (isContinue)
            {
                try
                {
                    Console.WriteLine("Please Select the Above PickingPoint");
                    char input = Console.ReadLine().ToUpper()[0];
                    if (positions.Contains(input))
                    {
                        return input;
                    }
                    Console.WriteLine("Invalid Position");
                }
                catch (Exception ex) { Console.WriteLine("Invalid Position"); }
            }
            return '\0';
        }

        public char CheckDropingPoint(char PickingPoint)
        {
            foreach(char c in positions)
            {
                if(c == PickingPoint)
                {
                    continue;
                }
                Console.Write(c + ",");
            }
            bool isContinue = true;
            while (isContinue)
            {
                try
                {
                    Console.WriteLine("Please Select the Above Droping Point");
                    char input = Console.ReadLine().ToUpper()[0];
                    if (positions.Contains(input) && input != PickingPoint)
                    {
                        return input;
                    }
                    Console.WriteLine("Invalid Position");
                }
                catch (Exception ex) { Console.WriteLine("Invalid Position"); }
            }
            return '\0';
        }

        public DateTime? CheckPickingTime()
        {
            bool isContinue = true;
            while (isContinue)
            {
                try
                {
                    Console.WriteLine("Enter pickup time in 24-hour format (e.g., 14:00):");
                    string? input = Console.ReadLine();
                    if (DateTime.TryParseExact(input, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                    {
                        return result;
                    }
                    Console.WriteLine("Invalid time format. Use HH:mm (e.g., 09:00, 23:30)");
                }
                catch (Exception ex) { Console.WriteLine("Invalid Time"); }
            }
            return null;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingApplication
{
    internal class TaxiOperation
    {
        private readonly InputHandler input;
        private readonly ITaxiFactory taxiFactory;
        private readonly ITaxiService taxiService;
        public TaxiOperation(InputHandler input,ITaxiFactory taxiFactory,ITaxiService taxiService)
        {
            this.input = input;
            this.taxiFactory = taxiFactory;
            this.taxiService = taxiService;
        }
        public void Start()
        {
            try
            {
                bool isContinue = true;
                while (isContinue)
                {
                    Console.WriteLine("\nPress 1 : Taxi Book\nPress 2 : Taxi Detail\nPress 3 : Exit\nEnter Your Choise");
                    string? input = Console.ReadLine();
                    switch (input)
                    {
                        case "1":
                            taxiFactory.BookTaxi();
                            break;
                        case "2":
                            taxiService.DisplayTaxiDetail();
                            break;
                        case "3":
                            isContinue = false;
                            break;
                        default:
                            Console.WriteLine("Please Enter the Valid Option");
                            break;
                    }
                }
            }
            catch(Exception ex) {  Console.WriteLine(ex.Message); }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingApplication
{
    internal interface ITaxiFactory
    {
        void BookTaxi();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingApplication
{
    internal class TaxiFactory : ITaxiFactory
    {
        private readonly InputHandler input;
        public readonly ITaxiService taxiService;
        private int BookingId = 1;
        public TaxiFactory(InputHandler input, ITaxiService taxiService)
        {
            this.input = input;
            this.taxiService = taxiService;
        }
        public void BookTaxi()
        {
            int id = BookingId++;
            char pickingPoint = input.CheckPickingPoint();
            if (pickingPoint == '\0')
            {
                Console.WriteLine("Try Again");
                return;
            }
            char DropingPoint = input.CheckDropingPoint(pickingPoint);
            if (DropingPoint == '\0')
            {
                Console.WriteLine("Try Again");
                return;
            }
            DateTime? pickingTime = input.CheckPickingTime();
            if (pickingTime == null)
            {
                Console.WriteLine("Try Again");
                return;
            }
            taxiService.Booking(pickingPoint,DropingPoint, pickingTime.Value);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingApplication
{
    internal interface ITaxiService
    {
        void Booking(char pickPosition, char dropPosition, DateTime pickTime);
        int GetDistance(char CurreCurrentPosition, char pickPosition);
        void DisplayTaxiDetail();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingApplication
{
    internal class TaxiService : ITaxiService
    {
        private List<Taxi> taxis = new List<Taxi>();
        private int taxisId = 1;
        public TaxiService(int numberOfTaxi)
        {
            try
            {
                for(int i = 1;i <= numberOfTaxi;i++)
                {
                    taxis.Add(new Taxi
                    {
                        TaxiID = taxisId++
                    });
                }
            }catch (Exception ex) { Console.WriteLine("Error :" +ex.Message); }
        }

        public void DisplayTaxiDetail()
        {
            if(taxis.Count == 0)
            {
                Console.WriteLine("No Taxi is found");
            }
            else
            {
                foreach(Taxi taxi in taxis)
                {
                    Console.WriteLine($"Taxi id - {taxi.TaxiID}\tCurrent Position of Taxi - {taxi.CurrentPosition}\tFree Time of Taxi - {taxi.FreeTime}\tTotal Earnings - {taxi.Earnings}");
                    if (taxi.TripHistroy.Count != 0)
                    {
                        Console.WriteLine($"History of taxi id {taxi.TaxiID}");
                        foreach (string history in taxi.TripHistroy)
                        {
                            Console.WriteLine(history);
                        }
                    }
                    else { Console.WriteLine("No Histroy Found"); }
                }
            }
        }

        public void Booking(char pickPosition, char dropPosition, DateTime pickTime)
        {
            var avaliableTaxi = taxis.Where(t => t.FreeTime.AddHours(GetDistance(t.CurrentPosition, pickPosition)) <= pickTime)
                                     .OrderBy(t => GetDistance(t.CurrentPosition, pickPosition))
                                     .ThenBy(t => t.Earnings)
                                     .ToList();
            if (avaliableTaxi.Count == 0)
            {
                Console.WriteLine("No Taxi is avaiable on this time");
                return;
            }
            else
            {
                var bookedTaxi = avaliableTaxi.FirstOrDefault();
                char oldPosition = bookedTaxi.CurrentPosition;
                int pickingDistance = GetDistance(bookedTaxi.CurrentPosition, pickPosition) * 15;
                int travelDistance = GetDistance(pickPosition, dropPosition) * 15;
                TimeSpan travelTime = TimeSpan.FromHours(GetDistance(pickPosition, dropPosition));
                DateTime dropingTime = pickTime.Add(travelTime);
                int earningOfRent = 100 + ((travelDistance - 5) * 10);

                // updated 
                bookedTaxi.CurrentPosition = dropPosition;
                bookedTaxi.Earnings += earningOfRent;
                bookedTaxi.FreeTime = dropingTime;
                bookedTaxi.TripHistroy.Add($"From {pickPosition} to {dropPosition} | Start: {pickTime:HH:mm} | Drop: {dropingTime:HH:mm} | Earned: Rs.{earningOfRent}");
                Console.WriteLine("Taxi Booked Successfully");
            }
        }
        public int GetDistance(char CurreCurrentPosition, char pickPosition)
        {
            return Math.Abs(CurreCurrentPosition - pickPosition);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiBookingApplication
{
    internal class Taxi
    {
        public int TaxiID {get;set;}
        public char CurrentPosition { get;set;} = 'A';
        public double Earnings { get; set; } = 0;
        public DateTime FreeTime { get; set; } = DateTime.MinValue;
        public List<string> TripHistroy { get; set; } = new List<string>();
    }
}
