using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator.Wpf
{
    public class Elevator
    {
        private readonly ElevatorType elevatorType;
        public Elevator(ElevatorType elevatorTypeParam)
        {
            this.elevatorType = elevatorTypeParam;            
        }
    }
}
