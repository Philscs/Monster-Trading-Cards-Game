using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonsterTradingCardGame.Classes;
using Npgsql;
using NpgsqlTypes;
using BCryptNet = BCrypt.Net.BCrypt;

namespace MonsterTradingCardGame.PostgreDB
{
    class DBConn
    {
        private string _connString = "Server=localhost;Port=5432;User Id=postgres;Password=admin;";

        private static NpgsqlConnection Connection(string connString)
        {
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            return conn;
        }

        public User LoginUser(string username, string pwd)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            NpgsqlDataReader? dr = null;
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "select \"userName\", \"userCoins\", \"userElo\", \"userPwd\" from \"gameUser\" where \"userName\" = @p1;",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, username);

                dr = userCmd.ExecuteReader();
                User user = null;
                while (dr.Read())
                {
                    user = new User(dr.GetString(0), dr.GetInt32(1), dr.GetInt32(2), dr.GetString(3));
                }

                if (user != null && BCryptNet.Verify(pwd, user.password))
                {
                    //Console.WriteLine();
                    //user.PrintUserInformation();
                    dr.Close();
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                //Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }

        public bool RegisterUser(User user)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "insert into \"gameUser\" (\"userName\" , \"userElo\", \"userCoins\", \"userPwd\") values(@p1, @p2, @p3, @p4);",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, user.UniqueUsername);
                userCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, user.UserElo);
                userCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Bigint, user.Coins);
                userCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Varchar, user.password);

                userCmd.ExecuteNonQuery();
                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"catch Exception: {ex}");
                return Rollback(transaction);
            }
        }

        private static bool Rollback(NpgsqlTransaction transaction)
        {
            transaction.Rollback();
            return false;
        }
        //using var cmd = new NpgsqlCommand("SELECT * FROM user WHERE (username=@p1)", conn);
        //cmd.Parameters.AddWithValue("p1", username);

        //        string sql = "SELECT * FROM user";

        //        using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
        //        {
        //            NpgsqlDataReader reader = cmd.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                var name = reader[0];
        //                Console.WriteLine("after read, before DB output");
        //                Console.WriteLine($"The Name of the User is {name}");
        //            }

        //        }
        //        //if (!reader.HasRows) return;
        //        //reader.Read();

        //        Console.ReadLine();

        //        //return User;
        //    }
    }
}
