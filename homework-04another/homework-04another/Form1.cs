using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace homework_04another
{
    /*--------自定义类声明--------*/
    public class Character
    {
        public char c;
        public int posX;
        public int posY;
        public Character preC;
        public int plicNum;
    }

    public class Word
    {
        public string s;
    }

    public class MyWord : Word
    {
        public int posX;
        public int posY;
        public int dir;
        public bool isPos;
    }
    
    public partial class Form1 : Form
    {
        //变量定义
        #region
        public List<MyWord> wordList = new List<MyWord>();
        public Character[,] map = new Character[100, 100];
        static int[,] dirChange = new int[8, 2] { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 }, { 1, 1 }, { 1, -1 }, { -1, 1 }, { -1, -1 } };
        public int[] dirNum = new int[8];
        public int mapN = 1, mapM = 1;
        public bool suc = false;
        #endregion
        
        /*--------读文件--------*/
        public void readFile()
        {
            StreamReader sr = new StreamReader("input.txt");
            Console.SetIn(sr);
            string s = Console.ReadLine();
            while (s != null)
            {
                MyWord newWord = new MyWord();
                newWord.s = s;
                if ((s[0] >= 'A' && s[0] <= 'Z') || (s[0] >= 'a' && s[0] <= 'z'))
                    wordList.Add(newWord);
                s = Console.ReadLine();
            }
        }

        /*--------自己写的字符串反转函数--------*/
        public string Reverse(string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /*--------翻转部分单词,保证八个方向--------*/
        void randomReverse()
        {
            for (int i = 0; i < wordList.Count; i++)
            {
                if (i % 2 == 1)
                {
                    wordList[i].s = Reverse(wordList[i].s);
                }
            }
        }
        
        /*--------主逻辑部分--------*/
        void initSearch()
        {
            searchWord(0);
            mapN++;
            mapM++;
            posWord(mapN, 1, 2, wordList.Count() - 3);
            posWord(mapN, mapM, 3, wordList.Count() - 2);
            posWord(1, mapM, 0, wordList.Count() - 1);
        }

        /*--------是否可以放单词--------*/
        bool canPos(int x, int y, int dir, string s)
        {
            int xc = x, yc = y;
            for (int i = 0; i < s.Length; i++)
            {
                if (xc < 1 || yc < 1)
                    return false;
                if (map[xc, yc] != null && map[xc, yc].c != s[i])
                    return false;
                xc = xc + dirChange[dir, 0];
                yc = yc + dirChange[dir, 1];
            }
            return true;
        }

        /*--------放入该单词--------*/
        void posWord(int x, int y, int dir, int num)
        {

            string s = wordList[num].s;
            int xc = x, yc = y;
            wordList[num].dir = dir;
            wordList[num].posX = x;
            wordList[num].posY = y;
            wordList[num].isPos = true;

            for (int i = 0; i < s.Length; i++)
            {
                Character c = new Character();
                c.c = s[i];
                c.posX = xc;
                c.posY = yc;
                if (map[xc, yc] != null)
                    c.plicNum = map[xc, yc].plicNum + 1;
                else
                    c.plicNum = 1;
                map[xc, yc] = c;

                if (i != 0)
                {
                    map[xc, yc].preC = map[xc - dirChange[dir, 0], yc - dirChange[dir, 1]];
                }
                xc += dirChange[dir, 0];
                yc += dirChange[dir, 1];

            }
        }

        /*--------移除该单词(回溯)--------*/
        void removePos(int x, int y, int dir, int num)
        {
            string s = wordList[num].s;
            wordList[num].dir = 0;
            wordList[num].posX = 0;
            wordList[num].posY = 0;
            wordList[num].isPos = false;
            int xc = x, yc = y;
            for (int i = 0; i < s.Length; i++)
            {
                map[xc, yc].plicNum--;
                if (map[xc, yc].plicNum <= 0)
                    map[xc, yc] = null;
                xc += dirChange[dir, 0];
                yc += dirChange[dir, 1];
            }
        }

        /*--------判断方向是否满足要求--------*/
        bool checkDir()
        {
            if (dirNum[0] < 2 || dirNum[2] < 2 || dirNum[3] < 2 || dirNum[1] < 3)
                return false;
            for (int i = 4; i < 8; i++)
            {
                if (dirNum[i] < 3)
                    return false;
            }
            return true;
        }

        /*--------类似深搜的过程放单词--------*/
        void searchWord(int cNum)
        {
            if (suc)
                return;
            if (cNum >= wordList.Count - 3)
            {
                if (Math.Abs(mapN - mapM) < 1) //&& checkDir())
                {
                    suc = true;
                }
                return;
            }
            MyWord word = wordList[cNum];
            for (int i = 1; i <= mapN; i++)
            {
                for (int j = 1; j <= mapM; j++)
                {
                    for (int dir = 0; dir < 8; dir++)
                    {
                        if (!canPos(i, j, dir, word.s))
                            continue;
                        int tN = i + (word.s.Length - 1) * dirChange[dir, 0], tM = j + (word.s.Length - 1) * dirChange[dir, 1];
                        int deltaN = 0, deltaM = 0;
                        if (tN > mapN) deltaN = tN - mapN;
                        if (tM > mapM) deltaM = tM - mapM;
                        if (mapN > mapM && deltaN > deltaM)
                            continue;
                        if (mapM > mapN && deltaM > deltaN)
                            continue;
                        posWord(i, j, dir, cNum);
                        dirNum[dir]++;
                        mapN += deltaN;
                        mapM += deltaM;
                        searchWord(cNum + 1);
                        if (suc)
                            return;
                        mapN -= deltaN;
                        mapM -= deltaM;
                        dirNum[dir]--;
                        removePos(i, j, dir, cNum);
                    }
                }
            }
        }

        /*--------调试用输出--------*/
        void writeResult()
        {
            for (int i = 1; i <= mapN; i++)
            {
                for (int j = 1; j <= mapM; j++)
                {
                    if (map[i, j] != null)
                        Console.Write(map[i, j].c);
                    else
                        Console.Write(" ");
                }
                Console.WriteLine();
            }
        }

        /*--------UI界面--------*/
        public void showUI()
        {
            int row = mapN;
            int col = mapM;
            Label[,] label = new Label[row + 1, col + 1];
            int height = this.Height;
            int width = this.Width;
            int uheight = 30;
            int uwidth = 30;
            Random ra = new Random();
            for (int i = 1; i <= row; i++)
            {
                for (int j = 1; j <= col; j++)
                {
                    label[i, j] = new Label();
                    label[i, j].BorderStyle = BorderStyle.FixedSingle;
                    label[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    label[i, j].Font = new Font("Courier New", 20, FontStyle.Bold);
                    if (map[i, j] == null)
                    {
                        label[i, j].Text = Convert.ToString((char)(ra.Next(0, 26) + 'A'));
                    }
                    else
                    {
                        label[i, j].Text = Convert.ToString(map[i, j].c);
                        label[i, j].BackColor = Color.Yellow;
                    }
                    label[i, j].SetBounds(j * uwidth, i * uheight, uwidth, uheight);
                    label[i, j].Visible = true;
                    this.Controls.Add(label[i, j]);
                }
            }
        }


        public Form1()
        {
            InitializeComponent();
            readFile();
            randomReverse();
            initSearch();
            writeResult();
            showUI();
        }
    }
}
