using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawWindow
{   
    internal class Window
    {
        /// <summary>
        /// Отрисовать окно
        /// </summary>
        /// <param name="x">Начальная позиция по оси X</param>
        /// <param name="y">Начальная позиция по оси Y</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        public static void Draw(int x, int y, int width, int height)
        {
            Console.SetCursorPosition(x, y);                                        // Начальное положение курсора для отрисовки
            // header - шапка
            Console.Write("╔");                                                     // Отрисовка 1-го верхнего уголка
            for (int i = 0; i < width - 2; i++)                                     // Цикл для соединения двух уголков. -2 от ширины - учет двух уголков
                Console.Write("═");
            Console.Write("╗");                                                     // Отрисовка 2-го верхнего уголка

            Console.SetCursorPosition(x, y + 1);                                    // Установка курсора на строку ниже
            for (int i = 0; i < height - 2; i++)                                    // Цикл для отрисовки рабочей зоны и краев. -2 от высоты с учетом header и footer
            {
                Console.Write("║");                                                 // Левая рамка
                for (int j = x + 1; j < x + width - 1; j++)                         // Цикл для отрисовки рабочей зоны внутри рамок
                {
                    Console.Write(" ");                                             // Середина пространства заполняется пробелами
                }
                Console.Write("║");                                                 // Правая рамка
            }

            // footer - подвал
            Console.Write("╚");                                                     // Отрисовка 1-го нижнего уголка
            for (int i = 0; i < width - 2; i++)                                     // Цикл для соединения двух уголков. -2 от ширины - учет двух уголков
                Console.Write("═");
            Console.Write("╝");                                                     // Отрисовка 2-го нижнего уголка
            Console.SetCursorPosition(x, y);                                        // Установка курсора в начальное положение
        }
    }
}
