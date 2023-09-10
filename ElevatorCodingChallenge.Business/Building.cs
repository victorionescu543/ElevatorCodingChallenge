using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;


namespace Elevator.Wpf
{
    public class Building : IDisposable
    {
        private readonly System.Threading.Timer timer;

        private readonly List<ElevatorSlots> elevatorSlots;


        private readonly int numberOfFloors;
        public Building(int numberOfFloorsParam)
        {
            if(numberOfFloorsParam <= 0)
                throw new ArgumentException("The numberOfFloorsParam must be greater than 0");

            this.numberOfFloors = numberOfFloorsParam;
            this.elevatorSlots = new List<ElevatorSlots>();
        }
        public void AddElevatorSlot(int speed, int maxPeopleInElevator, ElevatorType elevatorType)
        {
           // this.elevatorSlots.Add();
        }
        public void Build() 
        {
            if(elevatorSlots.Count == 0)
            {
                throw new ArgumentException("You need add atleast 1 elevator slot, use method 'AddElevatorSlot'");
            }
            System.Timers.Timer timer = new(interval: 1000);
            timer.Elapsed += (sender, e) => HandleTimer();
            timer.Start();
        }

      

        private void HandleTimer()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            timer.Dispose();

        }


    }
}
