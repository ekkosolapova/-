using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Дополнительная инициализация при загрузке формы (если нужно)
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            // Получаем введенные данные
            string username = textBoxUsername.Text.Trim(); // Убираем пробелы
            string password = textBoxPassword.Text.Trim(); // Убираем пробелы

            // Проверяем, что поля не пустые
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Не закрываем форму, просто показываем сообщение
            }

            // Пример проверки (замените на вашу логику)
            if (IsValidUser(username, password))
            {
                // Если данные верны, закрываем форму с результатом OK
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidUser(string username, string password)
        {
            var users = new Dictionary<string, string>
            {
                { "admin", "masterkey" },
                { "user1", "314159" },
                { "user2", "05031953" },
                { "user3", "19844511962" }
            };
            return users.TryGetValue(username, out string storedPassword) && storedPassword == password;
        }
    }
}
