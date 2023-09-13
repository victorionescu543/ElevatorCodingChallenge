using Elevator.Wpf;
using System.Drawing;

namespace ElevatorCodingChallenge.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task TestElevator()
        {
            string elevatorId = "firstelevator";


            Building currentBuilding = new Building(
                numberOfFloorsParam: 4,
                floorHeight: 10,
                betweenFloorHeight: 30)
                .AddElevatorSlot(elevatorId: elevatorId, speed: 10, maxPeopleInElevator: 5);
            //    .AddElevatorSlot(elevatorId: "secondelevator", speed:5,  maxPeopleInElevator: 5);


            currentBuilding.CallElevator(floorFromCalled: 1, floorToCalled: 4, NumberOfPeople: 1);



            currentBuilding.Build();

            currentBuilding.FireTimerEventElevatorForDebug(elevatorId);
            //run until all elevators are idle
            while (currentBuilding.GetElevatorsPosition().Where(d => d.elevatorState != ElevatorState.Idle).Count() > 0)
            {
                await Task.Delay(500);
                currentBuilding.FireTimerEventElevatorForDebug(elevatorId);
            }


            currentBuilding.CallElevator(floorFromCalled: 3, floorToCalled: 4, NumberOfPeople: 1);

            currentBuilding.FireTimerEventElevatorForDebug(elevatorId);
            //run until all elevators are idle
            while (currentBuilding.GetElevatorsPosition().Where(d => d.elevatorState != ElevatorState.Idle).Count() > 0)
            {
                await Task.Delay(500);
                currentBuilding.FireTimerEventElevatorForDebug(elevatorId);
            }


            int finalFloor = 1;
            currentBuilding.CallElevator(floorFromCalled: 2, floorToCalled: finalFloor, NumberOfPeople: 1);

            currentBuilding.FireTimerEventElevatorForDebug(elevatorId);
            //run until all elevators are idle
            while (currentBuilding.GetElevatorsPosition().Where(d => d.elevatorState != ElevatorState.Idle).Count() > 0)
            {
                await Task.Delay(500);
                currentBuilding.FireTimerEventElevatorForDebug(elevatorId);
            }

            Assert.IsTrue(currentBuilding.GetElevatorsPosition().First(x => x.elevatorId == elevatorId).currentHeight ==
               currentBuilding.GetFloorHeightByFloor(finalFloor), " We need elevator to be now present at the final floor: " + finalFloor.ToString()); 

            var tt = 3;
        }

    }
}