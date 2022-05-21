using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using System.Windows.Forms.DataVisualization.Charting;

namespace KC_20_Gimalova_lab9_var4
{
    public partial class Form1 : Form
    {

        // Здесь будем хранить путь к файлу с координатами
        public string pathFile1 { set; get; }
        public string pathFile2 { set; get; }

        public string selectColor;

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //для заполнения в дальнейшем combobox
            cbColors1.Items.Clear();
            cbColors2.Items.Clear();
            string[] colors = Enum.GetNames(typeof(KnownColor));
            cbColors1.Items.AddRange(colors);
            cbColors2.Items.AddRange(colors);

            btnLegendOn.Checked = true;
            btnMarkersOff1.Checked = true;
            btnMarkersOff2.Checked = true;

            //comboBox для выбора типа осей
            AddItemsToComboBox(cbAxisX1);
            AddItemsToComboBox(cbAxisY1);
            AddItemsToComboBox(cbAxisX2);
            AddItemsToComboBox(cbAxisY2);

            //дефолтное значение comboBox
            cbAxisX1.SelectedIndex = 0;
            cbAxisY1.SelectedIndex = 0;
            cbAxisX2.SelectedIndex = 0;
            cbAxisY2.SelectedIndex = 0;

            //добавление значений в comboboxmarkers
            CreateComboBoxMarkers(cbTypeOfMarkers1);
            CreateComboBoxMarkers(cbTypeOfMarkers2);

            //Тип графика
            CreateComboBoxTypesGraph(cbTypeOfGraph);
            CreateComboBoxTypesGraph(cbTypeOfGraph2);

            cbTypeOfGraph.SelectedIndex = 0;
            cbTypeOfGraph2.SelectedIndex = 0;
        }

        


        private void btnCreate1_Click(object sender, EventArgs e)
        {
            
            CreateGraph(dataGridView1,0);
        }

        private void btnImport1_Click(object sender, EventArgs e)
        {
            importTable(pathFile1, dataGridView1,0);
        }

        private void btnImport2_Click(object sender, EventArgs e)
        {
            importTable(pathFile2, dataGridView2,1);
        }

        private void btnCreate2_Click(object sender, EventArgs e)
        {
            CreateGraph(dataGridView2,1);

        }

        private void btnTable1_Click(object sender, EventArgs e)
        {
            CreateTable(dataGridView1, numRows1,0);
        }

        private void btnTable2_Click(object sender, EventArgs e)
        {
            CreateTable(dataGridView2, numRows2,1);
        }

        //запись кол-ва данных в таблицу
        private void CreateTable(DataGridView dataGridView, TextBox numRows,int index)
        {
            if (numRows.Text != "")
            {
                dataGridView.Rows.Clear();
                int rowCount = Convert.ToInt32(numRows.Text);
                dataGridView.RowCount = rowCount;
            }
        }

        //постройка графика
        private void CreateGraph(DataGridView dataGridView, int index)
        {
            chart1.Titles.Clear();
            chart1.Titles.Add("Вариант 04");
            try
            {
                if (dataGridView.RowCount > 0)
                {
                    chart1.Series[index].Points.Clear();

                    for (int i = 0; i < dataGridView.Rows.Count; i++)
                    {
                        double x = Convert.ToDouble(dataGridView.Rows[i].Cells[0].Value);
                        double y = Convert.ToDouble(dataGridView.Rows[i].Cells[1].Value);

                        chart1.Series[index].Points.AddXY(x, y);

                    }
                    
                   /* chart1.Series[index].ChartType = SeriesChartType.Line;*/
                    
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Не верные данные!", "Ошибка",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
             
        }

        //для загрузки в datagridview данных с txt
        private void importTable(string path, DataGridView dataGridView,int index)
        {
            try
            {
                openFileDialog1.ShowDialog(this);
                path = openFileDialog1.FileName;
                string[] coords = File.ReadAllLines(path);
                double x, y = 0;
                List<double> values = new List<double>();
                string[] text_coord;
                dataGridView.Rows.Clear();
                foreach (string s in coords)
                {
                    try
                    {
                        text_coord = s.Split(new char[] { ' ' });
                        x = Convert.ToDouble(text_coord[0]);
                        y = Convert.ToDouble(text_coord[1]);
                        int idx = dataGridView.Rows.Add();
                        dataGridView.Rows[idx].Cells[0].Value = x;
                        dataGridView.Rows[idx].Cells[1].Value = y;
                        values.Add(y);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Ошибка");
                    }
                }
                CreateGraph(dataGridView,index);


            }
            catch
            {
                MessageBox.Show("Внимание! Вы не выбрали файл");
            }
        }

        private void cbColors1_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawItems(e);
        }

        private void cbColors2_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawItems(e);
        }

        //отрисовка прямоугольников-цветов
        private void DrawItems(DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();
                string texto = cbColors1.Items[e.Index].ToString();
                Brush borde = new SolidBrush(e.ForeColor);
                Color color = Color.FromName(texto);
                Brush pincel = new SolidBrush(color);
                Pen pen = new Pen(e.ForeColor);

                e.Graphics.DrawRectangle(pen, new Rectangle(e.Bounds.Left + 2, e.Bounds.Top + 2, 50, e.Bounds.Height - 4));
                e.Graphics.FillRectangle(pincel, new Rectangle(e.Bounds.Left + 3, e.Bounds.Top + 3, 48, e.Bounds.Height - 6));
                e.Graphics.DrawString(texto, e.Font, borde, e.Bounds.Left + 65, e.Bounds.Top + 2);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbColors1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeColor("График 1", cbColors1);
        }
        private void cbColors2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeColor("График 2", cbColors2);
        }

        //изменение цвета графика
        private void ChangeColor(string legend,ComboBox comboBox)
        {
            try
            {
                selectColor = comboBox.SelectedItem.ToString();

                chart1.Series[$"{legend}"].Color = Color.FromName(selectColor);
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно применить фильтр", "Ошмбка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnLegendOff_CheckedChanged(object sender, EventArgs e)
        {
            if (btnLegendOff.Checked)
            {
                chart1.Series["График 1"].IsVisibleInLegend = false;
                chart1.Series["График 2"].IsVisibleInLegend = false;
            }
        }

        private void btnLegendOn_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (btnLegendOn.Checked)
                {
                    chart1.Series["График 1"].IsVisibleInLegend = true;
                    chart1.Series["График 2"].IsVisibleInLegend = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно применить фильтр", "Ошмбка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void cbAxisX1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeAxisX(0, cbAxisX1);
        }

        private void cbAxisY1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeAxisY(0, cbAxisY1);
        }

        private void cbAxisX2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeAxisX(1, cbAxisX2);
        }

        private void cbAxisY2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeAxisY(1, cbAxisY2);
        }

        private void AddItemsToComboBox(ComboBox comboBox)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Items.Clear();
            comboBox.Items.Add("Основная");
            comboBox.Items.Add("Вспомогательная");
        }
        private void ChangeAxisX(int index, ComboBox comboBox)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            try
            {
                switch (comboBox.SelectedIndex)
                {
                    case 0:
                        chart1.Series[index].XAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Primary;
                        break;
                    case 1:
                        chart1.Series[index].XAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
                        break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно применить фильтр","Ошмбка",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
                     
        }

        private void ChangeAxisY(int index, ComboBox comboBox)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            try
            {
                switch (comboBox.SelectedIndex)
                {
                    case 0:
                        chart1.Series[index].YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Primary;
                        break;
                    case 1:
                        chart1.Series[index].YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
                        break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно применить фильтр", "Ошмбка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void widthAxisX_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas["ChartArea1"].AxisX.LineWidth = Convert.ToInt32(widthAxisX.Value);
        }

        private void widthAxisY_ValueChanged(object sender, EventArgs e)
        {
            chart1.ChartAreas["ChartArea1"].AxisY.LineWidth = Convert.ToInt32(widthAxisY.Value);
        }
      

        private void cbTypeOfMarkers1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedMarker(cbTypeOfMarkers1, 0);
        }

        private void cbTypeOfMarkers2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedMarker(cbTypeOfMarkers2, 1);
        }

        private void CreateComboBoxMarkers(ComboBox comboBox)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Items.Clear();
            comboBox.Items.Add("Квадрат");
            comboBox.Items.Add("Круг");
            comboBox.Items.Add("Бриллиант");
            comboBox.Items.Add("Треугольник");
            comboBox.Items.Add("Крест");
            comboBox.Items.Add("Звезда 4");
            comboBox.Items.Add("Звезда 5");
            comboBox.Items.Add("Звезда 6");
            comboBox.Items.Add("Звезда 10");
        }

        private void btnMarkersOn1_CheckedChanged(object sender, EventArgs e)
        {
            labelTypeMarkers1.Enabled = true;
            cbTypeOfMarkers1.Enabled = true;

            cbTypeOfMarkers1.SelectedIndex = 0;
            //дефолтное значение маркера1
            chart1.Series[0].MarkerStyle = MarkerStyle.Square;
        }

        private void btnMarkersOn2_CheckedChanged(object sender, EventArgs e)
        {
            labelTypeMarkers2.Enabled = true;
            cbTypeOfMarkers2.Enabled = true;

            cbTypeOfMarkers2.SelectedIndex = 0;
            //дефолтное значение маркера2
            chart1.Series[1].MarkerStyle = MarkerStyle.Square;
        }
        private void btnMarkersOff1_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[0].MarkerStyle = MarkerStyle.None;
            labelTypeMarkers1.Enabled = false;
            cbTypeOfMarkers1.Enabled = false;
        }

        private void btnMarkersOff2_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[1].MarkerStyle = MarkerStyle.None;
            labelTypeMarkers2.Enabled = false;
            cbTypeOfMarkers2.Enabled = false;
        }

        
        private void SelectedMarker(ComboBox comboBox,int index)
        {
            try
            {
                switch (comboBox.SelectedIndex)
                {
                    case 0:
                        chart1.Series[index].MarkerStyle = MarkerStyle.Square;
                        break;
                    case 1:
                        chart1.Series[index].MarkerStyle = MarkerStyle.Circle;
                        break;
                    case 2:
                        chart1.Series[index].MarkerStyle = MarkerStyle.Diamond;
                        break;
                    case 3:
                        chart1.Series[index].MarkerStyle = MarkerStyle.Triangle;
                        break;
                    case 4:
                        chart1.Series[index].MarkerStyle = MarkerStyle.Cross;
                        break;
                    case 5:
                        chart1.Series[index].MarkerStyle = MarkerStyle.Star4;
                        break;
                    case 6:
                        chart1.Series[index].MarkerStyle = MarkerStyle.Star5;
                        break;
                    case 7:
                        chart1.Series[index].MarkerStyle = MarkerStyle.Star6;
                        break;
                    case 8:
                        chart1.Series[index].MarkerStyle = MarkerStyle.Star10;
                        break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно применить фильтр", "Ошмбка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbTypeOfGraph_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedTypeGraph(cbTypeOfGraph, 0);
        }
        private void cbTypeOfGraph2_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedTypeGraph(cbTypeOfGraph2, 1);
        }

        private void CreateComboBoxTypesGraph(ComboBox comboBox)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Items.Clear();
            comboBox.Items.Add("Сплайн");
            comboBox.Items.Add("Диаграмма");
           
        }

        private void SelectedTypeGraph(ComboBox comboBox, int index)
        {
            try
            {
                switch (comboBox.SelectedIndex)
                {
                    case 0:
                        chart1.Series[index].ChartType = SeriesChartType.Spline;
                        break;
                    case 1:
                        chart1.Series[index].ChartType = SeriesChartType.Column;
                        break;

                }
            }
            catch (Exception)
            {
                MessageBox.Show("Невозможно применить фильтр", "Ошмбка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
    }
}
