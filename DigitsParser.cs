using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;
namespace DigitParser
{
    //Программа переводит цифры в числа на англ. яз. Парсер работает на миллионных значениях
    class DigitsParser
    {
        static Dictionary<string, string> Words = new Dictionary<string, string>
        {
            ["1"] = "one",
            ["2"] = "two",
            ["3"] = "three",
            ["4"] = "four",
            ["5"] = "five",
            ["6"] = "six",
            ["7"] = "seven",
            ["8"] = "eight",
            ["9"] = "nine",
            ["0"] = "zero",
            ["10"] = "ten",
            ["11"] = "eleven",
            ["12"] = "twelve",
            ["13"] = "thirteen",
            ["14"] = "fourteen",
            ["15"] = "fifteen",
            ["16"] = "sixteen",
            ["17"] = "seventeen",
            ["18"] = "eighteen",
            ["19"] = "nineteen",
            ["20"] = "twenty",
            ["30"] = "thirty",
            ["40"] = "forty",
            ["50"] = "fifty",
            ["60"] = "sixty",
            ["70"] = "seventy",
            ["80"] = "eighty",
            ["90"] = "ninery",
        };
        static string million = "", hundred = "", thousand = "", answer = "";
        //Основной рабочий метод
        public static string Parser(string str)
        {
            //обнуляем это поле, если метод будет использоваться для обработки массива данных
            answer = "";
            //проверка на формат аргумента
            if (!Int32.TryParse(str, out int num))
                throw new ArgumentException("Введите число");
            //после прохождения этого цикла полям: миллионы, тысячи и сотни присвоятся значения в обратном порядке. 10001 сотни получат 100, а тысячи 01
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (hundred.Length != 3)
                    hundred += str[i];
                else if (thousand.Length != 3)
                    thousand += str[i];
                else
                    million += str[i];
            }
            //восстановить правильный порядок числа. Было 100 стало 001
            char[] temp = null;
            if (thousand != "")
            {
                temp = thousand.ToArray();
                Array.Reverse(temp);
                thousand = new string(temp);
            }
            if (hundred != "")
            {
                temp = hundred.ToArray();
                Array.Reverse(temp);
                hundred = new string(temp);
            }
            if (million != "")
            {
                temp = million.ToArray();
                Array.Reverse(temp);
                million = new string(temp);
            }
            //избавиться от лишних нулей перед числом. Было 001 стало 1 
            CheckingForZerosBeforeANumber();
            //работа метода разделена на три поток, потоки переводят миллионы, тысячи и сотни в буквенные представление
            Thread a = new Thread(new ThreadStart(ThreadMillion));
            Thread b = new Thread(new ThreadStart(ThreadThousand));
            Thread c = new Thread(new ThreadStart(ThreadHundred));
            //запускаются потоки
            if (million != "")
                a.Start();
            if (thousand != "")
                b.Start();
            if (hundred != "")
                c.Start();
            //ожидание полного выполнения потоков
            if (a.IsAlive)
                a.Join();
            if (b.IsAlive)
                b.Join();
            if (c.IsAlive)
                c.Join();
            //обнуляем поля, если метод будет обрабатывать массив данных
            thousand = "";
            hundred = "";
            million = "";
            return answer.Trim();
        }
        //поток обрабатывающий сотни
        static void ThreadHundred()
        {
            string thisanswer = "";
            if (hundred.Length == 3)
            {
                //если сотня не имеет десятков и единиц
                if (hundred[1] == '0' && hundred[2] == '0')
                {
                    answer += Words[hundred[0].ToString()] + " hundred";
                    return;
                }
                //сообщаем о кол-ве сотней
                thisanswer += Words[hundred[0].ToString()];
                thisanswer += " hundred";
                //если сотня не имеет десятков, но имеет единицы
                if (hundred[1] == '0')
                    thisanswer += " " + Words[hundred[2].ToString()] + " ";
                //если сотня имеет десятки, но не имеет единиц
                else if (hundred[2] == '0')
                {
                    thisanswer += " ";
                    thisanswer += Words[hundred[1].ToString() + hundred[2].ToString()];
                }
                //если дестяки и единицы начинаются на одну цифру
                else if (hundred[1] == hundred[2])
                {
                    thisanswer += " " + Words[hundred[1].ToString() + hundred[2].ToString()];
                }
                else if (hundred[1] == '1')
                {
                    thisanswer += " " + Words[hundred[1].ToString() + hundred[2].ToString()];
                } 
                //если сотня имеет разные десятки и единицы не равные 0
                else
                {
                    thisanswer += " ";
                    thisanswer += Words[hundred[1].ToString() + "0"];
                    thisanswer += "-";
                    thisanswer += Words[hundred[2].ToString()];
                    thisanswer += " ";
                }
                answer += thisanswer;
            }
            //если сотня состоит из десятков
            else if (hundred.Length == 2)
            {
                //если у сотни нет единиц
                if (hundred[1] == '0')
                    answer += Words[hundred[0].ToString() + hundred[1].ToString()];
                //если у сотни десяток и единицы ничинаются на одинаковую цифру
                else if (hundred[0] == hundred[1])
                    answer += Words[hundred[0].ToString() + hundred[1].ToString()];
                //уникальные двузначные числа twelve, eleven и тд
                else if (hundred[0] == '1')
                    answer += Words[hundred];
                //если у сотни десятки и единицы имеют разные цифры
                else
                    answer += Words[hundred[0].ToString() + "-" + hundred[1].ToString()];
            }
            //если сотни вообще есть
            else if (hundred.Length > 0)
                answer += Words[hundred[0].ToString()];
        }
        //поток обрабатывающий тысячи (работает как поток, обрабатывающий сотни)
        static void ThreadThousand()
        {
            string thisanswer = "";
            //если тысяч трехзначное число
            if (thousand.Length == 3)
            {
                //если в тысяче только сотни
                if (thousand[1] == '0' && thousand[2] == '0')
                {
                    answer += Words[thousand[0].ToString()] + " hundred thousand ";
                    return;
                }
                thisanswer += Words[thousand[0].ToString()];
                thisanswer += " hundred";
                //если тысяча имеет сотни, но не имеет десятки
                if (thousand[1] == '0')
                    thisanswer += " " + Words[thousand[2].ToString()];
                //если тысяа имеет только сотни
                else if (thousand[2] == '0')
                {
                    thisanswer += " ";
                    thisanswer += Words[thousand[1].ToString() + thousand[2].ToString()];
                }
                //если тысяча имеет уникальное двухзначное чилсло, twelve, eleven
                else if (thousand[1] == '1')
                    thisanswer += " " + Words[thousand[1].ToString() + thousand[2].ToString()];
                //если тысяча имеет двухзначное число по типу 22 twenty-two, 33, 35
                else
                {
                    thisanswer += " ";
                    thisanswer += Words[thousand[1].ToString() + "0"];
                    thisanswer += "-";
                    thisanswer += Words[thousand[2].ToString()];
                }

                thisanswer += " thousand ";
                answer += thisanswer;
            }
            //если тысяча состоит из десятков
            else if (thousand.Length == 2)
            {
                //если в тысяче нет единиц
                if (thousand[1] == '0')
                    answer += Words[thousand[0].ToString() + thousand[1].ToString()] + " thousand ";
                //если тысяча состоит из одинаковых цифр 22 33
                else if (thousand[0] == thousand[1])
                    answer += Words[thousand[0].ToString() + thousand[1].ToString()] + " thousand ";
                //если тысяча имеет уникальное двухзначное число 12 13 14 15
                else if (thousand[0] == '1')
                    answer += Words[million[0].ToString() + million[1].ToString()] + " thousand";
                //если тысяа другое любое число, 25, 26
                else
                    answer += Words[thousand[0].ToString() + "0"] + "-" + Words[thousand[1].ToString()] + " thousand ";
            }
            //если тысячи есть
            else if (thousand.Length > 0)
                answer += Words[thousand[0].ToString()] + " thousand ";
        }
        //поток обрабатывающий миллионы
        static void ThreadMillion()
        {
            string thisanswer = "";
            if (million.Length == 3)
            {
                if (million[1] == '0' && million[2] == '0')
                {
                    answer += Words[million[0].ToString()] + " hundred million ";
                    return;
                }
                thisanswer += Words[million[0].ToString()];
                thisanswer += " hundred";
                if (million[1] == '0')
                    thisanswer += " " + Words[million[2].ToString()];
                else if (million[2] == '0')
                {
                    thisanswer += " ";
                    thisanswer += Words[million[1].ToString() + million[2].ToString()];
                }
                else if (million[1] == '1')
                    thisanswer += " " + Words[million[1].ToString() + million[2].ToString()];
                else
                {
                    thisanswer += " ";
                    thisanswer += Words[million[1].ToString() + "0"];
                    thisanswer += "-";
                    thisanswer += Words[million[2].ToString()];
                }

                thisanswer += " million ";
                answer += thisanswer;
            }
            else if (million.Length == 2)
            {
                if (million[1] == '0')
                    answer += Words[million[0].ToString() + million[1].ToString()];
                else if (million[0] == million[1])
                    answer += Words[million[0].ToString() + million[1].ToString()];
                else if (million[0] == '1')
                    answer += Words[million[0].ToString() + million[1].ToString()];
                else
                    answer += Words[million[0].ToString() + "-" + million[1].ToString()];
                answer += " million ";
            }
            else if(million.Length > 0)
                answer += Words[million[0].ToString()] + " million ";
        }
        //избавиться от лишних нулей перед числом. Было 001 стало 1 
        static void CheckingForZerosBeforeANumber()
        {
            if (hundred.Length == 3)
            {
                if (hundred == "000")
                    hundred = "";
                else if (hundred[0] == '0' && hundred[1] == '0')
                    hundred = hundred.Remove(0, 2);
                else if (hundred[0] == '0')
                    hundred = hundred.Remove(0, 1);
            }
            if (thousand.Length == 3)
            {
                if (thousand == "000")
                    thousand = "";
                else if (thousand[0] == '0' && thousand[1] == '0')
                    thousand = thousand.Remove(0, 2);
                else if (thousand[0] == '0')
                    thousand = thousand.Remove(0, 1);
            }
        }
    }
}
