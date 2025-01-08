using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Windows.Forms; 
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        FbConnection fbCon;
        DataSet dsPatients;
        readonly string sqlDoctors = "SELECT * FROM doctors WHERE IsDeleted = 0";
        readonly string sqlPatients = "SELECT * FROM patients WHERE IsDeleted = 0";
        private DataTable orglDataTable1;
        private DataTable orglDataTable2;
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            ShowAuthorizationForm();
        }
        private void LoadDoctors()
        {
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Doctors WHERE IsDeleted = 0";

                using (FbDataAdapter adapter = new FbDataAdapter(query, connection))
                {
                    DataTable doctorsTable = new DataTable();
                    adapter.Fill(doctorsTable);

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = doctorsTable;

                    dataGridView1.Columns[0].HeaderText = "Фото";
                    dataGridView1.Columns["Surname_d"].HeaderText = "Фамилия";
                    dataGridView1.Columns["First_Name_d"].HeaderText = "Имя";
                    dataGridView1.Columns["Patronymic_d"].HeaderText = "Отчество";
                    dataGridView1.Columns["Specialty"].HeaderText = "Специальность";
                    dataGridView1.Columns["Experience"].HeaderText = "Стаж (лет)";

                    dataGridView1.Columns["Photo"].DisplayIndex = 0;
                    dataGridView1.Columns["Surname_d"].DisplayIndex = 1;
                    dataGridView1.Columns["First_Name_d"].DisplayIndex = 2;
                    dataGridView1.Columns["Patronymic_d"].DisplayIndex = 3;
                    dataGridView1.Columns["Specialty"].DisplayIndex = 4;
                    dataGridView1.Columns["Experience"].DisplayIndex = 5;

                    dataGridView1.Columns["Photo"].Width = 53;
                    dataGridView1.Columns["Surname_d"].Width = 116;
                    dataGridView1.Columns["First_Name_d"].Width = 116;
                    dataGridView1.Columns["Patronymic_d"].Width = 116;
                    dataGridView1.Columns["Specialty"].Width = 116;
                    dataGridView1.Columns["Experience"].Width = 116;

                    dataGridView1.Columns["DoctorId"].Visible = false;
                    dataGridView1.Columns["ISDELETED"].Visible = false;

                    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
            }
        }
        private void LoadPatients()
        {
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM patients WHERE IsDeleted = 0";

                using (FbDataAdapter adapter = new FbDataAdapter(query, connection))
                {
                    DataTable patientsTable = new DataTable();
                    adapter.Fill(patientsTable);

                    dataGridView2.DataSource = null;
                    dataGridView2.DataSource = patientsTable;

                    // Настраиваем заголовки столбцов
                    dataGridView2.Columns["Surname_d"].HeaderText = "Фамилия";
                    dataGridView2.Columns["First_Name_d"].HeaderText = "Имя";
                    dataGridView2.Columns["Patronymic_d"].HeaderText = "Отчество";
                    dataGridView2.Columns["SNILS"].HeaderText = "СНИЛС";
                    dataGridView2.Columns["BirthDate"].HeaderText = "Дата рождения";
                    dataGridView2.Columns["Address"].HeaderText = "Адрес места жительства";

                    // Устанавливаем ширину столбцов
                    dataGridView2.Columns["Surname_d"].Width = 100;
                    dataGridView2.Columns["First_Name_d"].Width = 100;
                    dataGridView2.Columns["Patronymic_d"].Width = 100;
                    dataGridView2.Columns["SNILS"].Width = 100;
                    dataGridView2.Columns["BirthDate"].Width = 120;
                    dataGridView2.Columns["Address"].Width = 250;

                    // Устанавливаем порядок столбцов
                    dataGridView2.Columns["Surname_d"].DisplayIndex = 0;
                    dataGridView2.Columns["First_Name_d"].DisplayIndex = 1;
                    dataGridView2.Columns["Patronymic_d"].DisplayIndex = 2;
                    dataGridView2.Columns["SNILS"].DisplayIndex = 3;
                    dataGridView2.Columns["BirthDate"].DisplayIndex = 4;
                    dataGridView2.Columns["Address"].DisplayIndex = 5;

                    // Скрываем ненужные столбцы
                    dataGridView2.Columns["PatientId"].Visible = false;
                    dataGridView2.Columns["ISDELETED"].Visible = false;

                    dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
            }
        }
        void Form1_Load(object sender, EventArgs e)
        {
            var fb_cons = new FbConnectionStringBuilder
            {
                DataSource = "localhost",
                Port = 3050,
                Role = "",
                Dialect = 3,
                Charset = "WIN1251",
                UserID = "SYSDBA",
                Password = "masterkey",
                Database = @"C:\Users\katerina\Desktop\Новая папка\3 курс\Курсовая работа. Базы данных\Базы данных\1.5.2\CLINICA.FDB"
            };
            fbCon = new FbConnection(fb_cons.ToString());
            try
            {
                fbCon.Open();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}");
            }
        }
        private void ShowAuthorizationForm()
        {
            Form3 form3 = new Form3();
            if (form3.ShowDialog() == DialogResult.OK)
            {
                this.Show();
            }
            else
            {
                Application.Exit();
            }
        }
        void LoadData()
        {
            try
            {
                using (FbDataAdapter adapterDoctors = new FbDataAdapter(sqlDoctors, fbCon))
                {
                    DataTable doctorsTable = new DataTable();
                    adapterDoctors.Fill(doctorsTable);

                    orglDataTable1 = doctorsTable.Copy();

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = doctorsTable;

                    // Настраиваем заголовки и ширину столбцов
                    dataGridView1.Columns[0].HeaderText = "Фото";
                    dataGridView1.Columns["Surname_d"].HeaderText = "Фамилия";
                    dataGridView1.Columns["First_Name_d"].HeaderText = "Имя";
                    dataGridView1.Columns["Patronymic_d"].HeaderText = "Отчество";
                    dataGridView1.Columns["Specialty"].HeaderText = "Специальность";
                    dataGridView1.Columns["Experience"].HeaderText = "Стаж (лет)";

                    // Устанавливаем порядок столбцов
                    dataGridView1.Columns["Photo"].DisplayIndex = 0;
                    dataGridView1.Columns["Surname_d"].DisplayIndex = 1;
                    dataGridView1.Columns["First_Name_d"].DisplayIndex = 2;
                    dataGridView1.Columns["Patronymic_d"].DisplayIndex = 3;
                    dataGridView1.Columns["Specialty"].DisplayIndex = 4;
                    dataGridView1.Columns["Experience"].DisplayIndex = 5;

                    // Устанавливаем ширину столбцов
                    dataGridView1.Columns["Photo"].Width = 53;
                    dataGridView1.Columns["Surname_d"].Width = 116;
                    dataGridView1.Columns["First_Name_d"].Width = 116;
                    dataGridView1.Columns["Patronymic_d"].Width = 116;
                    dataGridView1.Columns["Specialty"].Width = 116;
                    dataGridView1.Columns["Experience"].Width = 116;

                    // Скрываем ненужные столбцы
                    dataGridView1.Columns["DoctorId"].Visible = false;
                    dataGridView1.Columns["ISDELETED"].Visible = false;

                    dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
                dsPatients = new DataSet();
                using (FbDataAdapter adapterPatients = new FbDataAdapter(sqlPatients, fbCon))
                {
                    DataTable patientsTable = new DataTable();
                    adapterPatients.Fill(patientsTable);

                    orglDataTable2 = patientsTable.Copy();

                    dataGridView2.DataSource = null;
                    dataGridView2.DataSource = patientsTable;

                    // Настраиваем заголовки столбцов
                    dataGridView2.Columns["Surname_d"].HeaderText = "Фамилия";
                    dataGridView2.Columns["First_Name_d"].HeaderText = "Имя";
                    dataGridView2.Columns["Patronymic_d"].HeaderText = "Отчество";
                    dataGridView2.Columns["SNILS"].HeaderText = "СНИЛС";
                    dataGridView2.Columns["BirthDate"].HeaderText = "Дата рождения";
                    dataGridView2.Columns["Address"].HeaderText = "Адрес места жительства";

                    // Устанавливаем ширину столбцов
                    dataGridView2.Columns["Surname_d"].Width = 100;
                    dataGridView2.Columns["First_Name_d"].Width = 100;
                    dataGridView2.Columns["Patronymic_d"].Width = 100;
                    dataGridView2.Columns["SNILS"].Width = 100;
                    dataGridView2.Columns["BirthDate"].Width = 120;
                    dataGridView2.Columns["Address"].Width = 250;

                    // Устанавливаем порядок столбцов
                    dataGridView2.Columns["Surname_d"].DisplayIndex = 0;
                    dataGridView2.Columns["First_Name_d"].DisplayIndex = 1;
                    dataGridView2.Columns["Patronymic_d"].DisplayIndex = 2;
                    dataGridView2.Columns["SNILS"].DisplayIndex = 3;
                    dataGridView2.Columns["BirthDate"].DisplayIndex = 4;
                    dataGridView2.Columns["Address"].DisplayIndex = 5;

                    // Скрываем ненужные столбцы
                    dataGridView2.Columns["PatientId"].Visible = false;
                    dataGridView2.Columns["ISDELETED"].Visible = false;

                    dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void DisVisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var surname = dataGridView1.CurrentRow.Cells["Surname_d"].Value?.ToString();
            var firstName = dataGridView1.CurrentRow.Cells["First_Name_d"].Value?.ToString();
            var patronymic = dataGridView1.CurrentRow.Cells["Patronymic_d"].Value?.ToString();
            var specialty = dataGridView1.CurrentRow.Cells["Specialty"].Value?.ToString();
            var experience = dataGridView1.CurrentRow.Cells["Experience"].Value != null ? Convert.ToInt32(dataGridView1.CurrentRow.Cells["Experience"].Value) : (int?)null;

            if (string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(patronymic) || string.IsNullOrEmpty(specialty))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return;
            }

            if (!IsCyrillic(specialty))
            {
                MessageBox.Show("Специальность должна содержать только русские буквы.");
                return;
            }
            if (!IsCyrillic(surname) || !IsCyrillic(firstName) || !IsCyrillic(patronymic))
            {
                MessageBox.Show("ФИО должно содержать только русские буквы.");
                return;
            }
            if (!char.IsUpper(specialty[0]))
            {
                MessageBox.Show("Специальность должна начинаться с заглавной буквы.");
                return;
            }
            string[] nameParts = { surname, firstName, patronymic };
            foreach (var part in nameParts)
            {
                if (string.IsNullOrEmpty(part) || !char.IsUpper(part[0]))
                {
                    MessageBox.Show("Каждая часть ФИО должна начинаться с заглавной буквы.");
                    return;
                }
            }
            List<string> validSpecialties = new List<string>();
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();
                using (FbCommand command = new FbCommand("SELECT DISTINCT Specialty FROM Doctors", connection))
                {
                    using (FbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            validSpecialties.Add(reader.GetString(0));
                        }
                    }
                }
            }

            if (!validSpecialties.Contains(specialty))
            {
                MessageBox.Show("Специальность должна соответствовать одной из существующих в базе данных.");
                return;
            }

            bool exists = false;
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();
                using (FbCommand command = new FbCommand("SELECT COUNT(*) FROM Doctors WHERE Surname_d = @Surname_d AND First_Name_d = @First_Name_d AND Patronymic_d = @Patronymic_d AND Specialty = @Specialty", connection))
                {
                    command.Parameters.AddWithValue("@Surname_d", surname);
                    command.Parameters.AddWithValue("@First_Name_d", firstName);
                    command.Parameters.AddWithValue("@Patronymic_d", patronymic);
                    command.Parameters.AddWithValue("@Specialty", specialty);

                    var result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        exists = Convert.ToInt64(result) > 0;
                    }
                    if (exists)
                    {
                        MessageBox.Show("Запись с такими данными уже существует.");
                        return;
                    }
                }
            }
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();
                using (FbCommand command = new FbCommand("INSERT INTO Doctors (Surname_d, First_Name_d, Patronymic_d, Specialty, Experience) VALUES (@Surname_d, @First_Name_d, @Patronymic_d, @Specialty, @Experience)", connection))
                {
                    command.Parameters.AddWithValue("@Surname_d", surname);
                    command.Parameters.AddWithValue("@First_Name_d", firstName);
                    command.Parameters.AddWithValue("@Patronymic_d", patronymic);
                    command.Parameters.AddWithValue("@Specialty", specialty);
                    command.Parameters.AddWithValue("@Experience", experience.HasValue ? (object)experience.Value : DBNull.Value);

                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Запись успешно добавлена.");
                        LoadDoctors();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
                    }
                }
            }
        }

        private bool IsCyrillic(string str)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str, @"^[А-Яа-яЁёs]+$");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string inputText = textSearch1.Text;
            if (!string.IsNullOrEmpty(inputText))
            {
                SearchDoctors(inputText);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите текст для поиска.");
            }
        }

        private void SearchDoctors(string inputText)
        {
            // Проверяем входной текст
            if (string.IsNullOrWhiteSpace(inputText))
            {
                MessageBox.Show("Введите текст для поиска.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Открываем соединение
            try
            {
                // Проверяем состояние соединения перед его открытием
                if (fbCon.State != ConnectionState.Open)
                {
                    fbCon.Open();
                }

                string query = @"
                SELECT 
                Photo, 
                Surname_d, 
                First_Name_d,
                Patronymic_d,
                Specialty,
                Experience,
                ISDELETED
            FROM Doctors 
            WHERE 
                IsDeleted = 0 AND  
                (UPPER(Surname_d) LIKE @InputText OR 
                 UPPER(First_Name_d) LIKE @InputText OR 
                 UPPER(Patronymic_d) LIKE @InputText OR 
                 UPPER(Specialty) LIKE @InputText)
                ";

                using (FbCommand command = new FbCommand(query, fbCon))
                {
                    command.Parameters.AddWithValue("@InputText", "%" + inputText.ToUpper() + "%");

                    using (FbDataAdapter adapter = new FbDataAdapter(command))
                    {
                        DataTable doctorsTable = new DataTable();
                        adapter.Fill(doctorsTable);

                        // Устанавливаем ширину столбцов
                        dataGridView1.Columns["Surname_d"].Width = 119;
                        dataGridView1.Columns["First_Name_d"].Width = 119;
                        dataGridView1.Columns["Patronymic_d"].Width = 119;
                        dataGridView1.Columns["Specialty"].Width = 119;
                        dataGridView1.Columns["Experience"].Width = 119;
                        dataGridView1.Columns["Photo"].Width = 55;
                        dataGridView1.Columns["ISDELETED"].Visible = false;

                        // Проверяем наличие записей
                        if (doctorsTable.Rows.Count == 0)
                        {
                            MessageBox.Show("Запись не найдена.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            dataGridView1.DataSource = doctorsTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
            }
            finally
            {
                // Закрываем соединение
                if (fbCon.State == ConnectionState.Open)
                {
                    fbCon.Close();
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (orglDataTable1 != null)
            {
                // Возвращаем исходную таблицу без фильтрации
                dataGridView1.DataSource = orglDataTable1;
            }
            // Настраиваем заголовки и ширину столбцов
            dataGridView1.Columns[0].HeaderText = "Фото";
            dataGridView1.Columns["Surname_d"].HeaderText = "Фамилия";
            dataGridView1.Columns["First_Name_d"].HeaderText = "Имя";
            dataGridView1.Columns["Patronymic_d"].HeaderText = "Отчество";
            dataGridView1.Columns["Specialty"].HeaderText = "Специальность";
            dataGridView1.Columns["Experience"].HeaderText = "Стаж (лет)";

            // Устанавливаем порядок столбцов
            dataGridView1.Columns["Photo"].DisplayIndex = 0;
            dataGridView1.Columns["Surname_d"].DisplayIndex = 1;
            dataGridView1.Columns["First_Name_d"].DisplayIndex = 2;
            dataGridView1.Columns["Patronymic_d"].DisplayIndex = 3;
            dataGridView1.Columns["Specialty"].DisplayIndex = 4;
            dataGridView1.Columns["Experience"].DisplayIndex = 5;

            // Устанавливаем ширину столбцов
            dataGridView1.Columns["Photo"].Width = 53;
            dataGridView1.Columns["Surname_d"].Width = 116;
            dataGridView1.Columns["First_Name_d"].Width = 116;
            dataGridView1.Columns["Patronymic_d"].Width = 116;
            dataGridView1.Columns["Specialty"].Width = 116;
            dataGridView1.Columns["Experience"].Width = 116;

            // Скрываем ненужные столбцы
            dataGridView1.Columns["DoctorId"].Visible = false;
            dataGridView1.Columns["ISDELETED"].Visible = false;

            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;}

            private void button8_Click(object sender, EventArgs e)
            {
            string inputText = textSearch2.Text;
            if (!string.IsNullOrEmpty(inputText))
            {
                SearchPatients(inputText);
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите текст для поиска.");
            }
        }
        private void SearchPatients(string inputText, DateTime? birthDate = null)
        {
            try
            {
                if (fbCon.State != ConnectionState.Open)
                {
                    fbCon.Open();
                }

                string query = @"
            SELECT
                Surname_d,
                First_Name_d,
                Patronymic_d,
                BirthDate,
                Address,
                snils 
            FROM Patients 
            WHERE 
                (UPPER(TRIM(Surname_d)) LIKE @InputText OR 
                 UPPER(TRIM(First_Name_d)) LIKE @InputText OR 
                 UPPER(TRIM(Patronymic_d)) LIKE @InputText OR
                 snils LIKE @InputText)";

                if (birthDate.HasValue)
                {
                    query += " AND BirthDate = @BirthDate";
                }

                using (FbCommand command = new FbCommand(query, fbCon))
                {
                    string searchText = "%" + inputText.ToUpper().Trim() + "%";
                    command.Parameters.AddWithValue("@InputText", searchText);

                    if (birthDate.HasValue)
                    {
                        command.Parameters.AddWithValue("@BirthDate", birthDate.Value.Date);
                    }

                    FbDataAdapter adapter = new FbDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count == 0)
                    {
                        MessageBox.Show("Запись не найдена.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        dataGridView2.DataSource = dataTable;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка подключения к базе данных: " + ex.Message);
            }
            finally
            {
                // Закрываем соединение, если оно открыто
                if (fbCon.State == ConnectionState.Open)
                {
                    fbCon.Close();
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (orglDataTable2 != null)
            {
                // Возвращаем исходную таблицу без фильтрации
                dataGridView2.DataSource = orglDataTable2;
            }
            dataGridView2.Columns["Surname_d"].HeaderText = "Фамилия";
            dataGridView2.Columns["First_Name_d"].HeaderText = "Имя";
            dataGridView2.Columns["Patronymic_d"].HeaderText = "Отчество";
            dataGridView2.Columns["SNILS"].HeaderText = "СНИЛС";
            dataGridView2.Columns["BirthDate"].HeaderText = "Дата рождения";
            dataGridView2.Columns["Address"].HeaderText = "Адрес места жительства";

            // Устанавливаем ширину столбцов
            dataGridView2.Columns["Surname_d"].Width = 100;
            dataGridView2.Columns["First_Name_d"].Width = 100;
            dataGridView2.Columns["Patronymic_d"].Width = 100;
            dataGridView2.Columns["SNILS"].Width = 100;
            dataGridView2.Columns["BirthDate"].Width = 120;
            dataGridView2.Columns["Address"].Width = 250;

            // Устанавливаем порядок столбцов
            dataGridView2.Columns["Surname_d"].DisplayIndex = 0;
            dataGridView2.Columns["First_Name_d"].DisplayIndex = 1;
            dataGridView2.Columns["Patronymic_d"].DisplayIndex = 2;
            dataGridView2.Columns["SNILS"].DisplayIndex = 3;
            dataGridView2.Columns["BirthDate"].DisplayIndex = 4;
            dataGridView2.Columns["Address"].DisplayIndex = 5;

            // Скрываем ненужные столбцы
            dataGridView2.Columns["PatientId"].Visible = false;
            dataGridView2.Columns["ISDELETED"].Visible = false;

            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                         "Подтверждение удаления",
                         MessageBoxButtons.YesNo);
                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }

                // Получаем значения из выбранной строки
                string surname = dataGridView1.SelectedRows[0].Cells["Surname_d"].Value.ToString();
                string firstName = dataGridView1.SelectedRows[0].Cells["First_Name_d"].Value.ToString();
                string patronymic = dataGridView1.SelectedRows[0].Cells["Patronymic_d"].Value.ToString();

                SoftDeleteDoctor(surname, firstName, patronymic);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите врача для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SoftDeleteDoctor(string surname, string firstName, string patronymic)
        {
            using (var connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();

                // Обновляем запрос для использования новых столбцов
                using (var command = new FbCommand("UPDATE Doctors SET IsDeleted = 1 WHERE Surname_d = @Surname AND First_Name_d = @FirstName AND Patronymic_d = @Patronymic AND IsDeleted = 0", connection))
                {
                    command.Parameters.AddWithValue("@Surname", surname);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@Patronymic", patronymic);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        LoadDoctors();
                        MessageBox.Show("Врач успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Врач не найден или уже удален.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования.");
                return;
            }

            var surname = dataGridView1.CurrentRow.Cells["Surname_d"].Value.ToString();
            var firstName = dataGridView1.CurrentRow.Cells["First_Name_d"].Value.ToString();
            var patronymic = dataGridView1.CurrentRow.Cells["Patronymic_d"].Value.ToString();
            var specialty = dataGridView1.CurrentRow.Cells["Specialty"].Value.ToString();
            var experience = dataGridView1.CurrentRow.Cells["Experience"].Value;

            if (!IsCyrillic(specialty))
            {
                MessageBox.Show("Специальность должна содержать только русские буквы.");
                return;
            }
            if (!char.IsUpper(specialty[0]))
            {
                MessageBox.Show("Специальность должна начинаться с заглавной буквы.");
                return;
            }

            // Проверка ФИО
            if (string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(patronymic))
            {
                MessageBox.Show("ФИО не может быть пустым.");
                return;
            }

            if (!char.IsUpper(surname[0]) || !char.IsUpper(firstName[0]) || !char.IsUpper(patronymic[0]))
            {
                MessageBox.Show("Каждая часть ФИО должна начинаться с заглавной буквы.");
                return;
            }
            List<string> validSpecialties = new List<string>();
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();
                using (FbCommand command = new FbCommand("SELECT DISTINCT Specialty FROM Doctors", connection))
                {
                    using (FbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            validSpecialties.Add(reader.GetString(0));
                        }
                    }
                }
            }
            if (!validSpecialties.Contains(specialty))
            {
                MessageBox.Show("Специальность должна соответствовать одной из существующих в базе данных.");
                return;
            }
            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();
                using (FbCommand command = new FbCommand("UPDATE Doctors SET Surname_d = @Surname, First_Name_d = @FirstName, Patronymic_d = @Patronymic, Specialty = @Specialty, Experience = @Experience WHERE Surname_d = @OldSurname AND First_Name_d = @OldFirstName AND Patronymic_d = @OldPatronymic", connection))
                {
                    command.Parameters.AddWithValue("@Surname", surname);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@Patronymic", patronymic);
                    command.Parameters.AddWithValue("@Specialty", specialty);
                    command.Parameters.AddWithValue("@Experience", experience ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@OldSurname", dataGridView1.CurrentRow.Cells["Surname_d"].Value.ToString());
                    command.Parameters.AddWithValue("@OldFirstName", dataGridView1.CurrentRow.Cells["First_Name_d"].Value.ToString());
                    command.Parameters.AddWithValue("@OldPatronymic", dataGridView1.CurrentRow.Cells["Patronymic_d"].Value.ToString());

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Запись успешно обновлена.");
                            LoadDoctors();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить запись. Возможно, она была изменена другим пользователем.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}");
                    }
                }
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            var surname = dataGridView2.CurrentRow.Cells["Surname_d"].Value?.ToString();
            var firstName = dataGridView2.CurrentRow.Cells["First_Name_d"].Value?.ToString();
            var patronymic = dataGridView2.CurrentRow.Cells["Patronymic_d"].Value?.ToString();
            var birthdate = dataGridView2.CurrentRow.Cells["BirthDate"].Value?.ToString();
            var address = dataGridView2.CurrentRow.Cells["Address"].Value?.ToString();
            var snils = dataGridView2.CurrentRow.Cells["Snils"].Value?.ToString();


            string[] requiredFields = { surname, firstName, patronymic, birthdate, address, snils };
            if (requiredFields.Any(string.IsNullOrEmpty))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return;
            }
            string[] nameParts = { surname, firstName, patronymic, address }; 
            if (nameParts.Any(part => !IsCyrillic(part)))
            {
                MessageBox.Show("ФИО и адрес должны содержать только русские буквы.");
                return;
            }
           
            foreach (var part in nameParts)
            {
                if (string.IsNullOrEmpty(part) || !char.IsUpper(part[0]))
                {
                    MessageBox.Show("Каждая часть ФИО должна начинаться с заглавной буквы.");
                    return;
                }
            }

            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
                {
                connection.Open();
                using (FbCommand checkDuplicateCommand = new FbCommand("SELECT COUNT(*) FROM Patients WHERE Surname_d = @Surname AND First_Name_d = @FirstName AND Patronymic_d = @Patronymic AND BirthDate = @BirthDate", connection))
                {
                    checkDuplicateCommand.Parameters.AddWithValue("@Surname", surname);
                    checkDuplicateCommand.Parameters.AddWithValue("@FirstName", firstName);
                    checkDuplicateCommand.Parameters.AddWithValue("@Patronymic", patronymic);
                    checkDuplicateCommand.Parameters.AddWithValue("@BirthDate", birthdate);

                    var duplicateCount = Convert.ToInt32(checkDuplicateCommand.ExecuteScalar());
                    if (duplicateCount > 0)
                    {
                        MessageBox.Show("Запись с такими данными уже существует.");
                        return;
                    }
                }
                using (FbCommand command = new FbCommand("INSERT INTO Patients (Surname_d, First_Name_d, Patronymic_d, BirthDate, Address, Snils) VALUES (@Surname, @FirstName, @Patronymic, @BirthDate, @Address, @Snils)", connection))
                {
                    command.Parameters.AddWithValue("@Surname", surname);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@Patronymic", patronymic);
                    command.Parameters.AddWithValue("@BirthDate", birthdate); ;
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Snils", snils);

                    try
                    {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Запись успешно добавлена.");
                        LoadPatients();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при добавлении записи: {ex.Message}");
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                var confirmResult = MessageBox.Show("Вы уверены, что хотите удалить эту запись?",
                         "Подтверждение удаления",
                         MessageBoxButtons.YesNo);
                if (confirmResult != DialogResult.Yes)
                {
                    return;
                }

                string surname = dataGridView2.SelectedRows[0].Cells["Surname_d"].Value.ToString();
                string firstName = dataGridView2.SelectedRows[0].Cells["First_Name_d"].Value.ToString();
                string patronymic = dataGridView2.SelectedRows[0].Cells["Patronymic_d"].Value.ToString();

                SoftDeletePatients(surname, firstName, patronymic);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пациента для удаления.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SoftDeletePatients(string surname, string firstName, string patronymic)
        {
            using (var connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();
                using (var command = new FbCommand("UPDATE Patients SET IsDeleted = 1 WHERE Surname_d = @Surname AND First_Name_d = @FirstName AND Patronymic_d = @Patronymic AND IsDeleted = 0", connection))
                {
                    command.Parameters.AddWithValue("@Surname", surname);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@Patronymic", patronymic);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        LoadPatients();
                        MessageBox.Show("Пациент успешно удален.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Пациент не найден или уже удален.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для редактирования.");
                return;
            }

            var surname = dataGridView2.CurrentRow.Cells["Surname_d"].Value?.ToString();
            var firstName = dataGridView2.CurrentRow.Cells["First_Name_d"].Value?.ToString();
            var patronymic = dataGridView2.CurrentRow.Cells["Patronymic_d"].Value?.ToString();
            var birthdate = dataGridView2.CurrentRow.Cells["BirthDate"].Value?.ToString();
            var address = dataGridView2.CurrentRow.Cells["Address"].Value?.ToString();

            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(birthdate))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                return;
            }

            DateTime birthDate;
            if (!DateTime.TryParseExact(birthdate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate))
            {
                MessageBox.Show("Пожалуйста, введите корректную дату рождения в формате дд.мм.гггг.");
                return;
            }

            if (birthDate.Year < 1915 || birthDate.Year > 2050)
            {
                MessageBox.Show("Год рождения должен быть в диапазоне от 1915 до 2050.");
                return;
            }

            var oldAddress = dataGridView2.CurrentRow.Cells["Address"].Value?.ToString();

            using (FbConnection connection = new FbConnection(fbCon.ConnectionString))
            {
                connection.Open();
                using (FbCommand command = new FbCommand("UPDATE Patients SET BirthDate = @BirthDate, Address = @Address WHERE Surname_d = @Surname AND First_Name_d = @FirstName AND Patronymic_d = @Patronymic AND Address = @OldAddress", connection))
                {
                    command.Parameters.AddWithValue("@BirthDate", birthDate);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@Surname", surname);
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@Patronymic", patronymic);
                    command.Parameters.AddWithValue("@OldAddress", oldAddress);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Запись успешно обновлена.");
                            LoadPatients();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить запись. Возможно, она была изменена другим пользователем.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при обновлении записи: {ex.Message}");
                    }
                }
            }
        }
    }
}