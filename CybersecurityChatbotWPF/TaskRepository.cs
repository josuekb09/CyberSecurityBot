using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace CyberSecurityBot
{
    public class TaskRepository
    {
        private const string ServerConnectionString =
     "server=127.0.0.1;port=3306;uid=root;pwd=;SslMode=Disabled;AllowPublicKeyRetrieval=True;Connection Timeout=5;";

        private const string DatabaseConnectionString =
            "server=127.0.0.1;port=3306;database=cybersecuritybot;uid=root;pwd=;SslMode=Disabled;AllowPublicKeyRetrieval=True;Connection Timeout=5;";

        public bool IsDatabaseReady { get; private set; }
        public string LastError { get; private set; } = "";

        public TaskRepository()
        {
            EnsureDatabaseAndTable();
        }

        private void EnsureDatabaseAndTable()
        {
            try
            {
                using (var connection = new MySqlConnection(ServerConnectionString))
                {
                    connection.Open();

                    using var command = new MySqlCommand(
                        "CREATE DATABASE IF NOT EXISTS cybersecuritybot;",
                        connection);

                    command.ExecuteNonQuery();
                }

                using (var connection = new MySqlConnection(DatabaseConnectionString))
                {
                    connection.Open();

                    string createTableSql = @"
                        CREATE TABLE IF NOT EXISTS cyber_tasks (
                            id INT AUTO_INCREMENT PRIMARY KEY,
                            title VARCHAR(150) NOT NULL,
                            description TEXT NOT NULL,
                            reminder_date DATETIME NULL,
                            reminder_shown BOOLEAN NOT NULL DEFAULT FALSE,
                            is_completed BOOLEAN NOT NULL DEFAULT FALSE,
                            created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
                        );";

                    using var command = new MySqlCommand(createTableSql, connection);
                    command.ExecuteNonQuery();
                }

                IsDatabaseReady = true;
                LastError = "";
            }
            catch (Exception ex)
            {
                IsDatabaseReady = false;
                LastError = ex.Message;
            }
        }

        private bool EnsureReady()
        {
            if (!IsDatabaseReady)
            {
                EnsureDatabaseAndTable();
            }

            return IsDatabaseReady;
        }

        public int AddTask(string title, string description, DateTime? reminderDate)
        {
            if (!EnsureReady())
            {
                return -1;
            }

            try
            {
                using var connection = new MySqlConnection(DatabaseConnectionString);
                connection.Open();

                string sql = @"
                    INSERT INTO cyber_tasks (title, description, reminder_date, is_completed)
                    VALUES (@title, @description, @reminderDate, FALSE);";

                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@reminderDate", reminderDate.HasValue ? reminderDate.Value : DBNull.Value);

                command.ExecuteNonQuery();

                return Convert.ToInt32(command.LastInsertedId);
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return -1;
            }
        }

        public List<CyberTask> GetAllTasks()
        {
            var tasks = new List<CyberTask>();

            if (!EnsureReady())
            {
                return tasks;
            }

            try
            {
                using var connection = new MySqlConnection(DatabaseConnectionString);
                connection.Open();

                string sql = @"
                    SELECT id, title, description, reminder_date, is_completed, created_at
                    FROM cyber_tasks
                    ORDER BY is_completed ASC, created_at DESC;";

                using var command = new MySqlCommand(sql, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int reminderIndex = reader.GetOrdinal("reminder_date");

                    tasks.Add(new CyberTask
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        ReminderDate = reader.IsDBNull(reminderIndex) ? null : reader.GetDateTime(reminderIndex),
                        IsCompleted = reader.GetBoolean("is_completed"),
                        CreatedAt = reader.GetDateTime("created_at")
                    });
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }

            return tasks;
        }

        public bool MarkTaskCompleted(int id)
        {
            if (!EnsureReady())
            {
                return false;
            }

            try
            {
                using var connection = new MySqlConnection(DatabaseConnectionString);
                connection.Open();

                string sql = "UPDATE cyber_tasks SET is_completed = TRUE WHERE id = @id;";

                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public bool DeleteTask(int id)
        {
            if (!EnsureReady())
            {
                return false;
            }

            try
            {
                using var connection = new MySqlConnection(DatabaseConnectionString);
                connection.Open();

                string sql = "DELETE FROM cyber_tasks WHERE id = @id;";

                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public List<CyberTask> GetDueRemindersAndMarkShown()
        {
            var dueTasks = new List<CyberTask>();

            if (!EnsureReady())
            {
                return dueTasks;
            }

            try
            {
                using var connection = new MySqlConnection(DatabaseConnectionString);
                connection.Open();

                string sql = @"
                    SELECT id, title, description, reminder_date, is_completed, created_at
                    FROM cyber_tasks
                    WHERE reminder_date IS NOT NULL
                    AND reminder_date <= @now
                    AND is_completed = FALSE
                    AND reminder_shown = FALSE;";

                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@now", DateTime.Now);

                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int reminderIndex = reader.GetOrdinal("reminder_date");

                    dueTasks.Add(new CyberTask
                    {
                        Id = reader.GetInt32("id"),
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        ReminderDate = reader.IsDBNull(reminderIndex) ? null : reader.GetDateTime(reminderIndex),
                        IsCompleted = reader.GetBoolean("is_completed"),
                        CreatedAt = reader.GetDateTime("created_at")
                    });
                }

                reader.Close();

                foreach (var task in dueTasks)
                {
                    string updateSql = "UPDATE cyber_tasks SET reminder_shown = TRUE WHERE id = @id;";
                    using var updateCommand = new MySqlCommand(updateSql, connection);
                    updateCommand.Parameters.AddWithValue("@id", task.Id);
                    updateCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }

            return dueTasks;
        }
    }
}