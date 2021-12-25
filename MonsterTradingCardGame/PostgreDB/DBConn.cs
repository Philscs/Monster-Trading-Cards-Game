using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MonsterTradingCardGame.Classes;
using MonsterTradingCardGame.Enum;
using Npgsql;
using NpgsqlTypes;
using BCryptNet = BCrypt.Net.BCrypt;

namespace MonsterTradingCardGame.PostgreDB
{
    class DBConn
    {
        private string _connString = "Server=localhost;Port=5432;User Id=postgres;Password=admin;Include Error Detail = true";

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
                    "select \"userID\", \"userName\", \"userCoins\", \"userElo\", \"userPwd\", \"userWins\", \"userLoses\" from \"gameUser\" where \"userName\" = @p1;",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, username);
                
                dr = userCmd.ExecuteReader();
                User user = null;
                while (dr.Read())
                {
                    user = new User(dr.GetInt32(0),dr.GetString(1), dr.GetInt32(2), dr.GetInt32(3), dr.GetString(4), dr.GetInt32(5), dr.GetInt32(6));
                }

                if (user != null && BCryptNet.Verify(pwd, user.password))
                {
                    //Console.WriteLine();
                    //user.PrintUserInformation();
                    //Console.ReadLine();
                    dr.Close();
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }

        public User RegisterUser(User user)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "insert into \"gameUser\" (\"userName\" , \"userElo\", \"userCoins\", \"userPwd\") values(@p1, @p2, @p3, @p4) returning \"userID\";",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, user.UniqueUsername);
                userCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, user.UserElo);
                userCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Bigint, user.Coins);
                userCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Varchar, user.password);
                userCmd.ExecuteNonQuery();
                transaction.Commit();

                int userID = GetUserIDReg(user.UniqueUsername);

                if (userID != 0)
                {
                    //Console.WriteLine($"DB register; good");
                    User registerUser = new User(userID, user.UniqueUsername, user.Coins, user.UserElo, user.password, user.Wins, user.Loses);
                    //Console.WriteLine($"user id: {registerUser.UserID}");
                    Console.ReadLine();
                    return registerUser;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }

        public int GetUserIDReg(string username)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            NpgsqlDataReader? dr = null;
            int userID = 0;
            try
            {
                using var userIDCmd = new NpgsqlCommand(
                    "select \"userID\" from \"gameUser\" where \"userName\" = @p1;",
                    conn);
                userIDCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, username);
                dr = userIDCmd.ExecuteReader();

                while (dr.Read())
                {
                    userID = dr.GetInt32(0);
                    //Console.WriteLine($"\n{userID}");
                     
                    Console.ReadLine();
                }

                dr.Close();
                return userID;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return userID;
            }

        }

        public List<Cards> GetUserPlayCardStack(User user)
        {
            List<int> cardIDs = new List<int>();
            List<Cards> cards = new List<Cards>();

            using var conn = Connection(_connString);;
            var transaction = conn.BeginTransaction();
            NpgsqlDataReader? dr = null;
            try
            {
                using var cardIDCmd = new NpgsqlCommand(
                    "select \"cardID\" from \"userPlayCards\" where \"gameUserID\" = @p1;",
                    conn);

                cardIDCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);

                dr = cardIDCmd.ExecuteReader();
                while (dr.Read())
                {
                    cardIDs.Add(dr.GetInt32(0));
                }

                dr.Close();

                foreach (var i in cardIDs)
                {
                    using var cardCmd = new NpgsqlCommand(
                        "select * from \"allCards\" where \"cardID\" = @p1;",
                        conn);

                    cardCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, i);

                    dr = cardCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Cards card = new Cards(dr.GetInt32(0), dr.GetString(1), (double)dr.GetInt32(2), (CardTypesEnum.CardElementEnum)dr.GetInt32(3), (CardTypesEnum.CardTypeEnum)dr.GetInt32(4));
                        cards.Add(card);
                    }
                    dr.Close();
                }
                
                return cards.Count > 0 ? cards : null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }
        public List<Cards> GetUserAllCardStack(User user)
        {
            List<int> cardIDs = new List<int>();
            List<Cards> cards = new List<Cards>();

            using var conn = Connection(_connString); ;
            var transaction = conn.BeginTransaction();
            NpgsqlDataReader? dr = null;
            try
            {
                using var cardIDCmd = new NpgsqlCommand(
                    "select \"allCardID\" from \"userAllCards\" where \"allGameUserID\" = @p1;",
                    conn);

                cardIDCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);

                dr = cardIDCmd.ExecuteReader();
                while (dr.Read())
                {
                    cardIDs.Add(dr.GetInt32(0));
                }

                dr.Close();

                foreach (var i in cardIDs)
                {
                    using var cardCmd = new NpgsqlCommand(
                        "select * from \"allCards\" where \"cardID\" = @p1;",
                        conn);

                    cardCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, i);

                    dr = cardCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Cards card = new Cards(dr.GetInt32(0), dr.GetString(1), (double)dr.GetInt32(2), (CardTypesEnum.CardElementEnum)dr.GetInt32(3), (CardTypesEnum.CardTypeEnum)dr.GetInt32(4));
                        cards.Add(card);
                    }
                    dr.Close();
                }

                return cards.Count > 0 ? cards : null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }

        private static bool Rollback(NpgsqlTransaction transaction)
        {
            transaction.Rollback();
            return false;
        }
    }
}
