using System;
using System.IO;
using System.Collections.Generic;

namespace AlgorithmForRemovingEpsilonProducts
{
    class NandE
    {
        private HashSet<char> n;
        private HashSet<char> e;
        private bool isContainsEps;
        private HashSet<string> groups;
        public HashSet<string> Groups
        {
            get { return groups; }
            set { groups = value; }
        }
        public HashSet<char> N
        {
            get { return n; }
            set { n = value; }
        }
        public HashSet<char> E
        {
            get { return e; }
            set { e = value; }
        }
        public bool IsContainsEps
        {
            get { return isContainsEps; }
            set { isContainsEps = value; }
        }
        public NandE()
        {
            n = new HashSet<char>();
            e = new HashSet<char>();
            IsContainsEps = false;
            groups = new HashSet<string>();
        }
        public NandE(List<string> listOfOfRightChars, string right)
        {
            n = new HashSet<char>();
            e = new HashSet<char>();
            IsContainsEps = false;
            foreach (String c in listOfOfRightChars)
            {
                if (c != "\\e")
                {
                    if (IsN(c[0]))
                    {
                        n.Add(c[0]);
                    }
                    else if (IsE(c[0]))
                    {
                        e.Add(c[0]);
                    }
                }
                else
                {
                    IsContainsEps = true;
                }
            }
            Groups = CreateSetOfGroups(right);
        }
        public string GetCountInfo()
        {
            return $"Count of N: {n.Count}; Count of E: {e.Count}";
        }
        public void PrintCountInfo()
        {
            Console.WriteLine(GetCountInfo());
        }
        //Является ли символ нетерминалом
        private bool IsN(char c) { return Char.IsUpper(c); }

        //Является ли символ терминалом
        private bool IsE(char c) { return Char.IsLower(c) || Char.IsDigit(c); }
        private HashSet<String> CreateSetOfGroups(string right)
        {
            HashSet<String> myGroups = new HashSet<String>();
            string current_group = "";
            for (int i = 0; i < right.Length; i++)
            {
                if (right[i] != '|') //если прочли символ
                {
                    current_group += right[i];
                }
                else
                {
                    myGroups.Add(current_group);
                    current_group = "";
                }
            }
            myGroups.Add(current_group);//Прочли последнюю группу, но мы добавляли только после |
            return myGroups;
        }
        private int Count_N_of_group(string group)
        {
            int k = 0;
            foreach (char c in group)
            {
                if (IsN(c))
                {
                    k++;
                }
            }
            return k;
        }
        private int Count_E_of_group(string group)
        {
            int k = 0;
            if (group == "\\e")
            {
                return 0;
            }
            foreach (char c in group)
            {
                if (IsE(c))
                {
                    k++;
                }
            }
            return k;
        }
        public bool Contains_required_N_to_add_to_W(HashSet<char> w)
        {
            int k;//Счетчик найденных нетерминалов в одной из групп в правой части
            //Пройдемся по всем группам
            foreach (string group in Groups)
            {
                k = 0;
                foreach (char w_c in w)
                {
                    if (group.Contains(w_c))
                    {
                        k++;
                    }
                }
                if (k == Count_N_of_group(group) && Count_E_of_group(group)==0)
                {
                    return true;
                }
            }
            return false;
        }
    }
    class Product
    {
        private char left;
        private string right;
        private int countOfRightChars;
        public NandE RightChars;
        private bool isNeed;
        public char Left
        {
            get { return left; }
            set { left = value; }
        }
        public string Right
        {
            get { return right; }
            set { right = value; }
        }
        public int CountOfRightChars
        {
            get { return countOfRightChars; }
            set { countOfRightChars = value; }
        }
        public bool IsNeed
        {
            get { return isNeed; }
            set { isNeed = value; }
        }
        public Product() : this("Empty->Empty")
        {
        }
        public Product(string pString)
        {
            String[] str = pString.Split("->");
            if (str[0].Length > 1 && Char.IsLower(str[0][0]))
                throw new Exception("Слева должен быть 1 нетерминал");
            this.left = str[0][0];
            this.right = str[1];
            CountOfRightChars = CalculateCountOfRightChars();

            List<string> lst = CreateListOfRightChars();
            RightChars = new NandE(lst, Right);
            IsNeed = true;
        }
        public Product(string xLeft, string xRight)
        {
            if (xLeft.Length > 1 && Char.IsLower(xLeft[0]))
                throw new Exception("Слева должен быть 1 нетерминал");
            this.left = xLeft[0];
            this.right = xRight;
            CountOfRightChars = CalculateCountOfRightChars();

            List<string> lst = CreateListOfRightChars();
            RightChars = new NandE(lst,Right);
            IsNeed = true;
        }
        public string GetFullString()
        {
            return $"{left}->{right}";
        }
        public void Print()
        {
            Console.WriteLine(GetFullString());
        }
        private List<String> CreateListOfRightChars()
        {
            List<String> lst = new List<string>();
            bool f = true; //флаг для \e
            for (int i = 0; i < right.Length; i++)
            {
                f = true;
                if (right[i] != '|') //если прочли символ
                {
                    if (right[i] == '\\') //Встретили ли мы начало \e?
                    {
                        if ((i + 1) < right.Length && right[i + 1] == 'e')
                        {      //встретили \e
                            lst.Add(right[i].ToString() + right[i + 1].ToString());  //Добавили символ \e
                            f = false; //т.к. добавили символ \e
                            i++; //Чтобы пропустить \e
                        }
                    }
                    if (f)//Если не добавили \e
                    {
                        lst.Add(right[i].ToString());  //Добавили символ
                    }
                }
            }
            return lst;
        }
        private int CalculateCountOfRightChars()
        {
            int k = 0;
            for (int i = 0; i < right.Length; i++)
            {
                if (right[i] != '|') //если прочли символ
                {
                    if (right[i] == '\\') //Встретили ли мы начало \e?
                    {
                        if ((i + 1) < right.Length && right[i + 1] == 'e')
                        {      //встретили \e
                            i++; //Чтобы пропустить \e
                        }
                    }
                    k++;//Прочли символ
                }
            }
            return k;
        }
    }
    class Program
    {
        //Чтение строки из файла
        static string MyReadFile(ref string inPath)
        {
            string sourceString = "";
            try
            {
                using (var f = new StreamReader(inPath))
                {
                    sourceString = f.ReadLine();
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            return sourceString;
        }
        //Кол-во вхождений подстроки в строку
        static int CountOfContains(string str, string substr)
        {
            int count = 0, index = 0;
            while ((index = str.IndexOf(substr, index) + 1) != 0)
                count++;
            return count;
        }
        //Возвращает индексы вхождений
        static List<int> IndexsOfContains(string str, string substr)
        {
            int index = 0;
            List<int> indexs = new List<int>();
            while ((index = str.IndexOf(substr, index) + 1) != 0)
            {
                indexs.Add(index-1);
            }
            return indexs;
        }
        //Визуальная запись в файл
        static void MyVisualWriteFile(ref string outComfortablePath, ref Product[] p, bool myAppend, bool isNewRoot, bool isOnlyNewRoot, bool IsAfterProcessing)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(outComfortablePath, myAppend, System.Text.Encoding.Default))
                {
                    if (!IsAfterProcessing)
                    {
                        sw.WriteLine("Дано:");
                        sw.WriteLine();
                        for (int i = 0; i < p.Length; i++)
                        {
                            sw.WriteLine(p[i].GetFullString());
                        }
                        sw.WriteLine("-----------------------");
                    }
                    if (IsAfterProcessing)
                    {
                        if (!isOnlyNewRoot)
                        {
                            sw.WriteLine("После алгоритма удаления \\e продукций:");
                            sw.WriteLine();
                            if (isNewRoot)
                            {
                                sw.WriteLine("S'->\\e|S");
                            }
                            for (int i = 0; i < p.Length; i++)
                            {
                                if (p[i].Right.Length != 0)
                                {
                                    sw.WriteLine(p[i].GetFullString());
                                }
                            }
                            sw.WriteLine("*****************************");
                            sw.WriteLine("*****************************");
                        }
                        else
                        {
                            sw.WriteLine("После алгоритма удаления \\e продукций:");
                            sw.WriteLine();
                            sw.WriteLine("S->\\e");

                            sw.WriteLine("*****************************");
                            sw.WriteLine("*****************************");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            string inPath = @"in.txt";
            string outPath = @"out.txt";
            string outComfortablePath = @"outComfortable.txt";
            string outExamplePath = @"outComfortableExamples.txt";


            string sourceString = MyReadFile(ref inPath); //Прочли строку

            String[] pStrings = sourceString.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            Product[] p = new Product[pStrings.Length];
            Product[] p_before = new Product[pStrings.Length];//Для печати в дальнейшем
            for (int i = 0; i < pStrings.Length; i++)
            {
                p[i] = new Product(pStrings[i]);
                p_before[i] = new Product(pStrings[i]);
            }
            int n_old_count = 0; // Для 3 шага
            HashSet<Char> n_old = new HashSet<Char>(); //Нетерминалы для того, чтобы лишний раз по ним не проходиться
            HashSet<Char> n = new HashSet<Char>(); //Нетерминалы будущие

            HashSet<Char> w = new HashSet<Char>();
            HashSet<Char> w_new = new HashSet<Char>(); //В него будут добавляться новые нетерминалы

            //Шаг 1
            //W0
            //Пробегаем по всем левым частям
            for (int i = 0; i < p.Length; i++)
            {
                //Добавляем в w нетерминалы, которых нет. Если в правой части есть эпсилон, то добавить его во мн-во w
                if (!w.Contains(p[i].Left) && p[i].RightChars.IsContainsEps)
                {
                    w.Add(p[i].Left);
                    n_old_count++;
                }
            }

            while (true)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    //Работаем сейчас с w i, чтобы потом соединить вместе с w i-1 основным
                    //(Добавляем в w нетерминалы, которых нет). (Если в правой части уже есть нетерминалы помеченные отдельно, то добавить его во мн-во w)
                    if (!w.Contains(p[i].Left) && p[i].RightChars.Contains_required_N_to_add_to_W(w))//второе условие переделать
                    {
                        w_new.Add(p[i].Left);
                    }
                }

                //Теперь добавляем к основному мн-ву w
                foreach (char w_c in w_new)
                {
                    w.Add(w_c);
                }


                //Шаг 3
                if (n.Count == n_old_count)
                {
                    break;
                }
                else
                {
                    n_old_count = n.Count;
                }
            }

            //Шаг 4
            //--------------------------------------------------------------------------
            for (int i = 0; i < pStrings.Length; i++)
            {
                if (p[i].RightChars.IsContainsEps && p[i].Left!='S')
                {
                    p[i].RightChars.Groups.Remove("\\e");

                    int indexOfEps = p[i].Right.IndexOf("\\e");
                    p[i].Right = p[i].Right.Remove(indexOfEps, "\\e".Length);

                    //Удаление |
                    if (indexOfEps != 0)
                    {
                        p[i].Right = p[i].Right.Remove(indexOfEps-1, 1);
                    }
                    else
                    {
                        if ((p[i].RightChars.Groups.Count) > 0)
                        {
                            p[i].Right = p[i].Right.Remove(0, 1);
                        }
                    }
                    p[i].RightChars.IsContainsEps = false;
                }
            }

            //Шаг 5
            //--------------------------------------------------------------------------

            HashSet<string>[] p_new = new HashSet<string>[p.Length];
            for(int i=0;i< p.Length; i++)
            {
                p_new[i] = new HashSet<string>();
            }

            //Узнаем максимальное кол-во нетерминалов группы
            int max_count_N_of_group = 0;

            for (int i = 0; i < p.Length; i++)
            {
                foreach (string group in p[i].RightChars.Groups)
                {
                    int countOfContains = 0;//Кол-во вхождений
                    foreach (char w_c in w)
                    {
                        countOfContains += CountOfContains(group, w_c.ToString());
                    }
                    if(max_count_N_of_group < countOfContains)
                    {
                        max_count_N_of_group = countOfContains;
                    }
                }
            }

            for (int y=0;y< max_count_N_of_group; y++)
            {
                for (int i = 0; i < p.Length; i++)
                {
                    foreach (string group in p[i].RightChars.Groups)
                    {
                        int countOfContains = 0;//Кол-во вхождений
                        List<int> indexs = new List<int>();
                        List<int> w_c_indexs = new List<int>();//вспомогательный для добавления новых индексов
                        foreach (char w_c in w)
                        {
                            countOfContains += CountOfContains(group, w_c.ToString());
                            w_c_indexs = IndexsOfContains(group, w_c.ToString());
                            foreach (int x in w_c_indexs)
                            {
                                indexs.Add(x);
                            }
                        }
                        foreach (int index in indexs)
                        {
                            string help_group = group;
                            help_group = help_group.Remove(index, 1);

                            if(p[i].Left != 'S' && help_group != "")
                            {
                                p_new[i].Add(help_group);
                            }
                            if (p[i].Left == 'S')
                            {
                                p_new[i].Add(help_group);
                            }
                        }
                    }
                    foreach (string str in p_new[i])
                    {
                        p[i].RightChars.Groups.Add(str);
                    }
                }
            }
            //Обработка строк
            for (int i = 0; i < p.Length; i++)
            {
                p[i].Right = "";
                foreach (string group in p[i].RightChars.Groups)
                {
                    if (group != "")
                    {
                        p[i].Right += group + "|";
                    }
                }
                int lastOfStr = p[i].Right.Length - 1;
                if (p[i].Right.Length!=0 && p[i].Right[lastOfStr] == '|')
                {
                    p[i].Right = p[i].Right.Remove(lastOfStr, 1);
                }
            }

            //Шаг 6
            //--------------------------------------------------------------------------

            bool isNewRoot = false;//Если S->\e...
            bool isOnlyNewRoot = false;//Если S->\e и всё

            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].Left == 'S' && (p[i].RightChars.Groups.Contains("") || p[i].RightChars.IsContainsEps))
                {
                    p[i].RightChars.IsContainsEps = false;
                    p[i].RightChars.Groups.Remove("");
                    p[i].RightChars.Groups.Remove("\\e");

                    int indexOfEps = p[i].Right.IndexOf("\\e");
                    if (indexOfEps != -1)
                    {
                        p[i].Right = p[i].Right.Remove(indexOfEps, "\\e".Length);

                        //Удаление |
                        if (indexOfEps != 0)
                        {
                            p[i].Right = p[i].Right.Remove(indexOfEps - 1, 1);
                        }
                        else
                        {
                            if ((p[i].RightChars.Groups.Count) > 0)
                            {
                                p[i].Right = p[i].Right.Remove(0, 1);
                            }
                        }
                    }
                    isNewRoot = true;
                    if (p[i].RightChars.Groups.Count == 0)
                    {
                        isOnlyNewRoot = true;
                    }
                }
            }

            HashSet<char> notNeeded = new HashSet<char>();

            //Обработка бессмысленных правил
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i].Right.Length == 0)
                {
                    p[i].IsNeed = false;
                    notNeeded.Add(p[i].Left);
                }
            }
            for (int i = 0; i < p.Length; i++)
            {
                foreach(char notNeeded_c in notNeeded)
                {
                    while(p[i].Right.Contains(notNeeded_c))
                    {
                        int indexOfNotNeeds = p[i].Right.IndexOf(notNeeded_c);
                        p[i].Right = p[i].Right.Remove(indexOfNotNeeds, 1);
                    }
                }
            }
            //...|бессмысленный символ|... -> ...||... - Исправляем
            for (int i = 0; i < p.Length; i++)
            {
                while (p[i].Right.Contains("||"))
                {
                    p[i].Right = p[i].Right.Replace("||", "|");
                }
            }
            //Запись в файл
            try
            {
                using (StreamWriter sw = new StreamWriter(outPath, false, System.Text.Encoding.Default))
                {
                    if (!isOnlyNewRoot)
                    {
                        if (isNewRoot)
                        {
                            sw.Write("S'->\\e|S ");
                        }
                        for (int i = 0; i < p.Length; i++)
                        {
                            if (p[i].Right.Length != 0)
                            {
                                sw.Write(p[i].GetFullString() + " ");
                            }
                        }
                    }
                    else
                    {
                        sw.Write("S->\\e");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //----------------------------------------------

            //Визуальная запись в файл до/после
            MyVisualWriteFile(ref outComfortablePath, ref p_before, false, isNewRoot, isOnlyNewRoot, false);
            MyVisualWriteFile(ref outComfortablePath, ref p, true, isNewRoot, isOnlyNewRoot, true);

            //----------------------------------------------
            //Запись примеров в файл до/после без перезаписывания

            MyVisualWriteFile(ref outExamplePath, ref p_before, true, isNewRoot, isOnlyNewRoot, false);
            MyVisualWriteFile(ref outExamplePath, ref p, true, isNewRoot, isOnlyNewRoot, true);
        }
    }
}
