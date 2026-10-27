using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Bakery_MVP
{
    public partial class Form1 : Form
    {
        // Строка подключения к СУБД Access
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\bakery.accdb;";

        public Form1()
        {
            InitializeComponent();
            // Гарантируем запуск чтения базы данных при старте программы
            this.Load += new System.EventHandler(this.Form1_Load);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RefreshErpGrids();
        }

        public void RefreshErpGrids()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // 1. Вкладка "Склад сырья": Загружаем данные в dataGridViewSklad
                    string queryIngredients = "SELECT * FROM Ингридиенты";
                    OleDbDataAdapter adapter1 = new OleDbDataAdapter(queryIngredients, connection);
                    DataTable dtIngredients = new DataTable();
                    adapter1.Fill(dtIngredients);
                    if (dataGridViewSklad != null) dataGridViewSklad.DataSource = dtIngredients;

                    // 2. Вкладка "Очередь кухни": Загружаем данные в dataGridViewKitchen
                    string queryOrders = "SELECT * FROM Заказы ORDER BY ID_Заказа DESC";
                    OleDbDataAdapter adapter2 = new OleDbDataAdapter(queryOrders, connection);
                    DataTable dtOrders = new DataTable();
                    adapter2.Fill(dtOrders);
                    if (dataGridViewKitchen != null) dataGridViewKitchen.DataSource = dtOrders;

                    // 3. Вкладка "Продажи": Загружаем витрину в dataGridViewShowcase
                    string queryProducts = "SELECT * FROM Продукция";
                    OleDbDataAdapter adapter3 = new OleDbDataAdapter(queryProducts, connection);
                    DataTable dtProducts = new DataTable();
                    adapter3.Fill(dtProducts);
                    if (dataGridViewShowcase != null) dataGridViewShowcase.DataSource = dtProducts;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка чтения базы данных Access: " + ex.Message, "ERP Контроль");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Заполните имя и телефон клиента!", "Внимание");
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string orderQuery = "INSERT INTO Заказы (ID_Сотрудника, [Имя клиента], [Телефон клиента], [Способ оплаты], [Статус заказа], [Дата и время заказа]) " +
                                        "VALUES (1, @name, @phone, @payment, 'Новый', @date)";

                    OleDbCommand orderCmd = new OleDbCommand(orderQuery, connection);
                    orderCmd.Parameters.AddWithValue("@name", textBox1.Text);
                    orderCmd.Parameters.AddWithValue("@phone", textBox2.Text);
                    orderCmd.Parameters.AddWithValue("@payment", comboBox1.SelectedItem?.ToString() ?? "Карта");
                    orderCmd.Parameters.AddWithValue("@date", DateTime.Now);
                    orderCmd.ExecuteNonQuery();

                    RefreshErpGrids();

                    MessageBox.Show($"Заказ для {textBox1.Text} успешно отправлен на кухню пекарю!\nСтатус: Новый.", "ERP Система");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении заказа: " + ex.Message, "Ошибка ERP");
                }
            }
        }
    }
}
