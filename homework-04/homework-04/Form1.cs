using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace homework_04
{
    public partial class Form1 : Form
    {
        /*------变量定义------*/
        #region
        public const int LEN = 120;
        public int n = 0;
        public int pos = 0;
        public int row = 0;
        public int col = 0;
        public int len = 0;
        public char[,] map = new char[LEN, LEN];
        public ArrayList str = new ArrayList();
        #endregion
        /*------构造函数------*/
        public Form1()
        {
            InitializeComponent();
            init();
            input();
            work();
        //    output();
            showUI();
        }
        /*------显示UI------*/
        public void showUI()
        {
            Label[,] label = new Label[row + 1, col + 1];
            int height = this.Height;
            int width = this.Width;
            int uheight=30;
            int uwidth=30;
            Random ra=new Random();
            for (int i = 0; i <= row; i++)
            {
                for (int j = 0; j <= col; j++)
                {
                    label[i, j] = new Label();
                    label[i, j].BorderStyle = BorderStyle.FixedSingle;
                    label[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    label[i, j].Font = new Font("Courier New", 20, FontStyle.Bold);
                    if (map[i, j] == '\0')
                    {
                        label[i, j].Text = Convert.ToString((char)(ra.Next(0, 26) + 'A'));
                    }
                    else
                    {
                        label[i, j].Text = Convert.ToString(map[i, j]);
                        label[i, j].BackColor = Color.Yellow;
                    }
                    label[i, j].SetBounds(j * uwidth, i * uheight, uwidth, uheight);
                    label[i, j].Visible = true;
                    this.Controls.Add(label[i, j]);
                }
            }
        }
        /*------初始化矩阵------*/
        public void init()
        {
            for (int i = 0; i < LEN; i++)
            {
                for (int j = 0; j < LEN; j++)
                {
                    map[i, j] = '\0';
                }
            }
        }
        /*------读文件------*/
        public void input()
        {
            StreamReader sr = new StreamReader("input.txt");
            Console.SetIn(sr);
            string tempstr;
            while ((tempstr = Console.ReadLine()) != null)
            {
                char ch = tempstr[0];
                if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z')
                {
                    tempstr.ToUpper();
                    str.Add(tempstr);
                }
            }
            n = str.Count;
        }
        /*------测试用------*/
        public void test()
        {
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine(str[i]);
            }
        }
        /*------主逻辑部分------*/
        public void work()
        {
            MyCmp cmp = new MyCmp();
            str.Sort(cmp);//单词按照长度从小到大排序
            Swap(0, n - 2);//为保证不冲突,开始构造对角线时选择一个最长的单词
            Swap(4, n - 1);
            string[] str1 = new string[4];//分别存对角线两种方向的单词
            string[] str2 = new string[4];
            for (int i = 0; i < 4; i++)
            {
                str2[i] = str[i].ToString();
            }
            for (int i = 4; i < 8; i++)
            {
                str1[i - 4] = str[i].ToString();
            }
            int lastlen = 0;
            for (pos = 0; pos < 4; pos++)//左上->右下方向填四个单词(提前逆序两个单词)
            {
                if (pos % 2 == 1)
                {
                    str1[pos] = Reverse(str1[pos]);
                }
                len = str1[pos].Length;
                for (int i = 0; i < len; i++)
                {
                    map[i + lastlen, i + lastlen] = str1[pos][i];
                }
                lastlen += len;
            }
            row = col = lastlen - 1;
            bool flag = false;//false表示偶数
            int mark = 0;
            if (lastlen % 2 == 1)//若为奇数,为保证不冲突,需空出一列
            {
                mark = lastlen;
                flag = true;
                row++;
            }
            lastlen = 0;
            for (pos = 0; pos < 4; pos++)//左下->右上方向填四个单词(提前逆序两个单词)
            {
                if (pos % 2 == 1)
                {
                    str2[pos] = Reverse(str2[pos]);
                }
                len = str2[pos].Length;
                for (int i = 0; i < len; i++)
                {
                    map[row - i - lastlen, i + lastlen] = str2[pos][i];
                }
                lastlen += len;
            }
            col = row;
            
            if (n % 2 == 1)//若n为奇数,则剩下3个用来特判
            {
                for (pos = 0; pos < n - 11; pos++)
                {
                    if (pos % 2 == 1)
                    {
                        if (pos % 4 == 1)
                        {
                            str[pos + 8] = Reverse(str[pos + 8].ToString());
                        }
                        row++;
                        len = str[pos + 8].ToString().Length;
                        for (int j = 0; j < len; j++)
                        {
                            map[row, j] = str[pos + 8].ToString()[j];
                        }
                    }
                    else
                    {
                        if (pos % 4 == 2)
                        {
                            str[pos + 8] = Reverse(str[pos + 8].ToString());
                        }
                        col++;
                        len = str[pos + 8].ToString().Length;
                        for (int i = 0; i < len; i++)
                        {
                            map[i, col] = str[pos + 8].ToString()[i];
                        }
                    }
                }
                if (flag)//若之前空了一列,那么填充它,剩下两个单词放在右下角
                {
                    len = str[n - 3].ToString().Length;
                    for (int i = 0; i < len; i++)
                    {
                        map[i, mark] = str[n - 3].ToString()[i];
                    }
                    len = str[n - 2].ToString().Length;
                    for (int i = row; i > row - len; i--)
                    {
                        map[i, col] = str[n - 2].ToString()[row - i];
                    }
                    len = str[n - 1].ToString().Length;
                    for (int j = col - 1; j > col - len - 1; j--)
                    {
                        map[row, j] = str[n - 1].ToString()[col - 1 - j];
                    }
                }
                else//若之前没有空,那么其中两个单词照样填,剩一个放在右下角
                {
                    row++;
                    len = str[n - 3].ToString().Length;
                    for (int j = 0; j < len; j++)
                    {
                        map[row, j] = str[n - 3].ToString()[j];
                    }
                    col++;
                    len = str[n - 2].ToString().Length;
                    for (int i = 0; i < len; i++)
                    {
                        map[i, col] = str[n - 2].ToString()[i];
                    }
                    for (int i = row; i > row - len; i--)
                    {
                        map[i, col] = str[n - 1].ToString()[row - i];
                    }
                }
            }
            else//若n为偶数,则剩下2个单词用来特判
            {
                for (pos = 0; pos < n - 10; pos++)
                {
                    if (pos % 2 == 1)
                    {
                        if (pos % 4 == 1)
                        {
                            str[pos + 8] = Reverse(str[pos + 8].ToString());
                        }
                        row++;
                        len = str[pos + 8].ToString().Length;
                        for (int j = 0; j < len; j++)
                        {
                            map[row, j] = str[pos + 8].ToString()[j];
                        }
                    }
                    else
                    {
                        if (pos % 4 == 2)
                        {
                            str[pos + 8] = Reverse(str[pos + 8].ToString());
                        }
                        col++;
                        len = str[pos + 8].ToString().Length;
                        for (int i = 0; i < len; i++)
                        {
                            map[i, col] = str[pos + 8].ToString()[i];
                        }
                    }
                }
                if (flag)
                {
                    len = str[n - 2].ToString().Length;
                    for (int i = 0; i < len; i++)
                    {
                        map[i, mark] = str[n - 2].ToString()[i];
                    }
                    for (int i = row; i > row - len; i--)
                    {
                        map[i, col] = str[n - 1].ToString()[row - i];
                    }
                }
                else
                {
                    len = str[n - 2].ToString().Length;
                    for (int i = row; i > row - len; i--)
                    {
                        map[i, col] = str[n - 2].ToString()[row - i];
                    }
                    len = str[n - 1].ToString().Length;
                    for (int j = col - 1; j > col - len - 1; j--)
                    {
                        map[row, j] = str[n - 1].ToString()[col - 1 - j];
                    }
                }
            }
        }
        /*------输出部分------*/
        public void output()
        {
            Random ra = new Random();
            for (int i = 0; i < LEN; i++)
            {
                for (int j = 0; j < LEN; j++)
                {
                    if (map[i, j] == '\0')
                    {
                        map[i, j] = (char)(ra.Next(0,26)+'A');
                    }
                }
            }
        }
        /*------自己写的交换函数------*/
        public void Swap(int x, int y)
        {
            string temp;
            temp = str[x].ToString();
            str[x] = str[y];
            str[y] = temp;
        }
        /*------自己写的字符串反转函数------*/
        public string Reverse(string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

    }
    /*------实现自定义排序接口,根据字符串长度排序------*/
    public class MyCmp : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            string xx = x as string;
            string yy = y as string;
            return xx.Length.CompareTo(yy.Length);
        }
    }
}
