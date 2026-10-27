using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Bakery_MVP
{
    public partial class Form1 : Form
    {
        // Строка подключения к твоей базе данных Access
        // Файл Bakery.accdb должен лежать в папке проекта вместе с этим кодом!
        string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\Bakery.accdb;";

        public Form1()
        {
            InitializeComponent();
        }

        // Логика кнопки «Оформить заказ» для Версии 1 (MVP)
        private void button1_Click(object sender, EventArgs e)
        {
            // 1. Проверяем, заполнил ли пользователь текстовые поля
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Заполните имя и телефон клиента!", "Внимание");
                return;
            }

            // 2. SQL-запрос для записи в твою таблицу "Заказы"
            string query = "INSERT INTO Заказы (ID_Сотрудника, [Имя клиента], [Телефон клиента], [Способ оплаты], [Статус заказа], [Дата и время заказа]) " +
                           "VALUES (1, @name, @phone, @payment, 'Выдан', @date)";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    // Передаем значения из элементов интерфейса формы
                    command.Parameters.AddWithValue("@name", textBox1.Text);
                    command.Parameters.AddWithValue("@phone", textBox2.Text);
                    command.Parameters.AddWithValue("@payment", comboBox1.SelectedItem?.ToString() ?? "Карта");
                    command.Parameters.AddWithValue("@date", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery(); // Выполняем запись в БД Access
                        MessageBox.Show("Заказ успешно сохранен в базе данных со статусом 'Выдан'!", "MVP Успех");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка работы с базой данных: " + ex.Message, "Ошибка");
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}