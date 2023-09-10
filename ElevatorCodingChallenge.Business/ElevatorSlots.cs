using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Elevator.Wpf
{
    public class ElevatorSlots : IDisposable
    {
        private readonly int maxNumberPeople;
        // In meters per second.
        private readonly int elevatorSpeed;
        private readonly ElevatorType elevatorType;
        private readonly System.Timers.Timer timer;
        private readonly List<ElevatorFloorCalls> floorCalls;
        private readonly List<ElevatorSlotsFloors> elevatorSlotsFloors;

        
        private int CurrentElevatorHeight { get; set; }

        public ElevatorSlots(int maxNumberPeople, int elevatorSpeed, ElevatorType elevatorType, int numberOfFloors, int floorHeight, int betweenFloorHeight)
        {
            this.maxNumberPeople = maxNumberPeople;
            this.elevatorSpeed = elevatorSpeed;
            this.elevatorType = elevatorType;
            this.timer = new(interval: 1000);

            this.elevatorSlotsFloors = new List<ElevatorSlotsFloors>();

            for (int currentFloorCount = 0; currentFloorCount < numberOfFloors; currentFloorCount++)
            {
                this.elevatorSlotsFloors.Add(new ElevatorSlotsFloors(
                    FloorStartHeight: (floorHeight + betweenFloorHeight) * currentFloorCount, 
                    FloorEndHeight: (betweenFloorHeight * currentFloorCount) + floorHeight));
            }
        }


        public void AddElevatorFloorCall(ElevatorFloorCalls floorCall)
        {
            floorCall.Added = DateTime.Now;
            this.floorCalls.Add(floorCall);
        }
        public void GoUpIncremenet()
        {            
            ///if(this.floorCalls.Count > 0 && this.floorCalls.Where(f => !f.WasFullfilled)
            //    .OrderBy(d => d.Added).First().ElevatorFloorCallNumber
        //    {
        //
           // }
           // CurrentElevatorHeight = CurrentElevatorHeight + elevatorSpeed;
        }

        public void GoDownIncremenet()
        {
            CurrentElevatorHeight = CurrentElevatorHeight - elevatorSpeed;
        }


        public void StartElevator()
        {
           timer.Elapsed += (sender, e) => HandleTimer();
           timer.Start();
        }

        private void HandleTimer()
        {
            
        }



        public void Dispose()
        {
            this.timer.Stop();
            this.timer.Dispose();
        }


        public class ElevatorFloorCalls
        {
            public DateTime Added { get; set; }
            public int ElevatorFloorCallNumber { get; set; }
            public bool WasFullfilled { get; set; }

        }

        public class ElevatorSlotsFloors
        {
            public ElevatorSlotsFloors(int FloorStartHeight, int FloorEndHeight)
            {
                this.FloorStartHeight = FloorStartHeight;
                this.FloorEndHeight = FloorEndHeight; 
            }
            public readonly int FloorEndHeight;
            public readonly int FloorStartHeight;
            
        }
    }
}
