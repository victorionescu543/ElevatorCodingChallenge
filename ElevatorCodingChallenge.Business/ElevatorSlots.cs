using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Elevator.Wpf
{
    public class ElevatorSlots : IDisposable
    {
       
        // In meters per second.
        private readonly int elevatorSpeed;        
        private readonly System.Timers.Timer timer;
        private readonly List<ElevatorFloorCalls> floorCalls;
        private readonly List<ElevatorSlotsFloors> elevatorSlotsFloors;
        private readonly int maxFloorHeight;
        public  ElevatorState ElevatorState { get; private set; }
        
        public int CurrentElevatorHeight { 
            get; 
            private set; }
        //This is an ID mostly for debugging.
        public readonly string ElevatorId;
        public readonly int MaxNumberPeople;
        public int CurrentCapacityOfPeople { get; private set; }

        public ElevatorSlots(int maxFloorHeight, string elevatorId, int maxNumberPeople, int elevatorSpeed, int numberOfFloors, int floorHeight, int betweenFloorHeight)
        {
            this.MaxNumberPeople = maxNumberPeople;
            this.CurrentCapacityOfPeople = 0;
            this.elevatorSpeed = elevatorSpeed;
            this.ElevatorState = ElevatorState.Idle;
            this.timer = new(interval: 1000);

            this.elevatorSlotsFloors = new List<ElevatorSlotsFloors>();

            for (int currentFloorCount = 0; currentFloorCount < numberOfFloors; currentFloorCount++)
            {
                this.elevatorSlotsFloors.Add(new ElevatorSlotsFloors(
                    FloorStartHeight: (floorHeight + betweenFloorHeight) * currentFloorCount, 
                    FloorEndHeight: (betweenFloorHeight * currentFloorCount) + floorHeight));
            }
            floorCalls = new List<ElevatorFloorCalls>();
            this.ElevatorId = elevatorId;
            this.maxFloorHeight = maxFloorHeight;
        }


        public void AddElevatorFloorCall(ElevatorFloorCalls floorCall)
        {
            floorCall.Added = DateTime.Now;
            this.floorCalls.Add(floorCall);
        }
        public void GoUpIncremenet()
        {
            Console.WriteLine($"{this.ElevatorId} going up");
            if(this.CurrentElevatorHeight >= this.maxFloorHeight)
            {
                this.CurrentElevatorHeight = this.maxFloorHeight;
                this.ElevatorState = ElevatorState.Idle;
            }
            else
            {
                this.CurrentElevatorHeight = CurrentElevatorHeight + elevatorSpeed;
            }
           
        }

        public void GoDownIncremenet()
        {
            Console.WriteLine($"{this.ElevatorId} going down");
            this.CurrentElevatorHeight = CurrentElevatorHeight - elevatorSpeed;   
            if(this.CurrentElevatorHeight <= 0)
            {
                this.CurrentElevatorHeight = 0;
                this.ElevatorState = ElevatorState.Idle;
            }
        }


        
        public void StartElevator()
        {
         //  timer.Elapsed += (sender, e) => HandleTimer();
         //  timer.Start();
        }

        public void HandleTimer()
        {
            bool adjustedMovementInLoop = false;
         
            // here we want to see if we can let people out elevator.
            if (this.floorCalls.Count > 0)
            {
                // we do order by ElevatorFloorHeightToStop, so that would be 10, 40, 70 and so on.
                var orderedElevatorFloorCalls = this.ElevatorState == ElevatorState.MovingUp ? this.floorCalls.OrderBy(g => g.ElevatorFloorHeightToStopToStepOut).ToList() :
                    this.floorCalls.OrderByDescending(g => g.ElevatorFloorHeightToStopToStepOut).ToList();
                foreach (var curFloorCall in orderedElevatorFloorCalls.ToList())
                {
                    //elevator arrived exactly where the current floor needs to be
                    if (curFloorCall.ElevatorFloorHeightToStopToStepOut == this.CurrentElevatorHeight)
                    {
                        // we can claim that in here, people went out of the elevator. We can maybe fire an event to Building
                        this.PeopleStepOutOfElevator(curFloorCall); 
                    }
                    else if (this.ElevatorState == ElevatorState.MovingUp)
                    {
                        // if next loop we go over our floor height then reduce the increment 
                        if (curFloorCall.ElevatorFloorHeightToStopToStepOut < this.CurrentElevatorHeight + this.elevatorSpeed)
                        {
                            var  toIncrement = (this.CurrentElevatorHeight + this.elevatorSpeed) - curFloorCall.ElevatorFloorHeightToStopToStepOut;

                            this.CurrentElevatorHeight = CurrentElevatorHeight + toIncrement;
                            // next cycle we can stepout
                            adjustedMovementInLoop = true;
                            break;
                        }
                    }
                    else if (this.ElevatorState == ElevatorState.MovingDown)
                    {
                        // if next loop we go over our floor height then reduce the increment 
                        if (curFloorCall.ElevatorFloorHeightToStopToStepOut > this.CurrentElevatorHeight + this.elevatorSpeed)
                        {
                            var toDecrement = (this.CurrentElevatorHeight + this.elevatorSpeed) - curFloorCall.ElevatorFloorHeightToStopToStepOut;

                            this.CurrentElevatorHeight = CurrentElevatorHeight + toDecrement;
                            // next cycle we can stepout
                            adjustedMovementInLoop = true;
                            break;
                        }
                    }
                }
            }

            // after potentially stepping out, we check to see if people can step in.
            if (this.floorCalls.Count > 0)
            {
                var orderedElevatorFloorCalls = this.ElevatorState == ElevatorState.MovingUp ? this.floorCalls.OrderBy(g => g.ElevatorFloorHeightToStopToStepIn).ToList() :
                   this.floorCalls.OrderByDescending(g => g.ElevatorFloorHeightToStopToStepIn).ToList();

                orderedElevatorFloorCalls = orderedElevatorFloorCalls.Where(x => x.SteppedIn == false).ToList();
                // we do order by ElevatorFloorHeightToStop, so that would be 10, 40, 70 and so on.
                foreach (var curFloorCall in orderedElevatorFloorCalls.ToList())
                {

                    //elevator arrived exactly where the current floor needs to be
                    if (curFloorCall.ElevatorFloorHeightToStopToStepIn == this.CurrentElevatorHeight)
                    {
                        // we can claim that in here, people went out of the elevator. We can maybe fire an event to Building
                        this.PeopleStepInElevator(curFloorCall);
                    }
                    // if next loop we go over our floor height then reduce the increment 
                    else if (this.ElevatorState == ElevatorState.MovingUp && adjustedMovementInLoop == false)
                    {
                        if (curFloorCall.ElevatorFloorHeightToStopToStepIn < this.CurrentElevatorHeight + this.elevatorSpeed)
                        {
                            var toIncrement = (this.CurrentElevatorHeight + this.elevatorSpeed) - curFloorCall.ElevatorFloorHeightToStopToStepIn;

                            this.CurrentElevatorHeight = CurrentElevatorHeight + toIncrement;
                            adjustedMovementInLoop = true;
                            break;
                        }
                    }
                    // if next loop we go over our floor height then reduce the increment 
                    else if (this.ElevatorState == ElevatorState.MovingDown && adjustedMovementInLoop == false)
                    {                        
                        if (curFloorCall.ElevatorFloorHeightToStopToStepIn > this.CurrentElevatorHeight + this.elevatorSpeed)
                        {
                            var toDecrement = (this.CurrentElevatorHeight + this.elevatorSpeed) - curFloorCall.ElevatorFloorHeightToStopToStepIn;

                            this.CurrentElevatorHeight = CurrentElevatorHeight + toDecrement;
                            adjustedMovementInLoop = true;
                            break;
                        }
                    }
                }
            }

            if(adjustedMovementInLoop == false)
            {
                if (this.ElevatorState == ElevatorState.MovingUp)
                {
                    this.GoUpIncremenet();
                }
                else if (this.ElevatorState == ElevatorState.MovingDown)
                {
                    this.GoDownIncremenet();
                }
                else if (this.ElevatorState == ElevatorState.Idle)
                {
                    //if idle then we need to do somehing.
                    if (this.floorCalls.Count > 0)
                    {
                        var closestFloorCall = this.floorCalls.OrderBy(v => Math.Abs((long)v.ElevatorFloorHeightToStopToStepOut - this.CurrentElevatorHeight)).First();
                        if (closestFloorCall.ElevatorFloorHeightToStopToStepOut > this.CurrentElevatorHeight)
                        {
                            this.ElevatorState = ElevatorState.MovingUp;
                            this.GoUpIncremenet();
                        }
                        else if (closestFloorCall.ElevatorFloorHeightToStopToStepOut < this.CurrentElevatorHeight)
                        {
                            this.ElevatorState = ElevatorState.MovingDown;
                            this.GoDownIncremenet();
                        }
                        else if (closestFloorCall.ElevatorFloorHeightToStopToStepOut == this.CurrentElevatorHeight)
                        {
                            this.PeopleStepOutOfElevator(closestFloorCall);
                        }
                    }
                    else
                    {
                        // we are idle and no floor calls.
                    }
                }
            }
        }

        private void PeopleStepInElevator(ElevatorFloorCalls curFloorCall)
        {
            this.CurrentCapacityOfPeople = this.CurrentCapacityOfPeople + curFloorCall.CapacityOfPeople;
            curFloorCall.SteppedIn = true;
        }


        private void PeopleStepOutOfElevator(ElevatorFloorCalls curFloorCall)
        {
            this.CurrentCapacityOfPeople = this.CurrentCapacityOfPeople - curFloorCall.CapacityOfPeople;
            this.floorCalls.Remove(curFloorCall);
            if(this.floorCalls.Count == 0)
            {
                this.ElevatorState = ElevatorState.Idle;
            }
        }

        public void Dispose()
        {
            this.timer.Stop();
            this.timer.Dispose();
        }


        public record ElevatorFloorCalls
        {
            public  DateTime Added;
            public int ElevatorFloorHeightToStopToStepOut;
            public int ElevatorFloorHeightToStopToStepIn;
            public int CapacityOfPeople;

            public bool SteppedIn;
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
