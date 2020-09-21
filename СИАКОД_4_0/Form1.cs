using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace СИАКОД_4_0
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<Node> nodes = new List<Node>();
        List<Rebro> rebra = new List<Rebro>();
        

        
        private void pBox_Paint(object sender, PaintEventArgs e)
        {
           
            foreach (Rebro r in rebra)
            {
                r.draw(e.Graphics);
            }
            foreach (Node n in nodes)
            {
                n.draw(e.Graphics);
            }
            pBox.Select();
        }

        void link(Node n1,Node n2)
        {
            Rebro r = new Rebro(n1, n2);
            rebra.Add(r);
            n1.linkedN.Add(n2);
            n2.linkedN.Add(n1);
            for (int i = 0; i < n1.linkedN.Count; i++)
            {
                int j = i;
                int f = i;
                while (j != n1.linkedN.Count)
                {
                    if (n1.linkedN[f].num < n1.linkedN[j].num)
                    {
                        Node node = n1.linkedN[j];
                        n1.linkedN[j] = n1.linkedN[f];
                        n1.linkedN[f] = node;
                        f = j;
                    }
                    j++;
                }
            }
            for (int i = 0; i < n2.linkedN.Count; i++)
            {
                int j = i;
                int f = i;
                while (j != n2.linkedN.Count)
                {
                    if (n2.linkedN[f].num < n2.linkedN[j].num)
                    {
                        Node node = n2.linkedN[j];
                        n2.linkedN[j] = n2.linkedN[f];
                        n2.linkedN[f] = node;
                        f = j;
                    }
                    j++;
                }
            }
           
            foreach (Node no in nodes)
             {
                if (no.CheckSelect())
                { 
                    no.Select();
                }
             }
        }

        int next = 0;
        Node n1;
        Node n2;
        int count = 0;
        private void pBox_MouseClick(object sender, MouseEventArgs e)
        {
            int flag = 0;

            if (next == 0)
            {
                foreach (Node n in nodes)
                {
                    if (n.mouseInShape(e.X, e.Y) == true)
                    {
                        next++;
                        flag = 1;
                        n.Select();
                        n1 = n;
                        this.Refresh();
                        pBox.Select();
                        return;
                    }
                }
            }

            if (next == 1)
            {
                foreach (Node n in nodes)
                {
                    if (n.mouseInShape(e.X, e.Y) == true)
                    {
                        flag = 1;
                        if (n.CheckSelect() != true)
                        {
                            next = 0;
                            n.Select();
                            n2 = n;
                            //Это в связывание
                            link(n1, n2);
                            this.Refresh();
                            pBox.Select();
                            return;
                           
                        }
                    }
                }
            }

            if (flag == 0)
            {
                Node n = new Node(e.X, e.Y, count);
                nodes.Add(n);
                count++;
            }
            this.Refresh();
            pBox.Select();
        }

        public void Wait(double seconds)
        {
            int ticks = System.Environment.TickCount + (int)Math.Round(seconds * 1000.0);
            while (System.Environment.TickCount < ticks)
            {
                Application.DoEvents();
            }
        }



       
        List<Node> stack ;
        int c = 0;
        void BFS(Node n, Graphics g)
        {

            n.fill = 1;
            
            n.draw(g);
            Wait(1);
            foreach (Node node in n.linkedN)
            {
                if (!stack.Contains(node))
                {
                    stack.Add(node);
                    Pen pen;
                    pen = new Pen(Brushes.Blue);
                    pen.Width = 4;
                    g.DrawLine(pen, n.x, n.y, node.x, node.y);
                }
            }
            if (c < stack.Count)
            BFS(stack[c++],g);
        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            Graphics g = pBox.CreateGraphics(); ;
        
            stack = new List<Node>();
            int i = Convert.ToInt32(textBox1.Text);
            Node node = nodes[i];
           
            stack.Add(node);
            BFS(node,g);
           // this.Refresh();
            pBox.Select();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            count = 0;
            c = 0;
            label2.Text = "";
            rebra.Clear();
            nodes.Clear();
            this.Refresh();
            pBox.Select();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Graphics g = pBox.CreateGraphics(); 
            c = 0;
            label2.Text = "";
            foreach (Node n in nodes)
            {
                n.nonfillEllipse(g);
            }
            foreach (Rebro r in rebra)
            {
                r.visited = false;
            }
            this.Refresh();
            pBox.Select();
        }

        bool eiler(List<Node> n)
        {
            int count;
            for (int i = 0; i < n.Count; i++)
            {  //проходим все вершины
                Node tmp = n[i];
                count = tmp.linkedN.Count;
                if (count % 2 == 1) return false; // степень нечетная
            }
            return true;   // все степени четные
        }

       

        Rebro GetRebro(Node n1, Node n2)
        {
            IEnumerable<Rebro> myr = from reb in rebra where ((reb.node1 == n1 & reb.node2 == n2) || (reb.node1 == n2 & reb.node2 == n1)) select reb;
            if (myr.Count() > 1 || myr.Count() == 0)
            {
                return null;
            }
            else
            {
                return myr.First();
            }
            //Rebro r = new Rebro(n1, n2);
            //if (rebra.Contains(r))
            //    foreach (Rebro r1 in rebra)
            //    {
            //        if (r1.node1 == n1 && r1.node2 == n2)
            //            return r1;
            //    }
            //return null;
        }

        bool check(Node n)
        {
            int count = 0;
            foreach(Node no in n.linkedN)
            {
                if(GetRebro(n, no).visited == false)
                {
                    count++;
                }
            }
            if (count == 1)
                return true;
            else return false;
        }

        bool check2()
        {
            int count = 0;
            foreach (Rebro r in rebra)
            {
                if (r.visited == false)
                    count++;
            }
            if (count == 1)
                return true;
            else return false;
        }

        void eiler_path(Node n,Graphics g)
        {
            foreach (Node n1 in n.linkedN)
            {
                if (GetRebro(n, n1) != null && GetRebro(n, n1).visited == false)
                {
                    if (n1.first != true)
                    {
                        Wait(1);
                        GetRebro(n, n1).visited = true;
                        Pen pen;
                        pen = new Pen(Brushes.Blue);
                        pen.Width = 4;
                        g.DrawLine(pen, GetRebro(n, n1).node1.x, GetRebro(n, n1).node1.y, GetRebro(n, n1).node2.x, GetRebro(n, n1).node2.y);
                        Wait(1);
                        //label2.Text += GetRebro(n, n1).node1.num.ToString() + " >> " + GetRebro(n, n1).node2.num.ToString() + " ";
                        label2.Text += n1.num.ToString() + " >> ";
                        eiler_path(n1, g);
                    }
                    else if (check(n1))
                    {
                        if (!check2())
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Wait(1);
                        GetRebro(n, n1).visited = true;
                        Pen pen;
                        pen = new Pen(Brushes.Blue);
                        pen.Width = 4;
                        g.DrawLine(pen, GetRebro(n, n1).node1.x, GetRebro(n, n1).node1.y, GetRebro(n, n1).node2.x, GetRebro(n, n1).node2.y);
                        Wait(1);
                        label2.Text += n1.num.ToString() + " >> ";
                        //label2.Text += GetRebro(n, n1).node1.num.ToString() + " >> " + GetRebro(n, n1).node2.num.ToString();
                        eiler_path(n1, g);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Graphics g = pBox.CreateGraphics();
            //List<Node> nodesforpath = new List<Node>();
            //nodesforpath.AddRange(nodes);
            foreach (Rebro r in rebra)
            {
                r.visited = false;
            }
            foreach (Node no in nodes)
            {
                no.first = false;
            }
            label2.Text = "";
            this.Refresh();
            int i = Convert.ToInt32(textBox1.Text);
            Node n = nodes[i];
            n.first = true;
            if (eiler(nodes))
            {
                eiler_path(n,g);
            }
            else label2.Text = "Граф не является эйлеровым";
            
            pBox.Select();
        }
    }
}
