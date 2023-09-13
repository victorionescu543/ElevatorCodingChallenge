using Elevator.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorCodingChallenge.Business
{
    public record ElevatorsSlotLivePosition(string elevatorId, int currentHeight, ElevatorState elevatorState);
}
