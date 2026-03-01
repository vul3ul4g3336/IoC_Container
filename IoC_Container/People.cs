using IoC_Container.Car;
using IoC_Container.Game;
using IoC_Container.Game.Games;
using IoC_Container.House;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class People:IPeople
    {
        private IEnumerable<ICar> cars;
        private IHouse house;
        private IGamePlatform<IGame> gamePlatform;
        public People(IEnumerable<ICar> cars, IHouse house, IGamePlatform<IGame> gamePlatform)
        {
            this.cars = cars;
            this.house = house;
            this.gamePlatform = gamePlatform;
        }
        public void Launch()
        {
            cars.ToList().ForEach(car =>
            {

                car.ShowInfo();
                car.Drive();
            });


            house.ShowInfo();
            house.Live();
            gamePlatform.Launch();

        }
    }
}
