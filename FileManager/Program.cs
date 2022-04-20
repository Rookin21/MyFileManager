using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DrawWindow;

namespace FileManager
{
    class Program
    {
        // Константы для определения размеров консоли
        const int WINDOW_HEIGHT = 30;
        const int WINDOW_WIDTH = 120;
        // Установка текущей директории
        private static string currentDir = Directory.GetCurrentDirectory();

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;                // Цвет фона
            Console.Title = "MyFileManager";                                // Название консольного приложения

            Console.SetWindowSize(WINDOW_WIDTH, WINDOW_HEIGHT);             // Установка размеров консоли
            Console.SetBufferSize(WINDOW_WIDTH, WINDOW_HEIGHT);             // Установка размеров буфера

            Window.Draw(0, 0, WINDOW_WIDTH, 18);                             // Создание окна для показа дерева с файлами и каталогами
            Window.Draw(0, 18, WINDOW_WIDTH, 8);                             // Создание окна с информацией

            UpdateConsole();                                                // Метод для возможности повторного использования командной строки

            Console.ReadKey(true);
        }

        static void UpdateConsole()
        {
            DrawConsole(currentDir, 0, 26, WINDOW_WIDTH, 3);                // Отрисовка командной строки
            ProcessEnterCommand(WINDOW_WIDTH);                              // Обработка команд. Ограничение по ширине окна
        }

        /// <summary>
        /// Определение положения курсора
        /// </summary>
        /// <returns></returns>
        static (int left, int top) GetCursorPosition()
        {
            return (Console.CursorLeft, Console.CursorTop);
        }

        /// <summary>
        /// Ввод команд
        /// </summary>
        /// <param name="width"></param>
        static void ProcessEnterCommand(int width)
        {
            (int left, int top) = GetCursorPosition();                      // Определение положения курсора
            StringBuilder command = new StringBuilder();                    // создание динамической строки
            char key;                                                       // переменная для выполнения команд по символьно
            
            do                                                              // Цикл для обработки команда по символьно
            {
                key = Console.ReadKey().KeyChar;                            // считывание введененного символа и преобразование его в код

                if (key != 8 && key != 13)                                  // Записываем символ в консоль. Исключаем команды Enter и Backspace
                    command.Append(key);

                (int currentLeft, int currentTop) = GetCursorPosition();    // Определение позиции курсора

                if (currentLeft == width - 2)                               // Недопуск выхода за рамки
                {
                    Console.SetCursorPosition(currentLeft - 1, top);        // Отодвижение курсора на позицию назад
                    Console.Write(" ");                                     // Создание пустого пространства 
                    Console.SetCursorPosition(currentLeft - 1, top);
                }
                if (key == (char)8)                                         // Алгоритм работы кнопки Backspace
                {
                    if (command.Length > 0)                                 // Если в строке есть символы
                        command.Remove(command.Length - 1, 1);              // Удаляем последний символ
                    if (currentLeft >= left)                                // Блокировка для невозможности стереть стартовый путь в конcоли
                    {
                        Console.SetCursorPosition(currentLeft, top);        // Установка курсора в исходное положение
                        Console.Write(" ");
                        Console.SetCursorPosition(currentLeft, top);
                    }
                    else
                    {
                        Console.SetCursorPosition(left, top);               // После удаления символа остаемся на последнем символе
                    }
                }
            }
            while (key != (char)13);                                        // Заверашем цикл после нажатия Enter
            ParseCommandString(command.ToString());

        }
        static void ParseCommandString(string command)
        {
            string[] commandParams = command.ToLower().Split(' ');          // Строку переводим в нижний регистр, для исключения влияния регистра. Разбиваем команду на массив строк
            if (commandParams.Length > 0)                                   // Выполнение дальнейшних действий, если что-то было введено
            {
                switch (commandParams[0])                                   // Обработка команд начиная с первого введенного элемента
                {
                    // Переход в директорию на консоли
                    case "cd":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1])) // Если после cd было введена директория и существует ли она
                        {
                            currentDir = commandParams[1];                  // В глобальную переменную записываем введененный путь
                        }
                        break;
                        // Открытие дерева каталога
                    case "ls":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1])) // Если после ls было введена директория и существует ли она
                        {
                            if (commandParams.Length > 3 && commandParams[2] == "-p" && int.TryParse(commandParams[3], out int n))  // Если параметров больше 3 и третий параметр -p
                                                                                                                                    // и 4 параметр является int (записываем его в переменную)
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), n);   // Отрисовка дерева с директорией и с указанной страницей
                            }
                            else
                            {
                                DrawTree(new DirectoryInfo(commandParams[1]), 1);   // Отрисовка дерева с директорией с первой страницы
                            }
                        }
                        break;
                        // Копирование файла
                    case "cf":
                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))  // Проверка ввода чего-либо и существует ли файл
                        {                             
                            File.Copy($@"{commandParams[1]}", $@"{commandParams[2]}", true);
                        }
                        break;
                        // Копирование папки
                    case "cp":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1])) // Проверка ввода чего-либо и существует ли директория 
                        {
                            Directory.CreateDirectory(commandParams[2]);
                            foreach (string s1 in Directory.GetFiles(commandParams[1]))
                            {
                                string s2 = commandParams[2] + "\\" + Path.GetFileName(s1);
                                File.Copy(s1, s2);
                            }
                        }                       
                        break;
                        // Удаление папки
                    case "rm":
                        if (commandParams.Length > 1 && Directory.Exists(commandParams[1]))
                            Directory.Delete(commandParams[1], true);
                        break;
                        // Удаление файла
                    case "rf":
                        if (commandParams.Length > 1 && File.Exists(commandParams[1]))
                            File.Delete(commandParams[1]);
                        break;
                        // Вывод данных из файла
                    case "file":
                        Console.SetCursorPosition(1, 1);
                        string[] lines = File.ReadAllLines($@"{commandParams[1]}"); //Читаем все строки из файла names.txt в массив строк lines
                        foreach (var line in lines) //Перебираем все элементы массива lines. Для каждого значения будет вызываться код, находящийся ниже
                            Console.WriteLine(line);
                        break;
                        // Очистка экрана
                    case "clr":
                        Console.Clear();
                        Window.Draw(0, 0, WINDOW_WIDTH, 18);                             // Создание окна для показа дерева с файлами и каталогами
                        Window.Draw(0, 18, WINDOW_WIDTH, 8);                             // Создание окна с информацией
                        break;
                        // Вывод информации о документе
                    case "info":
                        Console.SetCursorPosition(1, 19);
                        DirectoryInfo directoryInfo = new DirectoryInfo($@"{commandParams[1]}");
                        Console.WriteLine("FullName: {0}", directoryInfo.FullName);
                        Console.WriteLine(" Name: {0}", directoryInfo.Name);
                        Console.WriteLine(" Creation: {0}", directoryInfo.CreationTime);
                        Console.WriteLine(" Root: {0}", directoryInfo.Root);
                        break;
                }
            }
            UpdateConsole();                                                        // После обработки команды обновляем консоль
        }
    
        /// <summary>
        /// Отрисовать делево каталогов
        /// </summary>
        /// <param name="dir">Директория</param>
        /// <param name="page">Страница</param>
        static void DrawTree(DirectoryInfo dir, int page)
        {
            StringBuilder tree = new StringBuilder();                               // Записываем дерево в строку                  
            GetTree(tree, dir, "", true);
            Window.Draw(0, 0, WINDOW_WIDTH, 18);                                    // Очистка окна
            (int currentLeft, int currentTop) = GetCursorPosition();                // Определение положения курсора
            int pageLines = 16;                                                     // Максимальное количество линий выводимых на странице
            string[] lines = tree.ToString().Split(new char[] { '\n' });            // Разбиение на линии с последующим переносом
            int pageTotal = (lines.Length + pageLines - 1) / pageLines;             // Подсчет общего количество страниц для отображения дерева
                                                                                    // Для исключения хвоста дробного числа к длине линии прибавляем количество линий
            if (page > pageTotal)                                                   // Исключение для ввода пользователем числа страницы больше, чем есть по факту
                page = pageTotal;

                                                                                    // Вывод страниц
            for (int i = (page - 1) * pageLines, counter = 0; i < page * pageLines; i++, counter++)
            {
                if (lines.Length - 1 > i)                                           // Проверка за выход границ линий
                {                                                                   // Начальная установка курсора для вывода дерева. Counter - для табуляции
                    Console.SetCursorPosition(currentLeft + 1, currentTop + 1 + counter);
                    Console.WriteLine(lines[i]);                                    // Вывод линий
                }
            }

            //footer
            string footer = $"╡ {page} of {pageTotal} ╞";                           // Номер текущей страницы и общего количества
            Console.SetCursorPosition(WINDOW_WIDTH / 2 - footer.Length / 2, 17);    // Установка курсора в середину экрана
            Console.WriteLine(footer);                                              // Вывод
        }

        /// <summary>
        /// Вывод дерева
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="dir"></param>
        /// <param name="indent"></param>
        /// <param name="lastDirectory"></param>
        static void GetTree(StringBuilder tree, DirectoryInfo dir, string indent, bool lastDirectory)
        {
            tree.Append(indent);                                                    // Добавление подстроки StringBuilder
            if (lastDirectory)                                                      // Последняя ли директория
            {
                tree.Append("└─");                                                  // Вывод ветвления 
                indent += "  ";
            }
            else
            {
                tree.Append("├─");                                                  // Вывод продолждения
                indent += "│ ";
            }

            tree.Append($"{dir.Name}\n"); 

            //Добавление отображения файлов
            FileInfo[] subFiles = dir.GetFiles();                                   // Запросить все файлы и записать в массив
            for (int i = 0; i < subFiles.Length; i++)                               // Проходим по всей длине массива
            {
                if (i == subFiles.Length - 1)                                       // Если последний файл
                {
                    tree.Append($"{indent}└─{subFiles[i].Name}\n");
                }
                else
                {
                    tree.Append($"{indent}├─{subFiles[i].Name}\n");
                }
            }

            DirectoryInfo[] subDirects = dir.GetDirectories();
            for (int i = 0; i < subDirects.Length; i++)
                GetTree(tree, subDirects[i], indent, i == subDirects.Length - 1);
        }

        /// <summary>
        /// Отрисовать консоль
        /// </summary>
        /// <param name="dir">Текущая директория</param>
        /// <param name="x">Начальная позиция по оси X</param>
        /// <param name="y">Начальная позиция по оси Y</param>
        /// <param name="width">Ширина</param>
        /// <param name="height">Высота</param>
        static void DrawConsole(string dir, int x, int y, int width, int height)
        {
            Window.Draw(x, y, width, height);                                       // Отрисовка командной строки
            Console.SetCursorPosition(x + 1, y + height / 2);                       // Установка курсора внутри рамки окна ввода команд 
            Console.Write($"{dir}>");                                               // Показ текущей директории
        }              
    }
}


