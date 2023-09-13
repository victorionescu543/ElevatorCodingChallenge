using Elevator.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElevatorCodingChallenge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {


            Building currentBuilding = new Building(numberOfFloorsParam: 4, floorHeight: 10, betweenFloorHeight: 30)
             .AddElevatorSlot(speed: 10, maxPeopleInElevator: 3, elevatorId: "first")
             .AddElevatorSlot(speed: 5, maxPeopleInElevator: 2, elevatorId: "second");


          
            currentBuilding.Build();

        
            InitializeComponent();
        }

        
    }
}
