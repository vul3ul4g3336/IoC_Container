using IoC_Container.Car;
using IoC_Container.CarFactory;
using IoC_Container.Game;
using IoC_Container.Game.Games;
using IoC_Container.Game.Platform;
using IoC_Container.House;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoC_Container
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"請問想要玩以下哪一款遊戲? 1.GTA \r\n 2.原神 \r\n 3. LOL");
            int game = int.Parse(Console.ReadLine());

           



           var arg = typeof(IGamePlatform<原神>).GetGenericArguments();
            ServiceCollection container = new ServiceCollection();
            ServiceProvider serviceProvider = new ServiceProvider(container);
            container.AddSingleton<ICar, Tesla>();
            container.AddSingleton<ICar, Toyota>();
            container.AddTransient<ICar>(() =>
            {
                ICar[] cars = { new BMW(), new BANZ() };

                return cars[new Random().Next(0,2)];
            });
            container.AddTransient<IHouse,BigHouse>();
            container.AddTransient<IFactory, TeslaFactory>();
            container.AddTransient<IPeople, People>();

            container.AddTransient(typeof(IGamePlatform<>), typeof(PC<>));
            //container.AddTransient<IGame, 原神>();

            container.AddTransient<IGame>(() =>
            {
                string gameName = "";
                switch (game)
                {
                    case 1:
                        gameName = "GTA";
                        break;
                    case 2:
                        gameName = "原神";
                        break;
                    case 3:
                        gameName = "LOL";
                        break;
                }
                Type gameType = Type.GetType($"IoC_Container.Game.Games.{gameName}");
                return (IGame)Activator.CreateInstance(gameType);
            });



            // IEnumerable<IGamePlatform<IGame>>
            // IGamePlatform<IGame>
            IPeople people = serviceProvider.GetService<IPeople>();
            people.Launch();
            Console.ReadKey();

      

            /*IGamePlatform<原神> game = new PC<原神>();*/
            // IGame<LOL> , PCGame<LOL> , XBOXGame<GTA6>
            //
        }
    }
}
