using ElevatorCodingChallenge.Business;
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
        

        private readonly List<ElevatorSlots> elevatorSlots;       

        private readonly int numberOfFloors;
        private readonly int floorHeight;
        private readonly int betweenFloorHeight;
        private bool isBuilt;
        private readonly int maxFloorHeight;
        public Building(int numberOfFloorsParam, int floorHeight, int betweenFloorHeight)
        {
            if(numberOfFloorsParam <= 0)
                throw new ArgumentException("The numberOfFloorsParam must be greater than 0");

            this.numberOfFloors = numberOfFloorsParam;
            this.floorHeight = floorHeight;
            this.betweenFloorHeight = betweenFloorHeight;
            this.elevatorSlots = new List<ElevatorSlots>();          
            this.maxFloorHeight = this.GetFloorHeightByFloor(numberOfFloorsParam);

        }

        public List<ElevatorsSlotLivePosition> GetElevatorsPosition()
        {
            return this.elevatorSlots.Select(slot => new ElevatorsSlotLivePosition(slot.ElevatorId ,slot.CurrentElevatorHeight,slot.ElevatorState)).ToList();
        }

        public void FireTimerEventElevatorForDebug(string elevatorSlotId)
        {
            this.elevatorSlots.First(x => x.ElevatorId == elevatorSlotId).HandleTimer();
        }

        public int GetFloorHeightByFloor(int floor)
        {
            return (this.betweenFloorHeight + this.floorHeight) * floor;
        }
        public Building AddElevatorSlot(int speed, int maxPeopleInElevator, string elevatorId)
        {
            if (isBuilt == false)
            {
                var elSlot = new ElevatorSlots(
                    maxFloorHeight: this.maxFloorHeight,
                    maxNumberPeople: maxPeopleInElevator,
                elevatorSpeed: speed,
                numberOfFloors: this.numberOfFloors,
                floorHeight: this.floorHeight,
                betweenFloorHeight: this.betweenFloorHeight,
                elevatorId: elevatorId);

                this.elevatorSlots.Add(elSlot);
                return this;
            }
            else
            {
                throw new Exception("Building was already built, when you called Build Method. You can no longer add new Elevator Slots");
            }
        }
        public void Build() 
        {
            if(isBuilt == false)
            {
                if (elevatorSlots.Count == 0)
                {
                    throw new ArgumentException("You need add atleast 1 elevator slot, use method 'AddElevatorSlot'");
                }


                this.elevatorSlots.ForEach(f => f.StartElevator());
                this.isBuilt = true;
            }
            else 
            {
                throw new Exception("Building was already built. You can not build this object again.");
            }
            
        }


       

        public void CallElevator(int floorFromCalled, int floorToCalled, int NumberOfPeople)
        {
            ElevatorState desiredElevatorState = floorToCalled > floorFromCalled ? ElevatorState.MovingUp : ElevatorState.MovingDown; ;

            var orderedAndAvailableCapcityElevators = this.elevatorSlots
                // only look at elevators that have capacity,
                // todo: split up this capacity into multiple calls of different elevators.
                .Where(curElevator => NumberOfPeople <= (curElevator.MaxNumberPeople - curElevator.CurrentCapacityOfPeople))

                // we need to take a moving up elevator.
                .Where(curElevator => ((curElevator.ElevatorState == desiredElevatorState) 
                                     || curElevator.ElevatorState == ElevatorState.Idle))


                // then we get the closet elevator.
                .OrderBy(elevator => Math.Abs((int)elevator.CurrentElevatorHeight - this.GetFloorHeightByFloor(floorFromCalled))).ToList();

            if (orderedAndAvailableCapcityElevators.Count() > 0)
            {
                orderedAndAvailableCapcityElevators.First().AddElevatorFloorCall(new ElevatorSlots.ElevatorFloorCalls()
                {
                    Added = DateTime.Now,
                    ElevatorFloorHeightToStopToStepOut = this.GetFloorHeightByFloor(floorToCalled),
                    ElevatorFloorHeightToStopToStepIn = this.GetFloorHeightByFloor(floorFromCalled),
                    CapacityOfPeople = NumberOfPeople
                });
            }
            else
            {
                // Here we need to take into account and split capacity and schedule multiple if avaiable.
            }
        }

      

        public void Dispose()
        {
           
        }


    }
}
